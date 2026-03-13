using Coordinator.Models;
using Coordinator.Models.Entities;
using Coordinator.Models.Enums;
using Coordinator.Models.Requests;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;

namespace Coordinator.Services
{
    public class TransactionService(TwoPhaseCommitContext _context, IHttpClientFactory _httpClientFactory) : ITransactionService
    {
        HttpClient _accountHttpClient = _httpClientFactory.CreateClient("Accounts.API");
        HttpClient _ledgerHttpClient = _httpClientFactory.CreateClient("Ledger.API");

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();
            var nodes = await _context.Nodes.ToListAsync();
            nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new(transactionId)
                {
                    IsReady = Models.Enums.ReadyType.Pending,
                    TransactionState = Models.Enums.TransactionState.Pending,
                }
            });
            await _context.SaveChangesAsync();
            return transactionId;
        }

        public async Task PreapreServicesAsync(Guid transactionId, CreateTransferRequest request)
        {
            var payload = new PrepareTransferRequest(
                TransactionId: transactionId.ToString(),
                FromAccountId: request.FromAccountId,
                ToAccountId: request.ToAccountId,
                Amount: request.Amount);


            var transactionNodes = await _context.NodeStates
                .Include(ns => ns.Node)
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Accounts.API" => _accountHttpClient.PostAsJsonAsync("/ready", payload),
                        "Ledger.API" => _ledgerHttpClient.PostAsJsonAsync("/ready", payload)
                    });
                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.IsReady = result ? Models.Enums.ReadyType.Ready : Models.Enums.ReadyType.Unready;
                } 
                catch (Exception)
                {
                    transactionNode.IsReady = Models.Enums.ReadyType.Unready;
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
        {
            return (await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns => ns.IsReady == ReadyType.Ready);
        }

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach(var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Accounts.API" => _accountHttpClient.GetAsync($"commit/{transactionId}"),
                        "Ledger.API" => _ledgerHttpClient.GetAsync($"commit/{transactionId}")
                    });
                    var result = bool.Parse(await response.Content.ReadAsStringAsync());
                    transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;
                }
                catch (Exception)
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
        {
            return (await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(ns => ns.TransactionState == TransactionState.Done);
        }

        public async Task RoolbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Where(ns => ns.TransactionId == transactionId)
                .Include(ns => ns.Node)
                .ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    if (transactionNode.TransactionState == TransactionState.Done)
                    {
                        _ = await(transactionNode.Node.Name switch
                        {
                            "Accounts.API" => _accountHttpClient.GetAsync($"rollback/{transactionId}"),
                            "Ledger.API" => _ledgerHttpClient.GetAsync($"rollback/{transactionId}")
                        });
                    }
                    transactionNode.TransactionState = TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

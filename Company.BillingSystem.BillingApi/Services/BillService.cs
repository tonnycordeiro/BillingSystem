using Company.BillingSystem.BillingApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.BillingApi.Services
{
    public interface IBillService
    {
        Task<Bill> CreateAsync(Bill bill);
        IAsyncEnumerable<Bill> GetAsync(string personId, int? month);
    }

    public class BillService : IBillService
    {
        private readonly IMongoCollection<Bill> _bills;

        public BillService(IBillingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _bills = database.GetCollection<Bill>(settings.BillsCollectionName);
        }

        public async Task<Bill> CreateAsync(Bill bill)
        {
            await _bills.InsertOneAsync(bill);
            return bill;
        }

        public async IAsyncEnumerable<Bill> GetAsync(string personId, int? month)
        {
            FilterDefinitionBuilder<Bill> builder = Builders<Bill>.Filter;
            FilterDefinition<Bill> filter = FilterDefinition<Bill>.Empty;
            Bill bill;

            if (!String.IsNullOrEmpty(personId))
            {
                filter &= builder.Eq(nameof(bill.PersonId), personId);
            }

            if (month.HasValue)
            {
                filter &= builder.Eq(nameof(bill.DueMonth), month);
            }

            using (IAsyncCursor<Bill> cursor = await _bills.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (Bill currentBill in cursor.Current)
                    {
                        yield return currentBill;
                    }
                }
            }
        }
    }
}

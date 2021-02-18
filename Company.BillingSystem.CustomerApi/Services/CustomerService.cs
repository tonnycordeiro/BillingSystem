using Company.BillingSystem.CustomerApi.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Services
{
    public interface ICustomerService
    {
        Task CreateAsync(Customer customer);
        Task<Customer> GetAsync(string id);
        IAsyncEnumerable<string> GetIdListAsync();
    }

    public class CustomerService : ICustomerService
    {
        private readonly CollectionReference _collection;

        public CustomerService(ICustomerRegistryDatabaseSettings settings)
        {
            FirestoreDb db = FirestoreDb.Create(settings.ProjectId);
            _collection = db.Collection(settings.CustomersCollectionName);
        }

        public async Task CreateAsync(Customer customer)
        {
            DocumentReference document = _collection.Document(customer.Id);
            Dictionary<string, object> customerData = new Dictionary<string, object>()
            {
                {nameof(customer.Name), customer.Name },
                {nameof(customer.State), customer.State },
                {nameof(customer.Cpf), customer.Cpf }
            };

            await document.SetAsync(customerData);
        }

        public async Task<Customer> GetAsync(string id)
        {
            QuerySnapshot querySnapshot = await _collection.GetSnapshotAsync();
            DocumentSnapshot document = querySnapshot.FirstOrDefault(d => d.Id == id);

            if (document == null)
                return null;

            return document.ConvertTo<Customer>();
        }

        public async IAsyncEnumerable<string> GetIdListAsync()
        {
            QuerySnapshot querySnapshot = await _collection.GetSnapshotAsync();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                yield return documentSnapshot.Id;
            }
        }
    }
}

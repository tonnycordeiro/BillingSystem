using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Company.BillingSystem.CustomerApi.Models
{
    [FirestoreData]
    public class Customer
    {
        [JsonIgnore]
        public string Id => Cpf;

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public string State { get; set; }

        [FirestoreProperty]
        public string Cpf { get; set; }
    }
}

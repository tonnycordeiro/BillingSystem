using Company.BillingSystem.ReportApi.Models;
using Company.BillingSystem.ReportApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace Company.BillingSystem.ReportApi.Managers
{
    public class StateBillingReportManager
    {
        private const string STATE_FOR_NOT_REGISTERED_CUSTOMER = "NO REGISTER";

        private StateBillingReport _stateBillingReport;
        private IBillingApiService _billingApiService;
        private ICustomerApiService _customerApiService;
        private IReportGeneratorSettings _reportGeneratorSettings;

        public StateBillingReportManager(int month, IReportGeneratorSettings reportGeneratorSettings,
                                        IBillingApiService billingApiService, ICustomerApiService customerApiService)
        {
            _stateBillingReport = new StateBillingReport(){Month = month };
            _billingApiService = billingApiService;
            _customerApiService = customerApiService;
            _reportGeneratorSettings = reportGeneratorSettings;
        }

        /// <summary>
        /// This generate state billing report through "Map Reduce" pattern, with parallelization.
        /// The maximum number of threads is equivalent to processors count (default), but can be customized in appsettings.json
        /// </summary>
        public async Task<StateBillingReport> CreateReport()
        {
            ConcurrentDictionary<string, string> personIdToSateMap = new ConcurrentDictionary<string, string>();
            ConcurrentDictionary<string, List<double>> stateToValueListMap = new ConcurrentDictionary<string, List<double>>();
            ConcurrentDictionary<string, double> stateByValueMap = new ConcurrentDictionary<string, double>();


            List<Bill> bills = (await _billingApiService.GetByMonthAsync(_stateBillingReport.Month)).ToList();
            personIdToSateMap = GetPersonIdToStateDic(bills, _reportGeneratorSettings.MaxNumberOfConcurrentTasks.Value);

            _stateBillingReport.StateByValueDic = new Dictionary<string, double>();

            //MAP
            Parallel
                .ForEach(bills,
                new ParallelOptions { MaxDegreeOfParallelism = _reportGeneratorSettings.MaxNumberOfConcurrentTasks.Value },
                bill =>
                {
                    stateToValueListMap.AddOrUpdate(personIdToSateMap[bill.PersonId], new List<double> { bill.Value },
                        (key, oldValue) => { oldValue.Add(bill.Value); return oldValue; });
                });

            //REDUCE
            Parallel.ForEach(stateToValueListMap,
                new ParallelOptions { MaxDegreeOfParallelism = _reportGeneratorSettings.MaxNumberOfConcurrentTasks.Value },
                pair =>
                {
                    stateByValueMap.TryAdd(pair.Key, pair.Value.Sum());
                });

            _stateBillingReport.StateByValueDic = stateByValueMap;

            return _stateBillingReport;
        }

        private ConcurrentDictionary<string, string> GetPersonIdToStateDic(List<Bill> bills, int maxNumberOfConcurrentTasks)
        {
            ConcurrentDictionary<string, string> personIdBySate = new ConcurrentDictionary<string, string>();
            ConcurrentQueue<int[]> rangeInfoQueue = GetRangeInfoToSplitCollections(bills.Count(), maxNumberOfConcurrentTasks);

            maxNumberOfConcurrentTasks = rangeInfoQueue.Count;
            Task[] customerGettingTasks = new Task[maxNumberOfConcurrentTasks];

            for (int i = 0; i < maxNumberOfConcurrentTasks; i++)
            {
                customerGettingTasks[i] = Task.Run
                (
                    async () =>
                    {
                        int[] rangeInfo;
                        rangeInfoQueue.TryDequeue(out rangeInfo);

                        await FillPersonIdToState(personIdBySate, bills, rangeInfo);
                    }
                );
            }

            Task.WaitAll(customerGettingTasks);

            return personIdBySate;
        }

        private async Task<bool> FillPersonIdToState(ConcurrentDictionary<string, string> personIdToSateDic, List<Bill> bills, int[] rangeInfo)
        {
            foreach(Bill bill in bills.GetRange(rangeInfo[0], rangeInfo[1]))
            {
                Customer customer = await _customerApiService.GetCustomerAsync(bill.PersonId);
                string state = (customer == null ? STATE_FOR_NOT_REGISTERED_CUSTOMER : customer.State);
                personIdToSateDic.TryAdd(bill.PersonId, state);
            }

            return true;
        }

        private ConcurrentQueue<int[]> GetRangeInfoToSplitCollections(int collectionSize, int numberOfDivisions)
        {
            ConcurrentQueue<int[]> rangeInfoQueue = new ConcurrentQueue<int[]>();
            Task[] customerGettingTasks = new Task[numberOfDivisions];
            int rangeSize = collectionSize / numberOfDivisions;
            int rest = collectionSize % numberOfDivisions;

            int nextIndex = 0;

            for (int i = 0; i < numberOfDivisions; i++)
            {
                int count = rangeSize + (rest-- > 0 ? 1 : 0);
                if (count == 0)
                    break;

                rangeInfoQueue.Enqueue(new int[] { nextIndex, count });
                nextIndex += count;
            }

            return rangeInfoQueue;
        }
    }
}

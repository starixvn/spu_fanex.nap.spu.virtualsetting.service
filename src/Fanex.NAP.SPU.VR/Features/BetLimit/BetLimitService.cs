namespace Fanex.NAP.SPU.VR.Features.BetLimit
{
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;

    public interface IBetLimitService
    {
        IEnumerable<int> GetBetLimitCustomers();

        (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList);

        void RemoveBetLimit(IEnumerable<int> customerList, int punterChunkSize);

        void UpdateBetLimit(IEnumerable<int> customerList, int punterChunkSize);

        public int PuntersUpdated { get; }

        public int RemovedCustomers { get; }

        public int UpdateCustomers { get; }

        public int RawCustomer { get; }

    }
    public class BetLimitService : IBetLimitService
    {
        private readonly IBetLimitDataAccess betLimitDataAccess;

        private int puntersUpdated;

        private int removedCustomers;

        private int updatedCustomers;

        private int rawCustomers;

        public BetLimitService(IBetLimitDataAccess betLimitDataAccess)
        {
            this.betLimitDataAccess = betLimitDataAccess;

            this.puntersUpdated = 0;
        }

        public int PuntersUpdated => puntersUpdated;

        public int RemovedCustomers => removedCustomers;

        public int UpdateCustomers => updatedCustomers;

        public int RawCustomer => rawCustomers;

        public IEnumerable<int> GetBetLimitCustomers()
        {
            var betLimitCustomers = betLimitDataAccess.GetCustomersBetLimit();

            this.rawCustomers = betLimitCustomers.Count();

            return betLimitCustomers;
        }

        public void RemoveBetLimit(IEnumerable<int> customerList, int punterChunkSize)
        {
            this.removedCustomers = customerList.Count();

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            foreach (var customers in customerChunks)
            {
                if(customers != null && customers.Any())
                {
                    betLimitDataAccess.RemoveBetLimit(string.Join(",", customers));
                }                
            }
        }

        public void UpdateBetLimit(IEnumerable<int> customerList, int punterChunkSize)
        {
            this.updatedCustomers = customerList.Count();

            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            foreach(var customers in customerChunks)
            {
                if (customers != null && customers.Any())
                {
                    betLimitDataAccess.UpdateBetLimit(string.Join(",", customers));

                    puntersUpdated += customers.Count();
                }                    
            }
        }

        public (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList)
        {
            return betLimitDataAccess.VerifyCustomersBetLimit(customerList);
        }
    }
}

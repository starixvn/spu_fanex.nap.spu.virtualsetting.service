using Fanex.NAP.SPU.CTS.Shares;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NAP.SPU.VR.Features.BetLimit
{
    public interface IBetLimitServices
    {
        IEnumerable<int> GetBetLimitCustomers();

        (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList);

        void RemoveBetLimit(IEnumerable<int> customerList, int punterChunkSize);

        void UpdateBetLimit(IEnumerable<int> customerList, int punterChunkSize);
    }
    public class BetLimitServices : IBetLimitServices
    {
        private readonly IBetLimitDataAccess betLimitDataAccess;

        public BetLimitServices(IBetLimitDataAccess betLimitDataAccess)
        {
            this.betLimitDataAccess = betLimitDataAccess;
        }

        public IEnumerable<int> GetBetLimitCustomers()
        {
            return betLimitDataAccess.GetCustomersBetLimit();
        }

        public void RemoveBetLimit(IEnumerable<int> customerList, int punterChunkSize)
        {
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
            var customerChunks = Utilities.ChunkList(customerList, punterChunkSize).ToList();

            foreach(var customers in customerChunks)
            {
                if (customers != null && customers.Any())
                {
                    betLimitDataAccess.UpdateBetLimit(string.Join(",", customers));
                }                    
            }
        }

        public (IEnumerable<int>, IEnumerable<int>) VerifyBetLimitCustomers(string customerList)
        {
            return betLimitDataAccess.VerifyCustomersBetLimit(customerList);
        }
    }
}

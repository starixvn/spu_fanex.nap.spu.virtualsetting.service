namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Criteria;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IVirtualSettingDataAccess
    {
        IEnumerable<int> GetBACustIds(string custIds);

        IEnumerable<CustomerVirtualRateEntity> GetCurrentCustomerVirtualRate(string custIds);

        IEnumerable<PunterEntity> GetWinningPunterSoccerList(string siteIds, string superIds, double virtualRateThreshold);

        IEnumerable<PunterEntity> GetWinningPunterList(string siteIds, string superIds, double virtualRateThreshold);

        DataResult UpdatePunterVirtualSetting(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip, short sportType);

        DataResult LogPunterVirtualSetting(string serviceName, IEnumerable<PunterEntity> punters, int userId, DateTime adjustDate);

        DataResult InsertCustomerMarginHistory(IEnumerable<PunterEntity> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate);
    }

    public class VirtualSettingDataAccess : IVirtualSettingDataAccess
    {
        private readonly IDynamicRepository repository;

        public VirtualSettingDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<int> GetBACustIds(string custIds)
        {
            var criteria = new GetBAAccountsCriteria
            {
                CustIds = custIds
            };

            return this.repository.Fetch<int>(criteria);
        }

        public IEnumerable<CustomerVirtualRateEntity> GetCurrentCustomerVirtualRate(string custIds)
        {
            var criteria = new GetCurrentCustomerVirtualRateCriteria
            {
                CustIds = custIds
            };

            return this.repository.Fetch<CustomerVirtualRateEntity>(criteria);
        }

        public IEnumerable<PunterEntity> GetWinningPunterSoccerList(string siteIds, string superIds, double virtualRateThreshold)
        {
            var criteria = new GetProPrunterSoccerCriteria
            {
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return this.repository.Fetch<PunterEntity>(criteria);
        }

        public IEnumerable<PunterEntity> GetWinningPunterList(string siteIds, string superIds, double virtualRateThreshold)
        {
            var criteria = new GetProPrunterCriteria
            {
                SiteIds = siteIds,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return this.repository.Fetch<PunterEntity>(criteria);
        }

        public DataResult UpdatePunterVirtualSetting(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip, short sportType)
        {
            var cusIds = string.Join(",", punters.Select(x => x.CustID));

            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = Utilities.GenerateVirtualSettingXmlForUpdating(cusIds, virtualMultiplier),
                UserId = userId,
                IP = ip,
                SportType = sportType
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);

                return new DataResult
                {
                    ErrorCode = criteria.ErrCode,
                    ErrorMessage = criteria.ErrMsg
                };
            }
        }

        public DataResult LogPunterVirtualSetting(string serviceName, IEnumerable<PunterEntity> punters, int userId, DateTime adjustDate)
        {
            var rate = string.Empty;

            foreach (var punter in punters)
            {
                object[] objParams =
                {
                    punter.CustID,
                    punter.ReportDate.ToShortDateString(),
                    punter.SiteID,
                    punter.SRecommend,
                    punter.MRecommend,
                    punter.Recommend,
                    punter.OldVirtualRate,
                    punter.NewVirtualRate,
                    punter.IsBA
                };

                rate += Utilities.GenerateRateVirtualSettingXmlForLogging(objParams);
            }

            var criteria = new LogVirtualSettingCriteria
            {
                PunterVRSettingsDoc = Utilities.GenerateVirtualSettingXmlFromRate(rate),
                CreatorId = userId,
                ServiceName = serviceName,
                AdjustDate = adjustDate
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);

                return new DataResult
                {
                    ErrorCode = criteria.ErrCode,
                    ErrorMessage = criteria.ErrMsg
                };
            }
        }

        public DataResult InsertCustomerMarginHistory(IEnumerable<PunterEntity> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate)
        {
            var rate = string.Empty;
            var customerMargins = GetCustomerMarginHistory(punters, sportGroupID, numOfMonth);

            foreach (var customerMargin in customerMargins)
            {
                object[] objParams =
                {
                    customerMargin.CustId,
                    customerMargin.ReportMonth,
                    customerMargin.ReportYear,
                    customerMargin.Turnover,
                    customerMargin.Winloss,
                    customerMargin.BetCount,
                    serviceName
                };

                rate += Utilities.GenerateRateVirtualSettingXmlForCustomerMarginHistory(objParams);
            }

            var doc = Utilities.GenerateVirtualSettingXmlFromRate(rate);

            return this.InsertCustomerMarginHistory(doc, adjustDate);
        }

        private IEnumerable<CustomerMarginEntity> GetCustomerMarginHistory(IEnumerable<PunterEntity> punters, byte sportGroupID, int numOfMonth)
        {
            var criteria = new GetCustomerMarginHistoryCriteria
            {
                CustIds = string.Join(",", punters.Select(c => c.CustID).Distinct()),
                SportGroupID = sportGroupID,
                NumOfMonth = numOfMonth
            };

            return this.repository.Fetch<CustomerMarginEntity>(criteria);
        }

        private DataResult InsertCustomerMarginHistory(string customerMarginDoc, DateTime adjustDate)
        {
            var criteria = new InsertCustomerMarginHistoryCriteria
            {
                CustomerMarginDoc = customerMarginDoc,
                AdjustDate = adjustDate,
                ErrCode = 0,
                ErrMsg = string.Empty
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(criteria);

                return new DataResult
                {
                    ErrorCode = criteria.ErrCode,
                    ErrorMessage = criteria.ErrMsg
                };
            }
        }
    }
}

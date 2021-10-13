namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;

    public interface IDangerLevelSapaSportDataAccess
    {
        int IsDataSabaSportDangerLevelReady();

        IEnumerable<SapaSportDangerLevelEntity> GetSapaSportDangerLevel(bool includeUUS = true);

        UpdateDangerResultEntity UpdateDangerLevelToMainDB(IEnumerable<SapaSportDangerLevelEntity> winLossSapaSportInfo);
    }

    public class DangerLevelSapaSportDataAccess : IDangerLevelSapaSportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public DangerLevelSapaSportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public int IsDataSabaSportDangerLevelReady()
        {
            var criteria = new SabaSportDangerLevelDateReadyCheckCriteria();

            return _repository.Execute(criteria);
        }

        public IEnumerable<SapaSportDangerLevelEntity> GetSapaSportDangerLevel(bool includeUUS = true)
        {
            var criteria = new SabaSportDangerLevelDateReadyCheckCriteria
            {
                IncludeUUS = includeUUS
            };

            return _repository.Fetch<SapaSportDangerLevelEntity>(criteria);
        }

        public UpdateDangerResultEntity UpdateDangerLevelToMainDB(IEnumerable<SapaSportDangerLevelEntity> winLossSapaSportInfo)
        {
            var criteria = new SabaSportDangerLevelUpdateCriteria
            {
                CustsToXML = winLossSapaSportInfo.ToXmlString()
            };

            _repository.Execute(criteria);

            return new UpdateDangerResultEntity
            {
                ErrorCode = criteria.ErrorCode,
                ErrorMsg = criteria.ErrorMessage,
                InPutParam = winLossSapaSportInfo
            };
        }
    }
}
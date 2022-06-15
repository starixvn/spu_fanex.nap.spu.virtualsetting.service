namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    using Fanex.Data.Repository;
    using Fanex.NAP.SPU.CTS.CC.Features.SabaSportDangerLevel;
    using Fanex.NAP.SPU.CTS.Shares;
    using System.Collections.Generic;
    using System.Linq;
    using static Fanex.NAP.SPU.VR.Shares.Consts;

    public interface IDangerLevelSapaSportDataAccess
    {
        int CheckDataSabaSportDangerLevelReady();

        IEnumerable<SapaSportDangerLevelEntity> GetSapaSportDangerLevel(int environment);

        UpdateDangerResultEntity UpdateDangerLevelToMainDB(IEnumerable<SapaSportDangerLevelEntity> winLossSapaSportInfo, int environtment);
    }

    public class DangerLevelSapaSportDataAccess : IDangerLevelSapaSportDataAccess
    {
        private readonly IDynamicRepository _repository;

        public DangerLevelSapaSportDataAccess()
        {
            _repository = new DynamicRepository();
        }

        public int CheckDataSabaSportDangerLevelReady()
        {
            var criteria = new SabaSportDangerLevelDateReadyCheckCriteria();

            return _repository.Fetch<int>(criteria)?.FirstOrDefault() ?? 0;
        }

        public IEnumerable<SapaSportDangerLevelEntity> GetSapaSportDangerLevel(int environment)
        {
            var criteria = new SabaSportDangerLevelGetCriteria();

            //if (environment == (int)Environment.PRO)
            //{
            //    criteria.IncludeUUS = 1;
            //}

            //if (environment == (int)Environment.UAT)
            //{
            //    criteria.IncludeUUS = 0;
            //}

            criteria.IncludeUUS = 1;

            return _repository.Fetch<SapaSportDangerLevelEntity>(criteria);
        }

        public UpdateDangerResultEntity UpdateDangerLevelToMainDB(IEnumerable<SapaSportDangerLevelEntity> winLossSapaSportInfo, int environtment)
        {
            var criteria = new SabaSportDangerLevelUpdateCriteria
            {
                CustsToXML = winLossSapaSportInfo.ToXmlString()
            };

            //if (environtment == (int)Environment.PRO)
            //{
            //    criteria.Enviroment = "PRO";
            //    criteria.IncludeUUS = 1;
            //}
            
            //if (environtment == (int)Environment.UAT)
            //{
            //    criteria.Enviroment = "UAT";
            //    criteria.IncludeUUS = 0;
            //}

            criteria.Enviroment = "UAT";
            criteria.IncludeUUS = 1;

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
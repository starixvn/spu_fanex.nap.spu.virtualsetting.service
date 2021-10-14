using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.SPU.CTS.Shares;
using Fanex.NAP.SPU.VR.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    public interface IDangerLevelSapaSportService
    {
        bool IsDataSabaSportDangerLevelReady(int retryTimesCheckDataReady);

        IDangerLevelSapaSportService Start();

        IDangerLevelSapaSportService GetSapaSportDangerLevel(int environtment);

        UpdateDangerResultEntity RetryUpdateDangerLevelToMainDB(int retryTimes, int batchSize);
    }

    public class DangerLevelSapaSportService : IDangerLevelSapaSportService
    {
        private readonly IDangerLevelSapaSportDataAccess dataAccess;
        private IEnumerable<SapaSportDangerLevelEntity> winlossSapaSportInfo;

        public DangerLevelSapaSportService(IDangerLevelSapaSportDataAccess dangerLevelSapaSportDataAccess)
        {
            this.dataAccess = dangerLevelSapaSportDataAccess;
        }

        public IDangerLevelSapaSportService Start()
        {
            winlossSapaSportInfo = new List<SapaSportDangerLevelEntity>();

            return this;
        }

        public bool IsDataSabaSportDangerLevelReady(int retryTimesCheckDataReady)
        {
            int countRetry = 0;

            while (countRetry < retryTimesCheckDataReady)
            {
                bool isDataReady = dataAccess.CheckDataSabaSportDangerLevelReady() == 1;

                if (isDataReady)
                {
                    return true;
                }

                countRetry++;

                Thread.Sleep(Consts.SleepTime_SapaSportDangerLevel);
            }

            return false;
        }

        public IDangerLevelSapaSportService GetSapaSportDangerLevel(int environtment)
        {
            winlossSapaSportInfo = dataAccess.GetSapaSportDangerLevel(environtment);

            return this;
        }

        public UpdateDangerResultEntity RetryUpdateDangerLevelToMainDB(int retryTimes, int batchSize)
        {
            var successBatch = new List<SapaSportDangerLevelEntity>();

            if (winlossSapaSportInfo != null && winlossSapaSportInfo.Any())
            {
                var winlossInfoChunk = winlossSapaSportInfo.ChunkList(batchSize);

                foreach (var batchData in winlossInfoChunk)
                {
                    var result = RetryUpdateDangerLevel(batchData, retryTimes);

                    if (result.ErrorCode != 0)
                    {
                        result.InPutParam = winlossSapaSportInfo.Except(successBatch);

                        return result;
                    }

                    successBatch.AddRange(batchData);
                }
            }

            return new UpdateDangerResultEntity { ErrorCode = 0, ErrorMsg = string.Empty };
        }

        private UpdateDangerResultEntity RetryUpdateDangerLevel(IEnumerable<SapaSportDangerLevelEntity> data, int retryTimes)
        {
            var result = new UpdateDangerResultEntity { ErrorCode = 0, ErrorMsg = string.Empty };

            if (data == null || !data.Any())
            {
                return result;
            }

            while (retryTimes > 0)
            {
                try
                {
                    result = dataAccess.UpdateDangerLevelToMainDB(data);

                    if (result.ErrorCode != 0)
                    {
                        return result;
                    }

                    retryTimes--;
                }
                catch (Exception ex)
                {
                    var logMessage = new LogMessage(ex.Message)
                                            .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                                            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    result = new UpdateDangerResultEntity { ErrorCode = -1, ErrorMsg = logMessage, InPutParam = data };

                    retryTimes--;
                }
            }

            return result;
        }
    }
}
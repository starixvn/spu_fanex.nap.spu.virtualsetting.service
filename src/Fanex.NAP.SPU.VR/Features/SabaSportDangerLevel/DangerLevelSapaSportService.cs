using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.NAP.SPU.VR.Shares;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Fanex.NAP.SPU.VR.Features.SabaSportDangerLevel
{
    public interface IDangerLevelSabaSportService
    {
        bool IsDataSabaSportDangerLevelReady(int retryTimesCheckDataReady);

        IDangerLevelSabaSportService Start();

        IDangerLevelSabaSportService GetSapaSportDangerLevel(int environtment);

        UpdateDangerResultEntity RetryUpdateDangerLevelToMainDB(int retryTimes, int batchSize, ISentryClient sentryClient, int environtment);
    }

    public class DangerLevelSapaSportService : IDangerLevelSabaSportService
    {
        private readonly IDangerLevelSapaSportDataAccess dataAccess;
        private IEnumerable<SapaSportDangerLevelEntity> winlossSapaSportInfo;

        public DangerLevelSapaSportService(IDangerLevelSapaSportDataAccess dangerLevelSapaSportDataAccess)
        {
            this.dataAccess = dangerLevelSapaSportDataAccess;
        }

        public IDangerLevelSabaSportService Start()
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

                Thread.Sleep(Consts.SleepTime_SapaSportCheckDangerLevel);
            }

            return false;
        }

        public IDangerLevelSabaSportService GetSapaSportDangerLevel(int environtment)
        {
            winlossSapaSportInfo = dataAccess.GetSapaSportDangerLevel(environtment);

            return this;
        }

        public UpdateDangerResultEntity RetryUpdateDangerLevelToMainDB(int retryTimes, int batchSize, ISentryClient sentryClient, int environtment)
        {
            var errorLog = new UpdateDangerResultEntity { ErrorCode = 0, ErrorMsg = string.Empty };

            if (winlossSapaSportInfo != null && winlossSapaSportInfo.Any())
            {
                var winlossInfoChunk = ChunkSize(winlossSapaSportInfo.OrderBy(c => c.CustID).ToList(), batchSize);

                foreach (var batchData in winlossInfoChunk)
                {
                    var result = RetryUpdateDangerLevel(batchData, retryTimes, environtment);

                    if (result.ErrorCode != 0)
                    {
                        errorLog = result;

                        var logMessage = new LogMessage($"VR - Update Sapa Sport Danger Level Job")
                                            .AddMessage($"Inner Exception: {result.ErrorMsg}")
                                            .AddMessage($"DateTime: {DateTime.Now}")
                                            .AddMessage($"Update DangerLevel To CustDB FAIL: {JsonConvert.SerializeObject(result.InPutParam)}").ToString();

                        sentryClient.CaptureMessage(logMessage, Sentry.Protocol.SentryLevel.Error);
                    }
                }
            }

            return errorLog;
        }

        private UpdateDangerResultEntity RetryUpdateDangerLevel(IEnumerable<SapaSportDangerLevelEntity> data, int retryTimes, int environtment)
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
                    result = dataAccess.UpdateDangerLevelToMainDB(data, environtment);

                    if (result.ErrorCode == 0)
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

                Thread.Sleep(Consts.SleepTime_SapaSportUpdateDangerLevel);
            }

            return result;
        }

        public static IEnumerable<IEnumerable<SapaSportDangerLevelEntity>> ChunkSize(List<SapaSportDangerLevelEntity> sportInfo, int batchSize)
        {
            var queue = new Queue<SapaSportDangerLevelEntity>();
            var totalItem = sportInfo.Count;

            for (int index = 0; index < totalItem; index++)
            {
                if (queue.Count >= batchSize)
                {
                    var prevIndex = index - 1;

                    if (sportInfo[index].CustID == sportInfo[prevIndex].CustID)
                    {
                        queue.Enqueue(sportInfo[index]);

                        continue;
                    }

                    var ret = queue;
                    queue = new Queue<SapaSportDangerLevelEntity>();

                    yield return ret;
                }

                queue.Enqueue(sportInfo[index]);
            }

            yield return queue;
        }
    }
}
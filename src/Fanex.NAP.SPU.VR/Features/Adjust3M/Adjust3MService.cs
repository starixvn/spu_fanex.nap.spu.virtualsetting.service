namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;   

    public interface IAdjust3MService
    {
        IAdjust3MService ResetPunter(bool isSoccer, VirtualSettingModel model);

        int CountPunterAffected();

        IAdjust3MService GetWinningPunterSoccerList(double virtualRateThreshold);

        IAdjust3MService GetWinningPunterList(double virtualRateThreshold);

        IAdjust3MService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports);

        IAdjust3MService AdjustVirtualSettingForSuperProPunter(int punterChunkSize, double virtualMultiplier);

        IAdjust3MService AdjustVirtualSettingForNormal(int punterChunkSize, double virtualMultiplier);
        
    }

    public class Adjust3MService : IAdjust3MService
    {
        private readonly IAdjust3MDataAccess adjust3MDataAccess;
        private VirtualSettingModel model;
        private IEnumerable<PunterEntity> punters { get; set; }
        private Dictionary<int, PunterEntity> puntersDict;
        private IEnumerable<PunterEntity> proPuntersUpdated;
        private IEnumerable<PunterEntity> normalPuntersUpdated;
        private bool isSoccer;

        public Adjust3MService(IAdjust3MDataAccess adjust3MDataAccess)
        {
            this.adjust3MDataAccess = adjust3MDataAccess;
        }

        public IAdjust3MService ResetPunter(bool isSoccer, VirtualSettingModel model)
        {
            this.model = model;
            this.punters = Enumerable.Empty<PunterEntity>();
            this.puntersDict = new Dictionary<int, PunterEntity>();
            this.proPuntersUpdated = Enumerable.Empty<PunterEntity>();
            this.normalPuntersUpdated = Enumerable.Empty<PunterEntity>();
            this.isSoccer = isSoccer;

            return this;
        }

        public int CountPunterAffected()
        {
            if (this.proPuntersUpdated != null && this.normalPuntersUpdated != null)
            {
                return this.proPuntersUpdated.Union(this.normalPuntersUpdated, this.model.PunterEqualityComparer).Count();
            }
            else if (this.proPuntersUpdated != null)
            {
                return this.proPuntersUpdated.Count();
            }
            else if (this.normalPuntersUpdated != null)
            {
                return this.normalPuntersUpdated.Count();
            }

            return 0;
        }        

        public IAdjust3MService GetWinningPunterSoccerList(double virtualRateThreshold)
        {
            this.punters = this.adjust3MDataAccess.GetWinningPunterSoccerList(this.model.SiteIds, this.model.SuperIds, virtualRateThreshold);

            return this;
        }

        public IAdjust3MService GetWinningPunterList(double virtualRateThreshold)
        {
            this.punters = this.adjust3MDataAccess.GetWinningPunterList(this.model.SiteIds, this.model.SuperIds, virtualRateThreshold);

            return this;
        }

        public IAdjust3MService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports)
        {
            if (this.punters == null || !this.punters.Any())
            {
                return this;
            }

            this.puntersDict = this.punters.Distinct().ToDictionary(p => p.CustID);
            var custIds = string.Join(",", this.puntersDict.Keys);

            var currentVRs = this.adjust3MDataAccess.GetCurrentCustomerVirtualRate(custIds);

            if (currentVRs != null && currentVRs.Any())
            {
                foreach (var currentVR in currentVRs)
                {
                    if (currentVR.VRSoccer < this.model.VirtualRateThreshold)
                    {
                        this.puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                        this.puntersDict[currentVR.CustID].SportType = sportType;
                    }
                    else if (compareVROtherSports && currentVR.VROtherSports < this.model.VirtualRateThreshold)
                    {
                        this.puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VROtherSports;
                        this.puntersDict[currentVR.CustID].SportType = 2;
                    }
                    else
                    {
                        this.puntersDict.Remove(currentVR.CustID);
                    }
                }
            }

            return this;
        }

        public IAdjust3MService AdjustVirtualSettingForSuperProPunter(int punterChunkSize, double virtualMultiplier)
        {
            if (this.puntersDict == null || !this.puntersDict.Any())
            {
                return this;
            }

            var proPunters = this.puntersDict.Values.Where(p => p.IsSuperProPunters);

            if (!proPunters.Any())
            {
                return this;
            }

            this.model.FailedUpdatePunterList.Clear();
        
            this.proPuntersUpdated = UpdateVirtualSetting(proPunters, punterChunkSize, virtualMultiplier);

            this.proPuntersUpdated = this.proPuntersUpdated.Except(this.model.FailedUpdatePunterList, this.model.PunterEqualityComparer);

            this.LogPunters(this.proPuntersUpdated, "Pro Punter(s)");

            return this;
        }

        public IAdjust3MService AdjustVirtualSettingForNormal(int punterChunkSize, double virtualMultiplier)
        {
            if (this.puntersDict == null || !this.puntersDict.Any())
            {
                return this;
            }

            var normalPunters = this.puntersDict.Values.Where(p => !p.IsSuperProPunters);

            if (!normalPunters.Any())
            {
                return this;
            }

            this.model.FailedUpdatePunterList.Clear();

            this.normalPuntersUpdated = UpdateVirtualSetting(normalPunters, punterChunkSize, virtualMultiplier);

            this.normalPuntersUpdated = this.normalPuntersUpdated.Except(this.model.FailedUpdatePunterList, this.model.PunterEqualityComparer);

            this.LogPunters(this.normalPuntersUpdated, "Normal Punter(s)");

            return this;
        }

        public IEnumerable<PunterEntity> UpdateVirtualSetting(IEnumerable<PunterEntity> punters, int chunkSize, double newVirtualRate)
        {            
            var chunkPunters = punters.ChunkList(chunkSize);

            foreach (var selectedPunters in chunkPunters)
            {
                try
                {
                    DetectBAAccounts(newVirtualRate, selectedPunters);
                   
                    var updateResult = UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, this.model.DummyUser, this.model.Ip);

                    if (!string.IsNullOrEmpty(updateResult.ErrorMessage))
                    {
                        this.model.FailedUpdatePunterList.AddRange(selectedPunters);

                        var jsonData = JsonConvert.SerializeObject(selectedPunters);

                        LogService.Error($"VR - Ajust 3M job - UpdatePunterVirtualSetting failed newVirtualRate {newVirtualRate} selected Punters(s) {jsonData}");
                    }
                    else
                    {
                        var serviceName = this.isSoccer ? "Soccer" : "Sportsbook";
                        
                        var logResult = this.adjust3MDataAccess.LogPunterVirtualSetting(serviceName, selectedPunters, this.model.DummyUser, this.model.AdjustDate);

                        if (!string.IsNullOrEmpty(logResult.ErrorMessage))
                        {
                            LogService.Error($"VR - Ajust 3M job - LogPunterVirtualSetting fail message {logResult.ErrorMessage}");
                        }

                        var insertResult = this.adjust3MDataAccess.InsertCustomerMarginHistory(selectedPunters, model.SportGroupID, 12, serviceName, model.AdjustDate);

                        if (!string.IsNullOrEmpty(insertResult.ErrorMessage))
                        {
                            LogService.Error($"VR - Ajust 3M job - InsertCustomerMarginHistory fail message {insertResult.ErrorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.model.FailedUpdatePunterList.AddRange(selectedPunters);

                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                    LogService.Error($"VR - Ajust 3M job UpdateVirtualSetting exception message:  {logMessage}");
                }
            }

            return punters;
        }

        private DataResult UpdatePunterVirtualSetting(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip)
        {
            if (this.isSoccer)
            {
                return this.adjust3MDataAccess.UpdatePunterVirtualSetting(punters, virtualMultiplier, userId, ip, 1);
            }
            else
            {
                var result = new DataResult { ErrorMessage = string.Empty };

                var puntersForAllSports = punters.Where(p => p.SportType == 0);

                if (puntersForAllSports.Any())
                {
                    result = this.adjust3MDataAccess.UpdatePunterVirtualSetting(puntersForAllSports, virtualMultiplier, userId, ip, 0);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogService.Error($"Adjust3M Service - UpdatePunterVirtualSetting for all sport fail message {result.ErrorMessage}");
                    }                                        
                }

                var puntersForOtherSports = punters.Where(p => p.SportType == 2);

                if (puntersForOtherSports.Any())
                {
                    result = this.adjust3MDataAccess.UpdatePunterVirtualSetting(puntersForOtherSports, virtualMultiplier, userId, ip, 2);

                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        LogService.Error($"Adjust3M Service - UpdatePunterVirtualSetting for other sport fail message {result.ErrorMessage}");
                    }                    
                }

                return result;
            }           
        }

        private void DetectBAAccounts(double newVirtualRate, IEnumerable<PunterEntity> punters)
        {
            try
            {
                var custIds = string.Join(",", punters.Select(p => p.CustID));

                var listBAAccounts = this.adjust3MDataAccess.GetBACustIds(custIds);

                foreach (var p in punters)
                {
                    if (newVirtualRate == 3 && listBAAccounts.Contains(p.CustID))
                    {
                        p.NewVirtualRate = 1.0;
                        p.IsBA = true;
                    }
                    else
                    {
                        p.NewVirtualRate = newVirtualRate;
                    }
                }
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                   .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error($"VR - Ajust 3M job DetectBAAccounts exception message:  {logMessage}");
            }
        }

        private void LogPunters(IEnumerable<PunterEntity> punters, string punterType)
        {
            var sportType = this.isSoccer ? "Soccer" : "Sport book";

            if (punters != null && punters.Any())
            {
                var info = $"List of { sportType } {punterType} updated({punters.Count()}): { string.Join(",", punters.Select(p => p.CustID))}";

                LogService.Info(info);
            }

            if (this.model.FailedUpdatePunterList.Any())
            {
                var error = $"List of { sportType } {punterType} update fail({this.model.FailedUpdatePunterList.Count}) : {string.Join(",", this.model.FailedUpdatePunterList.Select(p => p.CustID))}";

                LogService.Error(error);
            }
        }

    }
}

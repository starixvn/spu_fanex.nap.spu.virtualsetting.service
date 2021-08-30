namespace Fanex.NAP.SPU.VR.Features.Adjust3M
{
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.SPU.CTS.Shares;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Entities;
    using Fanex.NAP.SPU.VR.Features.Adjust3M.Model;
    using Fanex.NAP.SPU.VR.Shares;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPunterService
    {
        IPunterService GetWinningPunterSoccerList();

        IPunterService GetWinningPunterList();

        IPunterService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports);

        IPunterService AdjustVirtualSettingForSuperProPunter(UpdateVirtualSettingEntity updateVirtualSetting, double newVirtualRate);

        IPunterService AdjustVirtualSettingForNormal(UpdateVirtualSettingEntity updateVirtualSetting, double newVirtualRate);

        int CountAffected();
    }

    public class PunterService : IPunterService
    {
        private readonly bool _soccer;
        private readonly IAdjust3MDataAccess _dataAccess;
        private readonly VirtualSettingModel _model;
        private IEnumerable<PunterEntity> _punters { get; set; }
        private Dictionary<int, PunterEntity> _puntersDict;
        private IEnumerable<PunterEntity> _proPuntersUpdated;
        private IEnumerable<PunterEntity> _normalPuntersUpdated;

        public PunterService(IAdjust3MDataAccess dataAccess, VirtualSettingModel model, bool soccer = false)
        {
            _model = model;
            _soccer = soccer;
            _dataAccess = dataAccess;
        }

        public IPunterService GetWinningPunterSoccerList()
        {
            _punters = _dataAccess.GetWinningPunterSoccerList(_model.SiteIds, _model.SuperIds, _model.VirtualRateThreshold);

            if (_punters != null && _punters.Any())
            {
                LogService.Info($"List of Winning Punter Soccer is { string.Join(",", _punters.Select(p => p.CustID))}");
            }
            else
            {
                LogService.Info($"List of Winning Punter Soccer is 0");
            }

            return this;
        }

        public IPunterService GetWinningPunterList()
        {
            _punters = _dataAccess.GetWinningPunterList(_model.SiteIds, _model.SuperIds, _model.VirtualRateThreshold);

            if (_punters != null && _punters.Any())
            {
                LogService.Info($"List of Winning Punter is { string.Join(",", _punters.Select(p => p.CustID))}");
            }
            else
            {
                LogService.Info($"List of Winning Punter is 0");
            }

            return this;
        }

        public IPunterService FilterPunterBaseOnCurrentVirtualRate(short sportType, bool compareVROtherSports)
        {
            if (_punters == null || !_punters.Any())
            {
                return this;
            }

            _puntersDict = _punters.Distinct().ToDictionary(p => p.CustID);
            var custIds = string.Join(",", _puntersDict.Keys);

            var currentVRs = GetCurrentCustomerVirtualRate(custIds);

            if (currentVRs != null && currentVRs.Any())
            {
                foreach (var currentVR in currentVRs)
                {
                    if (currentVR.VRSoccer < _model.VirtualRateThreshold)
                    {
                        _puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                        _puntersDict[currentVR.CustID].SportType = sportType;
                    }
                    else if (compareVROtherSports && currentVR.VROtherSports < _model.VirtualRateThreshold)
                    {
                        _puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VROtherSports;
                        _puntersDict[currentVR.CustID].SportType = 2;
                    }
                    else
                    {
                        _puntersDict.Remove(currentVR.CustID);
                    }
                }
            }

            return this;
        }

        public IPunterService AdjustVirtualSettingForSuperProPunter(UpdateVirtualSettingEntity updateVirtualSetting, double newVirtualRate)
        {
            if (_puntersDict == null || !_puntersDict.Any())
            {
                LogService.Info($"List of AdjustVirtualSettingForSuperProPunter _puntersDict is 0");

                return this;
            }

            var proPunters = _puntersDict.Values.Where(p => p.IsSuperProPunters);

            if (!proPunters.Any())
            {
                LogService.Info($"List of AdjustVirtualSettingForSuperProPunter proPunters is 0");

                return this;
            }

            _model.FailedUpdatePunterList.Clear();

            _proPuntersUpdated = UpdateVirtualSetting(proPunters, updateVirtualSetting, newVirtualRate);

            _proPuntersUpdated = _proPuntersUpdated.Except(_model.FailedUpdatePunterList, _model.PunterEqualityComparer);

            this.LogPunters(_proPuntersUpdated, Consts.VSProPunter);

            return this;
        }

        public IPunterService AdjustVirtualSettingForNormal(UpdateVirtualSettingEntity updateVirtualSetting, double newVirtualRate)
        {
            if (_puntersDict == null || !_puntersDict.Any())
            {
                LogService.Info($"List of AdjustVirtualSettingForNormal _puntersDict is 0");

                return this;
            }

            var normalPunters = _puntersDict.Values.Where(p => !p.IsSuperProPunters);

            if (!normalPunters.Any())
            {
                LogService.Info($"List of AdjustVirtualSettingForNormal normalPunters is 0");

                return this;
            }

            _model.FailedUpdatePunterList.Clear();

            _normalPuntersUpdated = UpdateVirtualSetting(normalPunters, updateVirtualSetting, newVirtualRate);

            _normalPuntersUpdated = _normalPuntersUpdated.Except(_model.FailedUpdatePunterList, _model.PunterEqualityComparer);

            this.LogPunters(_normalPuntersUpdated, Consts.VSNormalPunter);

            return this;
        }

        public int CountAffected()
        {
            if (_proPuntersUpdated != null && _normalPuntersUpdated != null)
            {
                return _proPuntersUpdated.Union(_normalPuntersUpdated, _model.PunterEqualityComparer).Count();
            }
            else if (_proPuntersUpdated != null)
            {
                return _proPuntersUpdated.Count();
            }
            else if (_normalPuntersUpdated != null)
            {
                return _normalPuntersUpdated.Count();
            }
            else
            {
                return 0;
            }            
        }

        public IEnumerable<PunterEntity> UpdateVirtualSetting(IEnumerable<PunterEntity> punters, UpdateVirtualSettingEntity updateVirtualSetting, double newVirtualRate)
        {
            var userId = updateVirtualSetting.DummyUser;
            var ip = updateVirtualSetting.IP;
            var chunkSize = updateVirtualSetting.PunterChunkSize;
            var chunkPunters = punters.Partition(chunkSize);

            foreach (var selectedPunters in chunkPunters)
            {
                try
                {
                    DetectBAAccounts(newVirtualRate, selectedPunters);

                    var updateResult = UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, userId, ip);

                    if (!updateResult)
                    {
                        _model.FailedUpdatePunterList.AddRange(selectedPunters);

                        var jsonData = JsonConvert.SerializeObject(punters);

                        LogService.Error($"VR - Ajust 3M UpdatePunterVirtualSetting failed { _model.ServiceName } userId {userId} newVirtualRate {newVirtualRate} ip {ip} punter(s) {jsonData}");
                    }
                    else
                    {
                        LoggingUpdateVirtualSetting(userId, selectedPunters);

                        UpdateCustomerMarginHistory(selectedPunters);
                    }
                }
                catch (Exception ex)
                {
                    _model.FailedUpdatePunterList.AddRange(selectedPunters);

                    var logMessage = new LogMessage(ex.Message)
                       .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                       .AddMessage($"Service Name { _model.ServiceName }").ToString();

                    LogService.Error($"VR - Ajust 3M UpdateVirtualSetting exception {logMessage}");
                }
            }

            return punters;
        }

        private void DetectBAAccounts(double newVirtualRate, IEnumerable<PunterEntity> punters)
        {
            try
            {
                var custIds = string.Join(",", punters.Select(p => p.CustID));

                var listBAAccounts = _dataAccess.GetBACustIds(custIds);

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
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .AddMessage($"Service Name { _model.ServiceName }").ToString();

                LogService.Error($"VR - Ajust 3M DetectBAAccounts exception { logMessage }");
            }
        }

        private IEnumerable<CustomerVirtualRateEntity> GetCurrentCustomerVirtualRate(string custIds)
        {
            try
            {
                return _dataAccess.GetCurrentCustomerVirtualRate(custIds);
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .AddMessage($"Service Name { _model.ServiceName }").ToString();

                LogService.Error($"VR - Ajust 3M GetCurrentCustomerVirtualRate exception { logMessage }");
            }

            return new List<CustomerVirtualRateEntity>();
        }

        private bool UpdatePunterVirtualSetting(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip)
        {

            if (_soccer)
            {
                var result = _dataAccess.UpdatePunterVirtualSetting(punters, virtualMultiplier, userId, ip, 1);

                return string.IsNullOrEmpty(result.ErrorMessage);
            }
            else
            {
                return UpdatePunterVirtualSettingForAllSports(punters, virtualMultiplier, userId, ip) &&
                            UpdatePunterVirtualSettingForOtherSports(punters, virtualMultiplier, userId, ip);
            }
        }

        private bool UpdatePunterVirtualSettingForAllSports(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForAllSports = punters.Where(p => p.SportType == 0);

            if (puntersForAllSports != null && puntersForAllSports.Any())
            {
                var result = _dataAccess.UpdatePunterVirtualSetting(puntersForAllSports, virtualMultiplier, userId, ip, 0);

                var isSuccessful = string.IsNullOrEmpty(result.ErrorMessage);

                if (!isSuccessful)
                {
                    LogService.Error($"VR - Ajust 3M UpdatePunterVirtualSettingForAllSports failed { _model.ServiceName } message {result.ErrorMessage}");
                }

                return isSuccessful;
            }

            return true;
        }

        private bool UpdatePunterVirtualSettingForOtherSports(IEnumerable<PunterEntity> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForOtherSports = punters.Where(p => p.SportType == 2);

            if (puntersForOtherSports != null & puntersForOtherSports.Any())
            {
                var result = _dataAccess.UpdatePunterVirtualSetting(puntersForOtherSports, virtualMultiplier, userId, ip, 2);

                var isSuccessful = string.IsNullOrEmpty(result.ErrorMessage);

                if (!isSuccessful)
                {
                    LogService.Error($"VR - Ajust 3M UpdatePunterVirtualSettingForOtherSports failed { _model.ServiceName } message {result.ErrorMessage}");
                }

                return isSuccessful;
            }

            return true;
        }

        private void LoggingUpdateVirtualSetting(int userId, IEnumerable<PunterEntity> punters)
        {
            try
            {
                var logResult = _dataAccess.LogPunterVirtualSetting(_model.ServiceName, punters, userId, _model.AdjustDate);

                var isSuccessful = string.IsNullOrEmpty(logResult.ErrorMessage);

                if (!isSuccessful)
                {
                    var jsonData = JsonConvert.SerializeObject(punters);

                    LogService.Error($"VR - Ajust 3M LoggingUpdateVirtualSetting failed { _model.ServiceName } userId {userId} punter(s) {jsonData}");
                }
            }
            catch (Exception ex)
            {
                var jsonData = JsonConvert.SerializeObject(punters);

                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .AddMessage($"Service Name { _model.ServiceName } userId {userId}").ToString();

                LogService.Error($"VR - Ajust 3M LoggingUpdateVirtualSetting exception { logMessage } punter(s) {jsonData}");
            }
        }

        private void UpdateCustomerMarginHistory(IEnumerable<PunterEntity> punters)
        {
            try
            {
                var numOfMonth = 12;

                var result = _dataAccess.InsertCustomerMarginHistory(punters, _model.SportGroupID, numOfMonth, _model.ServiceName, _model.AdjustDate);

                var isSuccessful = string.IsNullOrEmpty(result.ErrorMessage);

                if (!isSuccessful)
                {
                    var jsonData = JsonConvert.SerializeObject(punters);

                    LogService.Error($"VR - Ajust 3M UpdateCustomerMarginHistory failed { _model.ServiceName } punter(s) {jsonData}");
                }
            }
            catch (Exception ex)
            {
                var jsonData = JsonConvert.SerializeObject(punters);

                var logMessage = new LogMessage(ex.Message)
                      .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                      .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                      .AddMessage($"Service Name { _model.ServiceName }").ToString();

                LogService.Error($"VR - Ajust 3M UpdateCustomerMarginHistory exception { logMessage } punter(s) {jsonData}");
            }
        }

        private void LogPunters(IEnumerable<PunterEntity> punters, string punterType)
        {
            var sportType = _soccer ? Consts.VSSoccer : Consts.VSSportbook;

            if (punters != null && punters.Any())
            {
                var info = $"List of { sportType } {punterType} updated({punters.Count()}): { string.Join(",", punters.Select(p => p.CustID))}";

                LogService.Info(info);
            }

            if (_model != null && _model.FailedUpdatePunterList.Any())
            {
                var error = $"List of { sportType } {punterType} update failed({_model.FailedUpdatePunterList.Count}) : {string.Join(",", _model.FailedUpdatePunterList.Select(p => p.CustID))}";

                LogService.Error(error);
            }
        }
    }
}

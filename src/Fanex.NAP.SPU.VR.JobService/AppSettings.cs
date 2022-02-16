namespace Fanex.NAP.SPU.VR.JobService
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.ComponentModel;

    public interface IAppSettings
    {
        int Environment { get; }

        string EnvironmentString { get; }

        int PunterChunkSize { get; }

        string SPName { get; }

        int MailingListIdAgentAjust { get; }

        int RetryTime { get; }

        string SentryDsn { get; }

        string Adjust3MExecutedDate { get; }

        int Adjust3MPunterChunkSize { get; }

        int Adjust3MNewVirtualMultiplierForSuperProPunter { get; }

        int Adjust3MNewVirtualMultiplierForWinning { get; }

        int Adjust3MVirtualRateThresholdForWinning { get; }

        string Adjust3MSiteIds { get; }

        string Adjust3MSuperIds { get; }

        string Adjust3MIP { get; }

        int Adjust3MDummyUser { get; }

        int Adjust3MMailingListId { get; }

        int MailingListIdToSPU { get; }

        int RetryTimesSapaSportDangerLevelForCheckingDataReady { get; }

        int RetryTimesSapaSportDangerLevelForUpdateDangerLevel { get; }

        bool IsTest { get; }

    }
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        private readonly string environment;

        private readonly int punterChunkSize;

        private readonly string spName;

        private readonly int mailingListIdAgentAjust;

        private readonly int retryTime;

        private readonly string sentryDsn;

        private readonly string adjust3MExecutedDate;

        private readonly int adjust3MPunterChunkSize;

        private readonly int adjust3MNewVirtualMultiplierForSuperProPunter;

        private readonly int adjust3MNewVirtualMultiplierForWinning;

        private readonly int adjust3MVirtualRateThresholdForWinning;

        private readonly string adjust3MSiteIds;

        private readonly string adjust3MSuperIds;

        private readonly string adjust3MIP;

        private readonly int adjust3MDummyUser;

        private readonly int adjust3MMailingListId;

        private readonly int mailingListIdToSPU;

        private readonly int retryTimesSapaSportDangerLevelForCheckingDataReady;

        private readonly int retryTimesSapaSportDangerLevelForUpdateDangerLevel;

        private readonly bool isTest;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            environment = GetValue<string>(nameof(Environment));

            punterChunkSize = GetValue<int>(nameof(PunterChunkSize));

            spName = GetValue<string>(nameof(SPName));

            mailingListIdAgentAjust = GetValue<int>(nameof(MailingListIdAgentAjust));

            retryTime = GetValue<int>(nameof(RetryTime));

            sentryDsn = GetValue<string>("Sentry:Dsn");

            adjust3MExecutedDate = GetValue<string>(nameof(Adjust3MExecutedDate));

            adjust3MPunterChunkSize = GetValue<int>(nameof(Adjust3MPunterChunkSize));

            adjust3MNewVirtualMultiplierForSuperProPunter = GetValue<int>(nameof(Adjust3MNewVirtualMultiplierForSuperProPunter));

            adjust3MNewVirtualMultiplierForWinning = GetValue<int>(nameof(Adjust3MNewVirtualMultiplierForWinning));

            adjust3MVirtualRateThresholdForWinning = GetValue<int>(nameof(Adjust3MVirtualRateThresholdForWinning));

            adjust3MSiteIds = GetValue<string>(nameof(Adjust3MSiteIds));

            adjust3MSuperIds = GetValue<string>(nameof(Adjust3MSuperIds));

            adjust3MIP = GetValue<string>(nameof(Adjust3MIP));

            adjust3MDummyUser = GetValue<int>(nameof(Adjust3MDummyUser));

            adjust3MMailingListId = GetValue<int>(nameof(Adjust3MMailingListId));

            mailingListIdToSPU = GetValue<int>(nameof(MailingListIdToSPU));

            retryTimesSapaSportDangerLevelForCheckingDataReady = GetValue<int>("SapaSportDangerLevel:RetryTimesForCheckingDataReady");

            retryTimesSapaSportDangerLevelForUpdateDangerLevel = GetValue<int>("SapaSportDangerLevel:RetryTimesForUpdateDangerLevel");

            isTest = GetValue<bool>(nameof(IsTest));
        }

        public int Environment
        {
            get
            {
                switch (environment)
                {
                    case "LOCAL":
                        return 1;
                    case "UAT":
                        return 2;
                    default:
                        return 3;
                }
            }
        }

        public string EnvironmentString
        {
            get
            {
                return environment;
            }
        }

        public string SPName => spName;

        public int PunterChunkSize => punterChunkSize;

        public int MailingListIdAgentAjust => mailingListIdAgentAjust;

        public int RetryTime => retryTime;

        public string SentryDsn => sentryDsn;

        public string Adjust3MExecutedDate => adjust3MExecutedDate;

        public int Adjust3MPunterChunkSize => adjust3MPunterChunkSize;

        public int Adjust3MNewVirtualMultiplierForSuperProPunter => adjust3MNewVirtualMultiplierForSuperProPunter;

        public int Adjust3MNewVirtualMultiplierForWinning => adjust3MNewVirtualMultiplierForWinning;

        public int Adjust3MVirtualRateThresholdForWinning => adjust3MVirtualRateThresholdForWinning;

        public string Adjust3MSiteIds => adjust3MSiteIds;

        public string Adjust3MSuperIds => adjust3MSuperIds;

        public string Adjust3MIP => adjust3MIP;

        public int Adjust3MDummyUser => adjust3MDummyUser;

        public int Adjust3MMailingListId => adjust3MMailingListId;

        public int MailingListIdToSPU => mailingListIdToSPU;

        public int RetryTimesSapaSportDangerLevelForCheckingDataReady => retryTimesSapaSportDangerLevelForCheckingDataReady;

        public int RetryTimesSapaSportDangerLevelForUpdateDangerLevel => retryTimesSapaSportDangerLevelForUpdateDangerLevel;

        public bool IsTest => isTest;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }
    }
}

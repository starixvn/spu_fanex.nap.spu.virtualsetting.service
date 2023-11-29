namespace Fanex.NAP.SPU.CTS.Shares
{
    using Fanex.NAP.SPU.Notification.Services;
    using Fanex.NAP.SPU.VR.Shares;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Utilities
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / partitionSize)
                        .Select(g => g.Select(x => x.item));
        }

        public static IEnumerable<IEnumerable<T>> ChunkList<T>(this IEnumerable<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("ChunkSize must be greater than 0.");
            }

            while (list.Any())
            {
                yield return list.Take(chunkSize);
                list = list.Skip(chunkSize);
            }
        }

        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string GenerateVirtualSettingXmlForUpdating(string param1, double param2)
        {
            var rate = string.Format(Consts.VSXmlForUpdating, param1, param2);

            return string.Format(Consts.VSXmlRoot, rate);
        }

        public static string GenerateVirtualSettingXmlFromRate(string param)
        {
            return string.Format(Consts.VSXmlRoot, param);
        }

        public static string GenerateRateVirtualSettingXmlForLogging(params object[] objParams)
        {
            return string.Format(Consts.VSXmlForLogging, objParams);
        }

        public static string GenerateRateVirtualSettingXmlForCustomerMarginHistory(params object[] objParams)
        {
            return string.Format(Consts.VSXmlForCustomerMarginHistory, objParams);
        }

        public static string GenerateXmlForVRRoot(string param)
        {
            return string.Format(Consts.VSXmlVRRoot, param);
        }

        public static string GenerateXmlForCustomerUpdFollowParent(params object[] objParams)
        {
            return string.Format(Consts.VSXmlForCustomerUpdFollowParent, objParams);
        }

        public static string GenerateXmlForAbnormalAgentUnflag(params object[] objParams)
        {
            return string.Format(Consts.VSXmlForAbnormalAgentUnflag, objParams);
        }

        public static string GenerateXmlForMemberBigStakeWhiteListUpd(params object[] objParams)
        {
            return string.Format(Consts.VSXmlForMemberBigStakeWhiteListUpd, objParams);
        }

        public static void WriteLog(string filename, string content)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            File.AppendAllText(filePath, string.Format("{0}{1}", content, System.Environment.NewLine));
        }

        public static void NotifyMessage(string message, string systemName, string environment, string groupId)
        {
            var telegramService = new NotificationService();
            string proxy = environment == "PRO" ? "lisasqd.nebula.prod:8801" : null;
            telegramService
               .Config("0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9", groupId, "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken", proxy)
               .Message(new MessageModel(systemName, DateTime.Now, environment, message))
               .SendWithoutDelay(isInfo: true);
        }
    }
}

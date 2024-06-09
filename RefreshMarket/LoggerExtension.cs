using BepInEx.Logging;
using System;

namespace RefreshMarket
{
    public static partial class LoggerExtension
    {
        public static void Infomation(this ManualLogSource logger, object message)
        {
            var now = DateTime.Now;
            logger.LogInfo($"[{now.ToString("HH:mm:ss.ffff")}] {message}");
        }
    }
}

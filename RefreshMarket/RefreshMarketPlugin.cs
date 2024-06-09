using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RefreshMarket
{
    
    [BepInPlugin("cn.kianakiferi.RefreshMarket", "刷新市场 Mod", version)]
    public class RefreshMarketPlugin : BaseUnityPlugin
    {
        private const string version = "0.0.1";
 
        private static ManualLogSource logger;

        void Awake()
        {
            logger = Logger;

            AddConfiguration();

            Harmony.CreateAndPatchAll(typeof(RefreshMarketPlugin));

            logger.Infomation($"刷新市场 Mod V{version} 已加载.");

            
        }

        void Update()
        {
            var key = RefreshMarketToogle.Value;
            if (key.IsDown())
            {
                shouldRefreshMarket = !shouldRefreshMarket;

                Logger.LogInfo($"刷新市场功能 {(shouldRefreshMarket ? "已开启" : "已关闭")}");
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Starbase), "UpdateStore")]
        public static bool UpdateStorePatch(Starbase __instance, float deltaTime)
        {
            if (shouldRefreshMarket)
            {
                var oldValue = Traverse.Create(__instance).Field("storeSpawnTimer").GetValue();

                Traverse.Create(__instance).Field("storeSpawnTimer").SetValue((float)0);

                logger.Infomation($"将补货时间剩余从 {oldValue} 设为 0");
            }

            
            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Starbase), "GetStoreCapacity")]
        public static void SetStarbaseStoreCapacityMultiplier(ref int __result)
        {
            var result = __result * StoreGoodsMultiplier.Value;
            logger.Infomation($"扩大市场容量 {__result} => {result}");

            __result = result;
        }


        #region Configuration
        private static ConfigEntry<int> StoreGoodsMultiplier;
        private static ConfigEntry<KeyboardShortcut> RefreshMarketToogle;
        private static bool shouldRefreshMarket = false;
        private KeyboardShortcut _toggleShortcut;
        private void AddConfiguration()
        {
            StoreGoodsMultiplier = Config.Bind("商品数量乘数", "Store Multiplier", 32, new ConfigDescription("Description", new AcceptableValueRange<int>(0, 100)));


            RefreshMarketToogle = Config.Bind("刷新市场热键", "RefreshMarketHotKey", new KeyboardShortcut(KeyCode.F8), "刷新市场热键"); 
        }

        #endregion
    }
}

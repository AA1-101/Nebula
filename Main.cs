using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Nebula.Modules;
using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace Nebula
{
    [BepInPlugin(PluginGuid, "Nebula", PluginVersion)]
    [BepInIncompatibility("jp.ykundesu.supernewroles")]
    [BepInIncompatibility("MalumMenu")]
    [BepInIncompatibility("com.crewmod.oficial")]
    [BepInIncompatibility("com.crewmod.showcase")]
    [BepInIncompatibility("xyz.crowdedmods.crowdedmod")]
    [BepInProcess("Among Us.exe")]
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class Main : BasePlugin
    {
        public const string PluginGuid = "com.aa-101.nebula";
        public const string PluginVersion = "0.0.1";
        public const string PluginDisplayVersion = "Alpha";
        public const bool IsTestBuild = true;
        public static ManualLogSource Logger;
        
        public override void Load()
        {
            Logger = Log;
            NebulaLogger.Init();
            NebulaLogger.StartLog();
            Log.LogInfo("Nebula Loaded!");
            Harmony harmony = new Harmony(Main.PluginGuid);
            harmony.PatchAll();
        }       

    }
}

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Nebula.Modules;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

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
        public const string PluginGuid = "github.com.AA1-10.Nebula";
        public const string PluginVersion = "0.0.1";
        public const string PluginDisplayVersion = "Alpha";
        public const bool IsTestBuild = true;

        public static Main Instance { get; private set; }

        public static ManualLogSource Logger;

        private Coroutines coroutines;

        public static bool IsChatCommand;

        public override void Load()
        {
            Instance = this;
            Logger = Log;           
            Log.LogInfo("Nebula Loaded!");
            ConfigurationManager.Load();
            CommandManager.LoadCommands();
            Harmony harmony = new Harmony(Main.PluginGuid);
            coroutines = AddComponent<Coroutines>();
            harmony.PatchAll();
            
        }
        public Coroutine StartCoroutine(Il2CppSystem.Collections.IEnumerator coroutine)
        {
            if (coroutine == null) return null;
            return coroutines.StartCoroutine(coroutine);
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            if (coroutine == null) return null;
            return coroutines.StartCoroutine(coroutine.WrapToIl2Cpp());
        }

        public void StopCoroutine(IEnumerator coroutine)
        {
            if (coroutine == null) return;
            coroutines.StopCoroutine(coroutine.WrapToIl2Cpp());
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null) return;
            coroutines.StopCoroutine(coroutine);
        }
        public void StopAllCoroutines()
        {
            coroutines.StopAllCoroutines();
        }
    }    
}
public class Coroutines : MonoBehaviour
{
}

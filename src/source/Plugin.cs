using BrilliantSkies.Modding;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace BMEffects_Remaster
{
    public class Plugin : GamePlugin_PostLoad
    {
        public static string guid = (string)JObject.Parse(File.ReadAllText(Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Asset Bundles"), "bmeffects_*.assetbundle")[0]))["ComponentId"]["Guid"];
        public string name { get { return "BMEffects_Remaster"; } }
        public Version version { get { return new Version(1, 0); } }
        public string ver = "1.0.0";

        public void OnLoad()
        {
            new Harmony("BMEffects_Remaster").PatchAll();
            ModProblems.AddModProblem($"{name} v{ver} active!", Assembly.GetExecutingAssembly().Location, string.Empty, false);
        }
        
        public void OnSave() { }
        public bool AfterAllPluginsLoaded() => true;
    }
}

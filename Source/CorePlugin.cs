using BrilliantSkies.Modding;
using HarmonyLib;
using System;

namespace BMEffects_Remaster
{
    public class CorePlugin : GamePlugin_PostLoad
    {
        public string name { get { return ModInfo.ModName; } }
        public Version version { get { return ModInfo.Version; } }

        public void OnLoad()
        {
            ModInfo.CheckVersion();
            new Harmony("BMEffects_Remaster").PatchAll();
        }
        
        public void OnSave() { }
        public bool AfterAllPluginsLoaded() => true;
    }
}

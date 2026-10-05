using BMEffects_Remaster.Mono;
using BMEffects_Remaster.Patches.VFX;
using BMEffects_Remaster.UI;
using BrilliantSkies.Effects.SoundSystem;
using BrilliantSkies.Modding.Types;
using BrilliantSkies.PlayerProfiles;
using HarmonyLib;
using MTMTVFX.Core;
using MTMTVFX.Core.AssetManagement;
using MTMTVFX.Effects.Muzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BMEffects_Remaster.Core
{
    [HarmonyPatch]
    public static class CorePatcher
    {
        public static event Action<Mode> OnModeSwap;
        private static bool _isInit = false;

        public static void SwapMode(Mode mode)
        {
            OnModeSwap.Invoke(mode);
        }

        [HarmonyPatch(typeof(MTMTVFX.Core.AssetManagement.AssetRegistry))]
        [HarmonyPatch("Init")]
        [HarmonyPostfix]
        public static void Init()
        {
            try
            {
                if (_isInit) return;
                else _isInit = true;

                Mode mode = ProfileManager.Instance.GetModule<BMEConfig>().mode;

                Dictionary<string, GameObject> assetDict = AssetLoader.GetAllAssets(new Guid(ModInfo.AssetbundleGUID));

                // pulse
                SoundRegistry.Register("pulse", BMEUtils.MakeClipDefinition(assetDict["pulse_sfx"].GetComponent<AudioSource>().clip));

                MaterialRegistry.Register("laser_pulse plain", assetDict["laser_pulse plain"].transform.Find("Beam").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse dark", assetDict["laser_pulse dark"].transform.Find("Beam").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse light", assetDict["laser_pulse light"].transform.Find("Beam").GetComponent<Renderer>().material);

                MaterialRegistry.Register("laser_pulse plain flash", assetDict["laser_pulse plain"].transform.Find("Flash").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse dark flash", assetDict["laser_pulse dark"].transform.Find("Flash").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse light flash", assetDict["laser_pulse light"].transform.Find("Flash").GetComponent<Renderer>().material);

                MaterialRegistry.Register("laser_pulse plain flash (1)", assetDict["laser_pulse plain"].transform.Find("Flash (1)").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse dark flash (1)", assetDict["laser_pulse dark"].transform.Find("Flash (1)").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_pulse light flash (1)", assetDict["laser_pulse light"].transform.Find("Flash (1)").GetComponent<Renderer>().material);

                // cont
                SoundRegistry.Register("wave_start", BMEUtils.MakeClipDefinition(assetDict["wave_sfx_start"].GetComponent<AudioSource>().clip));
                SoundRegistry.Register("wave_end", BMEUtils.MakeClipDefinition(assetDict["wave_sfx_end"].GetComponent<AudioSource>().clip));
                SoundRegistry.Register("wave", BMEUtils.MakeClipDefinition(assetDict["wave_sfx"].GetComponent<AudioSource>().clip));

                MaterialRegistry.Register("laser_cont plain", assetDict["laser_cont plain"].transform.Find("Core").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_cont dark", assetDict["laser_cont dark"].transform.Find("Core").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_cont light", assetDict["laser_cont light"].transform.Find("Core").GetComponent<Renderer>().material);

                MaterialRegistry.Register("laser_cont plain flash", assetDict["laser_cont plain"].transform.Find("Flash").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_cont dark flash", assetDict["laser_cont dark"].transform.Find("Flash").GetComponent<Renderer>().material);
                MaterialRegistry.Register("laser_cont light flash", assetDict["laser_cont light"].transform.Find("Flash").GetComponent<Renderer>().material);

                // pac
                SoundRegistry.Register("pac_med", BMEUtils.MakeClipDefinition(assetDict["pac_sfx med"].GetComponent<AudioSource>().clip));
                SoundRegistry.Register("pac_big", BMEUtils.MakeClipDefinition(assetDict["pac_sfx big"].GetComponent<AudioSource>().clip));

                // register the plain stuffs
                string defaultPulse = "";
                string defaultCont = "";
                switch (mode)
                {
                    case Mode.Plain:
                        defaultPulse = "laser_pulse plain";
                        defaultCont = "laser_cont plain";
                        break;
                    case Mode.Dark:
                        defaultPulse = "laser_pulse dark";
                        defaultCont = "laser_cont dark";
                        break;
                    default:
                        defaultPulse = "laser_pulse light";
                        defaultCont = "laser_cont light";
                        break;
                }
                assetDict["laser_pulse"] = assetDict[defaultPulse];
                assetDict["laser_cont"] = assetDict[defaultCont];
                AssetRegistry.Register(assetDict, 1, "BMEffects_Remaster");

                LaserVFXPatchContinuous.OnBeamDrawn += ContinuousLaserVFXPatch.DrawBeam;
                LaserVFXPatchPulse.OnPulseBeam += PulseLaserVFXPatch.DrawBeam;
            }
            catch (Exception e)
            {
                Utils.LogError(e.ToString(), BrilliantSkies.Core.Logger.LogOptions.PopupDev);
            }
        }

        [HarmonyPatch(typeof(MTMTVFX.Core.Utils))]
        [HarmonyPatch("AddScript")]
        [HarmonyPrefix]
        private static void AddScript(GameObject obj, Enum type, string modName)
        {
            if (modName == "BMEffects_Remaster")
            {
                if (type.GetType() == typeof(MuzzleFlashType) || type.GetType() == typeof(ExplosionType))
                {
                    List<GameObject> smokes = obj.GetComponentsInChildren<Transform>(true)
                                              .Where(t => t.name.Contains("Smoke"))
                                              .Select(t => t.gameObject)
                                              .ToList();
                    if (smokes != null && smokes.Count > 0)
                    {
                        foreach (GameObject smoke in smokes)
                        {
                            if (smoke.GetComponent<SmokeColorizer>() == null) smoke.AddComponent<SmokeColorizer>();
                        }
                    }
                }
                else if (type.GetType() == typeof(SpecialName) && (SpecialName)type == SpecialName.laser_cont)
                {
                    Utils.LogError($"{type} - {obj.name}");
                    if (obj.GetComponent<ContinuousBeamColorizer>() == null) obj.AddComponent<ContinuousBeamColorizer>();
                }
                else if (type.GetType() == typeof(BeamName))
                {
                    if ((BeamName)type == BeamName.laser_pulse)
                    {
                        Utils.LogError($"{type} - {obj.name}");
                        if (obj.GetComponent<PulseBeamColorizer>() == null) obj.AddComponent<PulseBeamColorizer>();
                    }
                    else if ((BeamName)type == BeamName.pac_beam)
                    {
                        if (obj.GetComponent<PacBeamUpdater>() == null) obj.AddComponent<PacBeamUpdater>();
                    }
                }
            }
        }
    }
}
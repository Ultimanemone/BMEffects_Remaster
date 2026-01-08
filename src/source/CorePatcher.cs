using BrilliantSkies.Effects.Pools.Lasers;
using BrilliantSkies.Effects.SoundSystem;
using BrilliantSkies.Effects.SpecialSounds;
using BrilliantSkies.Modding.Types;
using HarmonyLib;
using MTMTVFX.Core;
using MTMTVFX.Effects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static BrilliantSkies.Ftd.Avatar.Build.TurretIssueDisplayPacket;

namespace BMEffects_Remaster
{
    public enum Mode
    {
        Light = 0,
        Dark = 1,
        Plain = 2
    }

    public static class Constants
    {
        public static Mode mode = Mode.Dark;
    }

    [HarmonyPatch(typeof(AssetRegistry), "Init")]
    public class AssetRegistryPatch
    {
        public static AudioClipDefinition pulse;
        public static AudioClipDefinition wave_start;
        public static AudioClipDefinition wave_end;
        public static AudioClipDefinition wave;
        public static AudioClipDefinition pac_med;
        public static AudioClipDefinition pac_big;

        private static void Postfix(AssetRegistry __instance)
        {
            try
            {
                string dllPath = Assembly.GetExecutingAssembly().Location;
                string dllDir = Path.GetDirectoryName(dllPath);
                string configPath = Path.Combine(dllDir, "config.json");
                string json = File.ReadAllText(configPath);
                var obj = JObject.Parse(json);
                string mode = (string)obj["mode"];
                if (Enum.TryParse(typeof(Mode), mode, true, out object result)) Constants.mode = (Mode)result;

                Dictionary<string, GameObject> assetDict = AssetLoader.GetAllAssets(new Guid(Plugin.guid));

                if (Util.E_PULSE)
                {
                    pulse = BMEUtils.MakeClipDefinition(assetDict["pulse_sfx"].GetComponent<AudioSource>().clip);

                    if (Constants.mode == Mode.Plain) assetDict["laser_pulse"] = assetDict["laser_pulse plain"];
                    else if (Constants.mode == Mode.Dark) assetDict["laser_pulse"] = assetDict["laser_pulse dark"];
                    else assetDict["laser_pulse"] = assetDict["laser_pulse light"];
                }

                if (Util.E_CONTINUOUS)
                {
                    wave_start = BMEUtils.MakeClipDefinition(assetDict["wave_sfx_start"].GetComponent<AudioSource>().clip);
                    wave_end = BMEUtils.MakeClipDefinition(assetDict["wave_sfx_end"].GetComponent<AudioSource>().clip);
                    wave = BMEUtils.MakeClipDefinition(assetDict["wave_sfx"].GetComponent<AudioSource>().clip);

                    if (Constants.mode == Mode.Plain) assetDict["laser_cont"] = assetDict["laser_cont plain"];
                    else if (Constants.mode == Mode.Dark) assetDict["laser_cont"] = assetDict["laser_cont dark"];
                    else assetDict["laser_cont"] = assetDict["laser_cont light"];
                }

                if (Util.E_PAC)
                {
                    pac_med = BMEUtils.MakeClipDefinition(assetDict["pac_sfx med"].GetComponent<AudioSource>().clip);
                    pac_big= BMEUtils.MakeClipDefinition(assetDict["pac_sfx big"].GetComponent<AudioSource>().clip);
                }

                AssetRegistry.Register(assetDict, 1, "BMEffects_Remaster");
            }
            catch (Exception e)
            {
                Util.LogError<AssetRegistryPatch>(e.Message);
                Util.LogError<AssetRegistryPatch>(e.Message, BrilliantSkies.Core.Logger.LogOptions.PopupDev);
            }
        }
    }

    [HarmonyPatch(typeof(Util), "AddScript")]
    public class AddScriptPatch
    {
        private static void Prefix(GameObject obj, Enum type, string modName)
        {
            if (modName == "BMEffects_Remaster")
            {
                if (type.GetType() == typeof(MuzzleFlashName) || type.GetType() == typeof(ExplosionName))
                {
                    List<GameObject> smokes = obj.GetComponentsInChildren<Transform>(true)
                                              .Where(t => t.name.Contains("Smoke"))
                                              .Select(t => t.gameObject)
                                              .ToList();
                    if (smokes != null && smokes.Count > 0)
                    {
                        foreach (GameObject smoke in smokes)
                        {
                            if (smoke.GetComponent<SmokeColorer>() == null) smoke.AddComponent<SmokeColorer>();
                        }
                    }
                }
                else if ((SpecialName)type == SpecialName.laser_cont)
                {
                    if (obj.GetComponent<ContinuousBeamColorizer>() == null) obj.AddComponent<ContinuousBeamColorizer>();
                }
                else if ((BeamName)type == BeamName.laser_pulse)
                {
                    if (obj.GetComponent<PulseBeamColorizer>() == null) obj.AddComponent<PulseBeamColorizer>();
                }
                else if ((BeamName)type == BeamName.pac_beam)
                {
                    if (obj.GetComponent<PacBeamer>() == null) obj.AddComponent<PacBeamer>();
                }
            }
        }
    }
    public class BMEUtils
    {
        private static AnimationCurve _dissolveCurve;
        public static AnimationCurve dissolveCurve
        {
            get
            {
                if (_dissolveCurve == null)
                {
                    _dissolveCurve = new AnimationCurve(
                        new Keyframe(0f, 1f) { inTangent = 0f, outTangent = 0f, inWeight = 0f, outWeight = 0f, weightedMode = WeightedMode.None },
                        new Keyframe(0.629146338f, 0.6056918f) { inTangent = -2.14894271f, outTangent = -2.14894271f, inWeight = 0.333333343f, outWeight = 0.0978196338f, weightedMode = WeightedMode.None },
                        new Keyframe(1f, 0f) { inTangent = 0f, outTangent = 0f, inWeight = 0f, outWeight = 0.185950562f, weightedMode = WeightedMode.None }
                    );
                    _dissolveCurve.preWrapMode = WrapMode.ClampForever;
                    _dissolveCurve.postWrapMode = WrapMode.ClampForever;
                }
                return _dissolveCurve;
            }
        }

        public static AudioClipDefinition MakeClipDefinition(AudioClip clip)
        {
            if (clip == null) return null;

            AudioClipDefinition acd = new AudioClipDefinition();
            var prop = AccessTools.Property(typeof(AudioClipDefinition), "AudioClip");
            prop.SetValue(acd, clip);

            return acd;
        }

        public static void PlaySound(AudioClipDefinition clipDefinition, Vector3 pos, float volume = 1f, float minPitch = 1f, float maxPitch = -1f, float minDistance = 6f)
        {
            if (clipDefinition == null) return;

            float pitch;
            if (maxPitch > 0 && minPitch < maxPitch)
            {
                pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            }
            else pitch = minPitch;

            BrilliantSkies.Core.Pooling.Pooler.GetPool<AdvSoundManager>().PlaySound(new SoundRequest(clipDefinition, pos)
            {
                Priority = SoundPriority.ShouldHear,
                Pitch = pitch,
                MinDistance = minDistance,
                Volume = volume
            });
        }

        public static void SetGradient(ref Gradient gradient, Color color)
        {
            GradientAlphaKey[] gak = { new GradientAlphaKey(color.a, 0f) };
            GradientColorKey[] gck = { new GradientColorKey(color, 0f) };
            gradient.SetKeys(gck, gak);
        }
    }
}

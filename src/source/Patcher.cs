using BrilliantSkies.Effects.Pools.Lasers;
using BrilliantSkies.Effects.SoundSystem;
using BrilliantSkies.Effects.SpecialSounds;
using BrilliantSkies.Modding.Types;
using HarmonyLib;
using MTMTVFX.Core;
using MTMTVFX.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BMEffects_Remaster
{
    [HarmonyPatch(typeof(AssetRegistry), "Init")]
    public class AssetRegistryPatch
    {
        public static AudioClip pulse;
        public static AudioClip wave;

        private static void Postfix(AssetRegistry __instance)
        {
            Dictionary<string, GameObject> assetDict = AssetLoader.GetAllAssets(new Guid(Plugin.guid));

            if (Util.E_PULSE)
            {
                pulse = assetDict["pulse_sfx"].GetComponent<AudioSource>().clip;
                if (pulse == null) Util.LogError<AssetRegistryPatch>("Pulse laser SFX not found");
            }

            if (Util.E_CONTINUOUS)
            {
                wave = assetDict["wave_sfx"].GetComponent<AudioSource>().clip;
                if (wave == null) Util.LogError<AssetRegistryPatch>("Continuous laser SFX not found");
            }

            AssetRegistry.Register(assetDict, 1, "BMEffects_Remaster");
        }
    }

    [HarmonyPatch(typeof(VFXManager), "AddScriptConditional")]
    public class VFXManagerPatch
    {
        private static void Prefix(VFXManager __instance, GameObject obj, string objName, string modName)
        {
            if (modName == "BMEffects_Remaster")
            {
                string objType = objName.Split('_')[0];
                if (objType == "muzzleflash" || objType == "expl")
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

                if (objName == "laser_cont")
                {
                    if (obj.GetComponent<ContinuousBeamColorizer>() == null) obj.AddComponent<ContinuousBeamColorizer>();
                }
                else if (objName == "laser_pulse")
                {
                    if (obj.GetComponent<PulseBeamColorizer>() == null) obj.AddComponent<PulseBeamColorizer>();
                }
            }
        }
    }


    [HarmonyPatch(typeof(LaserPatchMod), "PulseMethod")]
    public class PulseLaserPatch2
    {
        public static void Prefix(LaserPulseSpecification spec, GameObject obj)
        {
            if (!Util.E_PULSE) return;

            PulseBeamColorizer colorizer = obj.GetComponent<PulseBeamColorizer>();
            colorizer.Fire(spec.Color, spec.StartPosition, spec.EndPosition, spec.StartingWidth);
        }
    }

    [HarmonyPatch(typeof(ConventionalLaser), "PulseSound")]
    public class PulseLaserSoundPatch
    {
        public static bool Prefix(ConventionalLaser __instance)
        {
            if (!Util.E_PULSE) return true;

            if (AssetRegistryPatch.pulse == null) return true;

            AudioClipDefinition pulseACD = new AudioClipDefinition();
            var prop = AccessTools.Property(typeof(AudioClipDefinition), "AudioClip");
            prop.SetValue(pulseACD, AssetRegistryPatch.pulse);

            BrilliantSkies.Core.Pooling.Pooler.GetPool<AdvSoundManager>().PlaySound(new SoundRequest(pulseACD, __instance.GameWorldPosition)
            {
                Priority = SoundPriority.ShouldHear,
                Pitch = UnityEngine.Random.Range(0.9f, 1.1f),
                MinDistance = 6f,
                Volume = 0.6f
            });

            return false;
        }
    }


    [HarmonyPatch(typeof(LaserPatchMod), "ContMethod")]
    public class ContLaserPatch
    {
        public static void Prefix(Vector3 start, Vector3 end, Vector3 direction, float width, Color color, GameObject obj)
        {
            if (!Util.E_CONTINUOUS) return;

            ContinuousBeamColorizer colorizer = obj.GetComponent<ContinuousBeamColorizer>();
            colorizer.Fire(color, start, end, direction, width);
        }
    }

    [HarmonyPatch(typeof(SpecialSound), "NoiseHere")]
    public static class ContLaserSoundPatch
    {
        private static void Prefix(SpecialSound __instance)
        {
            if (!Util.E_CONTINUOUS) return;

            if (AssetRegistryPatch.wave != null && __instance is LaserSound)
            {
                var field = AccessTools.Field(typeof(SpecialSound), "OurAudioSource");
                AudioSource source = ((AudioSource)field.GetValue(__instance));
                if (source.clip != AssetRegistryPatch.wave)
                {
                    source.spatialBlend = 1f;
                    source.maxDistance = 200f;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    __instance.SetNewClip(AssetRegistryPatch.wave);
                }
            }
        }
    }
}

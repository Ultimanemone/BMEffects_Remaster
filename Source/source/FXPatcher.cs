using BrilliantSkies.Effects.Pools.Lasers;
using BrilliantSkies.Effects.SoundSystem;
using BrilliantSkies.Effects.SpecialSounds;
using BrilliantSkies.Modding.Types;
using HarmonyLib;
using MTMTVFX.Core;
using MTMTVFX.Effects;
using UnityEngine;

namespace BMEffects_Remaster
{
    [HarmonyPatch(typeof(LaserPatchMod), "PulseMethod")]
    public class PulseLaserPatch2
    {
        private static void Prefix(LaserPulseSpecification spec, GameObject obj)
        {
            if (!Config.E_PULSE) return;

            obj.GetComponent<PulseBeamColorizer>()?.Fire(spec.Color, spec.StartPosition, spec.EndPosition, spec.StartingWidth);
        }
    }

    [HarmonyPatch(typeof(ConventionalLaser), "PulseSound")]
    public class PulseLaserSoundPatch
    {
        private static bool Prefix(ConventionalLaser __instance)
        {
            if (!Config.E_PULSE) return true;

            if (AssetRegistryPatch.pulse == null) return true;
            BMEUtils.PlaySound(AssetRegistryPatch.pulse, __instance.GameWorldPosition, 0.6f, 0.9f, 1.1f);

            return false;
        }
    }


    [HarmonyPatch(typeof(LaserPatchMod), "ContMethod")]
    public class ContLaserPatch
    {
        private static void Prefix(Vector3 start, Vector3 end, Vector3 direction, float width, Color color, GameObject obj)
        {
            if (!Config.E_CONTINUOUS) return;

            ContinuousBeamColorizer colorizer = obj.GetComponent<ContinuousBeamColorizer>();
            colorizer.Fire(color, start, end, direction, width);
        }
    }

    [HarmonyPatch(typeof(SpecialSound), "NoiseHere")]
    public static class ContLaserSoundPatch
    {
        private static void Prefix(SpecialSound __instance)
        {
            if (!Config.E_CONTINUOUS) return;

            if (AssetRegistryPatch.wave != null && __instance is LaserSound)
            {
                var field = AccessTools.Field(typeof(SpecialSound), "OurAudioSource");
                AudioSource source = ((AudioSource)field.GetValue(__instance));
                if (source.clip != AssetRegistryPatch.wave.AudioClip)
                {
                    source.spatialBlend = 1f;
                    source.maxDistance = 200f;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    __instance.SetNewClip(AssetRegistryPatch.wave.AudioClip);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PacPatchMod), "PacMethod")]
    public static class PACPatch
    {
        private static void Prefix(Vector3[] pointArray, GameObject pacBeam, float damage, ParticleType type, Color color)
        {
            pacBeam.GetComponent<PacBeamer>()?.Fire(pointArray, damage, type, color);
        }
    }

    [HarmonyPatch(typeof(ParticleCannon), "ClientAndServerFireNoise")]
    public static class PACSoundPatch
    {
        private static bool Prefix(ParticleCannon __instance, float energyDispensed)
        {
            if (energyDispensed < 400000f)
            {
                return true;
            }

            if (energyDispensed > 1500000f)
            {
                BMEUtils.PlaySound(AssetRegistryPatch.pac_big, __instance.GameWorldPosition, 1.3f, 1f, -1f, 500f);
            }
            else BMEUtils.PlaySound(AssetRegistryPatch.pac_med, __instance.GameWorldPosition, 1.3f, 1f, -1f, 200f);
            return false;
        }
    }
}

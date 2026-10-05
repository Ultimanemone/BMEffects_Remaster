using BMEffects_Remaster.Core;
using BMEffects_Remaster.UI;
using BrilliantSkies.Effects.SpecialSounds;
using BrilliantSkies.Modding.Types;
using BrilliantSkies.PlayerProfiles;
using HarmonyLib;
using MTMTVFX.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Patches.SFX
{
    [HarmonyPatch]
    public class ContinuousLaserSFXPatch
    {
        public static void PlayStart(Vector3 pos)
        {
            if (WeCanPlay())
            {
                SoundRegistry.TryGetACD("wave_start", out AudioClipDefinition acd);
                BMEUtils.PlaySound(acd, pos, 1.5f);
            }
        }

        public static void PlayEnd(Vector3 pos)
        {
            if (WeCanPlay())
            {
                SoundRegistry.TryGetACD("wave_end", out AudioClipDefinition acd);
                BMEUtils.PlaySound(acd, pos, 1.5f);
            }
        }

        [HarmonyPatch(typeof(SpecialSound))]
        [HarmonyPatch("NoiseHere")]
        [HarmonyPrefix]
        private static void PatchSound(SpecialSound __instance)
        {
            if (WeCanPlay())
            {
                if (SoundRegistry.TryGetACD("wave", out AudioClipDefinition acd) && __instance is LaserSound)
                {
                    var field = AccessTools.Field(typeof(SpecialSound), "OurAudioSource");
                    AudioSource source = (AudioSource)field.GetValue(__instance);
                    if (source.clip != acd.AudioClip)
                    {
                        source.spatialBlend = 1f;
                        source.maxDistance = 200f;
                        source.rolloffMode = AudioRolloffMode.Logarithmic;
                        __instance.SetNewClip(acd.AudioClip);
                    }
                }
            }
        }

        private static bool WeCanPlay()
        {
            BMEConfig config = ProfileManager.Instance.GetModule<BMEConfig>();
            return config.e_laserSound;
        }
    }
}

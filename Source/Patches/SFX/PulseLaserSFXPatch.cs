using BMEffects_Remaster.Core;
using BMEffects_Remaster.UI;
using BrilliantSkies.Modding.Types;
using BrilliantSkies.PlayerProfiles;
using HarmonyLib;
using MTMTVFX.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace BMEffects_Remaster.Patches.SFX
{
    [HarmonyPatch(typeof(ConventionalLaser), "PulseSound")]
    public class PulseLaserSoundPatch
    {
        private static bool Prefix(ConventionalLaser __instance)
        {
            BMEConfig config = ProfileManager.Instance.GetModule<BMEConfig>();
            if (config.e_pulseSound)
            {
                if (SoundRegistry.TryGetACD("pulse", out AudioClipDefinition acd))
                {
                    BMEUtils.PlaySound(acd, __instance.GameWorldPosition, 0.6f, 0.9f, 1.1f);
                }
                return false;
            }
            return true;
        }
    }
}

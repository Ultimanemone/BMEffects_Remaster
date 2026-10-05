using BMEffects_Remaster.Core;
using BMEffects_Remaster.UI;
using BrilliantSkies.Modding.Types;
using BrilliantSkies.PlayerProfiles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BMEffects_Remaster.Patches.SFX
{
    [HarmonyPatch(typeof(ParticleCannon), "ClientAndServerFireNoise")]
    public class PACSFXPatch
    {
        private static bool Prefix(ParticleCannon __instance, float energyDispensed)
        {
            BMEConfig config = ProfileManager.Instance.GetModule<BMEConfig>();
            if (config.e_pacSound && energyDispensed > 400000f)
            {
                if (energyDispensed > 1500000f)
                {
                    if (SoundRegistry.TryGetACD("pac_big", out AudioClipDefinition acd))
                    {
                        BMEUtils.PlaySound(acd, __instance.GameWorldPosition, 1.3f, 1f, -1f, 500f);
                    }
                }
                else
                {
                    if (SoundRegistry.TryGetACD("pac_med", out AudioClipDefinition acd))
                    {
                        BMEUtils.PlaySound(acd, __instance.GameWorldPosition, 1.3f, 1f, -1f, 200f);
                    }
                }
                return false;
            }
            return true;
        }
    }
}

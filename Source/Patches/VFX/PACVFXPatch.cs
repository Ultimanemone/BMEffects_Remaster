using BMEffects_Remaster.Mono;
using HarmonyLib;
using MTMTVFX.Effects.Muzzle;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Patches.VFX
{
    [HarmonyPatch(typeof(PacPatchMod), "PacMethod")]
    public class PACVFXPAtch
    {
        private static void Prefix(Vector3[] pointArray, GameObject pacBeam, float damage, ParticleType type, Color color)
        {
            pacBeam.GetComponent<PacBeamUpdater>()?.Fire(pointArray, damage, type, color);
        }
    }
}

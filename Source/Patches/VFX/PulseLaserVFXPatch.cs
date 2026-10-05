using BMEffects_Remaster.Mono;
using BrilliantSkies.Effects.Pools.Lasers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Patches.VFX
{
    public class PulseLaserVFXPatch
    {
        public static void DrawBeam(LaserPulseSpecification spec, GameObject beam)
        {
            beam.GetComponent<PulseBeamColorizer>()?.Fire(spec.Color, spec.StartPosition, spec.EndPosition, spec.StartingWidth);
        }
    }
}

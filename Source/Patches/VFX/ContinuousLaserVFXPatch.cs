using BMEffects_Remaster.Mono;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Patches.VFX
{
    public class ContinuousLaserVFXPatch
    {
        public static void DrawBeam(Vector3 start, Vector3 end, Vector3 direction, float width, Color color, GameObject beam)
        {
            beam.GetComponent<ContinuousBeamColorizer>()?.Fire(color, start, end, direction, width);
        }
    }
}

using BMEffects_Remaster.UI;
using BrilliantSkies.Effects.SoundSystem;
using BrilliantSkies.Modding.Types;
using BrilliantSkies.PlayerProfiles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Core
{
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

        public static string GetLaserModeName(bool pulse = true)
        {
            BMEConfig config = ProfileManager.Instance.GetModule<BMEConfig>();
            return (pulse ? "laser_pulse " : "laser_cont ") + config.mode.ToString().ToLower();
        }
    }
}

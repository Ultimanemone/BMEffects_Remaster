using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BMEffects_Remaster
{
    public class PacBeamer : MonoBehaviour
    {
        private float lifetime = 2f;
        private float width = 9f;
        private float glowWidth = 5f;
        private AnimationCurve widthCurve;
        private AnimationCurve glowCurve;
        private Gradient beamGradient;
        private Gradient glowGradient;
        private LineRenderer beam;
        private LineRenderer glow;
        private ParticleSystem flash;
        private List<ParticleSystem> shockwave;
        private Color color = Color.white;
        private MaterialPropertyBlock mpb;
        float counter = 0f;

        void Awake()
        {
            LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();
            shockwave = new List<ParticleSystem>();
            ParticleSystem[] psList = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in psList)
            {
                if (ps.name == "Flash") flash = ps;
                else if (ps.name.Contains("Shockwave"))
                {
                    shockwave.Add(ps);
                }
            }

            mpb = new MaterialPropertyBlock();
            beam = lines.First(x => x.name == "Beam");
            glow = lines.First(x => x.name == "Glow");
            beamGradient = new Gradient();
            glowGradient = new Gradient();

            GradientAlphaKey gak0 = new GradientAlphaKey(1f, 0.10f);
            GradientAlphaKey gak1 = new GradientAlphaKey(0f, 0.5f);
            GradientColorKey gck = new GradientColorKey(color, 0f);
            GradientAlphaKey[] gakList = { gak0, gak1 };
            GradientColorKey[] gckList = { gck, gck };
            glowGradient.SetKeys(gckList, gakList);

            glowCurve = new AnimationCurve(
                new Keyframe(0f, 0.5f) { inTangent = 0.5f, outTangent = 0.5f, inWeight = 0f, outWeight = 0.333333343f, weightedMode = WeightedMode.None },
                new Keyframe(1f, 1f) { inTangent = 0.5f, outTangent = 0.5f, inWeight = 0.333333343f, outWeight = 0f, weightedMode = WeightedMode.None }
            );
            glowCurve.preWrapMode = WrapMode.ClampForever;
            glowCurve.postWrapMode = WrapMode.ClampForever;
        }

        void Update()
        {
            counter = Mathf.Max(0, counter - Time.deltaTime);
            float t = 1 - counter / lifetime;

            mpb.SetFloat("_Dissolve", BMEUtilss.dissolveCurve.Evaluate(t));
            beam.SetPropertyBlock(mpb);
            beam.widthMultiplier = widthCurve.Evaluate(t) * width;
            Color c1 = beamGradient.Evaluate(t);
            beam.startColor = c1;
            beam.endColor = c1;

            glow.widthMultiplier = glowCurve.Evaluate(t) * glowWidth;
            Color c2 = glowGradient.Evaluate(t);
            glow.startColor = c2;
            c2.a = 0f;
            glow.endColor = c2;
        }

        public void Fire(Vector3[] pointArr, float damage, ParticleType type, Color color)
        {
            this.color = color;

            float mult = 0.5f;
            switch (type)
            {
                case ParticleType.Impact:
                    mult = Mathf.Pow(damage / 961f, 0.405f);
                    break;
                case ParticleType.Emp:
                    mult = Mathf.Pow(damage / 270f, 0.405f);
                    break;
                case ParticleType.Piercing:
                    mult = Mathf.Pow(damage / 240f, 0.405f);
                    break;
                case ParticleType.Explosive:
                    mult = Mathf.Pow(damage / 525f, 0.405f);
                    break;
            }
            lifetime = Mathf.Clamp(mult, 0.5f, 12f);
            counter = lifetime;
            width = Mathf.Min(20f, lifetime * 10f);
            glowWidth = width / 2f;

            widthCurve = new AnimationCurve(
                new Keyframe(0f, 1f) { inTangent = -10.6769314f, outTangent = -10.6769314f, inWeight = 0.333333343f, outWeight = 0.333333343f, weightedMode = WeightedMode.None },
                new Keyframe(0.06222067f, 0.3356742f) { inTangent = -2.69562864f, outTangent = -2.69562864f, inWeight = 0.5417401f, outWeight = 0.616004467f, weightedMode = WeightedMode.None },
                new Keyframe(0.269454956f, 0.0699489042f) { inTangent = -0.251315922f, outTangent = -0.251315922f, inWeight = 0.574416161f, outWeight = 0.124610864f, weightedMode = WeightedMode.None },
                new Keyframe(1f, 0.2f / width) { inTangent = 0f, outTangent = 0f, inWeight = 0f, outWeight = 0f, weightedMode = WeightedMode.None }
            );
            widthCurve.preWrapMode = WrapMode.ClampForever;
            widthCurve.postWrapMode = WrapMode.ClampForever;

            if (flash != null)
            {
                var main = flash.main;
                main.startColor = color;
                main.startLifetime = lifetime / 1.5f;
                main.startSize = width * 4f;
            }

            List<ParticleSystem> exclude = new List<ParticleSystem>();
            if (lifetime > 2.5f)
            {
                foreach (ParticleSystem ps in shockwave)
                {
                    if (ps.name == "Shockwave")
                    {
                        exclude.Add(ps);
                        var main = ps.main;
                        main.startLifetime = lifetime - 2f;
                        main.startSize = lifetime - 1f;
                    }

                    if (lifetime > 4f && ps.name == "Shockwave (1)")
                    {
                        exclude.Add(ps);
                        var main = ps.main;
                        main.startLifetime = lifetime - 2f;
                        main.startSize = lifetime - 3f;
                    }

                    if (lifetime > 5f && ps.name == "Shockwave (2)")
                    {
                        exclude.Add(ps);
                        var main = ps.main;
                        main.startLifetime = lifetime - 2f;
                        main.startSize = lifetime - 2f;
                    }
                }
            }

            foreach (ParticleSystem ps in shockwave)
            {
                if (exclude.Contains(ps)) continue;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            Color c = color;
            if (lifetime > 6f)
            {
                if (color.maxColorComponent >= 0.5f) c = Color.black;
                else c = Color.white;
            }

            GradientAlphaKey[] gakList = {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };
            GradientColorKey[] gckList = {
                new GradientColorKey(c, 0.07f / lifetime),
                new GradientColorKey(color, 0.08f / lifetime)
            };
            beamGradient.SetKeys(gckList, gakList);

            mpb.SetFloat("_Seed", UnityEngine.Random.Range(-500f, 500f));
            beam.SetPropertyBlock(mpb);
            beam.widthMultiplier = width;
            beam.positionCount = pointArr.Length;
            beam.SetPositions(pointArr);

            glow.widthMultiplier = glowWidth;
            int glowPosCount = Mathf.Min(6, pointArr.Length);
            glow.positionCount = glowPosCount;
            Vector3[] glowPos = new Vector3[glowPosCount];
            Array.Copy(pointArr, glowPos, glowPosCount);
            glow.SetPositions(glowPos);
        }
    }
}

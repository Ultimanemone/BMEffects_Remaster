using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BrilliantSkies.Environments;

namespace BMEffects_Remaster
{
    internal class SmokeColorer : MonoBehaviour
    {
        private Material mat;
        private Color color1;
        private Color color2;

        private void Awake()
        {
            mat = GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material;
            color1 = mat.GetColor("_Color1");
            color2 = mat.GetColor("_Color2");
        }

        private void Update()
        {
            if (mat != null)
            {
                float timeMult = Mathf.Abs((FtdEnvironmentManager.Instance.TimeOfDay + 12f) % 24f - 12f) / 12f * 0.85f + 0.15f;
                //Color ambientColor = FtdEnvironmentManager.Instance.CurrentWeather.AmbientLightColor * FtdEnvironmentManager.Instance.CurrentWeather.AmbientLightIntensity;
                //Color sunshaftColor = FtdEnvironmentManager.Instance.CurrentWeather.SunShaftColor * FtdEnvironmentManager.Instance.CurrentWeather.SunShaftIntensity;
                mat.SetColor("_Color1", color1 * timeMult);
                mat.SetColor("_Color2", color2 * timeMult);
            }
        }
    }
}

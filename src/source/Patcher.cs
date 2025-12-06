using HarmonyLib;
using MTMTVFX.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster
{
    [HarmonyPatch(typeof(AssetRegistry), "Init")]
    public class AssetRegistryPatch
    {
        private static void Postfix(AssetRegistry __instance)
        {
            AssetRegistry.Register(AssetLoader.GetAllAssets(new Guid(Plugin.guid)), 1, "BMEffects_Remaster");
        }
    }

    [HarmonyPatch(typeof(VFXManager), "AddScriptConditional")]
    public class VFXManagerPatch
    {
        private static void Prefix(VFXManager __instance, GameObject obj, string objName, string modName)
        {
            if (modName == "BMEffects_Remaster")
            {
                string objType = objName.Split('_')[0];
                if (objType == "muzzleflash" || objType == "expl")
                {
                    List<GameObject> smokes = obj.GetComponentsInChildren<Transform>(true)
                                              .Where(t => t.name.Contains("Smoke"))
                                              .Select(t => t.gameObject)
                                              .ToList();
                    if (smokes != null && smokes.Count > 0)
                    {
                        foreach (GameObject smoke in smokes)
                        {
                            smoke.AddComponent<SmokeColorer>();
                        }
                    }
                }
            }
        }
    }
}

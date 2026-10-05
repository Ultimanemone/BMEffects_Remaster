using BrilliantSkies.Modding.Types;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Core
{
    public static class SoundRegistry
    {
        private static Dictionary<string, AudioClipDefinition> _registry = new Dictionary<string, AudioClipDefinition>();

        public static void Register(string name, AudioClipDefinition def)
        {
            _registry[name] = def;
        }

        public static bool TryGetACD(string name, out AudioClipDefinition def)
        {
            return _registry.TryGetValue(name, out def);
        }
    }
}

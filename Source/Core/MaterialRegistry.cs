using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BMEffects_Remaster.Core
{
    public static class MaterialRegistry
    {
        private static Dictionary<string, Material> _registry = new Dictionary<string, Material>();

        public static void Register(string name, Material mat)
        {
            _registry[name] = mat;
        }

        public static bool TryGetMat(string name, out Material mat)
        {
            return _registry.TryGetValue(name, out mat);
        }
    }
}

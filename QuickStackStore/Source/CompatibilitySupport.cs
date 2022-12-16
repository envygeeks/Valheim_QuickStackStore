﻿using BepInEx;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuickStackStore
{
    // TODO OdinsQOL's quick slots, aedenthorns equipment slots, ComfyQuickSlots
    public static class CompatibilitySupport
    {
        private static MethodInfo IsEquipmentSlot;
        private static MethodInfo IsQuickSlot;
        public static Dictionary<string, bool> cache = new Dictionary<string, bool>();

        public static bool HasPlugin(string guid)
        {
            if (cache.ContainsKey(guid))
            {
                return cache[guid];
            }

            var plugins = UnityEngine.Object.FindObjectsOfType<BaseUnityPlugin>();

            cache[guid] = plugins.Any(plugin => plugin.Info.Metadata.GUID == guid);
            return cache[guid];
        }

        public static bool IsEquipOrQuickSlot(Vector2i itemPos)
        {
            if (HasPlugin("randyknapp.mods.equipmentandquickslots"))
            {
                //Plugin.instance.GetLogger().LogDebug("Found EquipmentAndQuickSlots plugin");
                if (IsEquipmentSlot == null && IsQuickSlot == null)
                {
                    var ass = Assembly.Load("EquipmentAndQuickSlots");

                    if (ass != null)
                    {
                        //Plugin.instance.GetLogger().LogDebug("Found assembly");
                        var type = ass.GetTypes().First(a => a.IsClass && a.Name == "EquipmentAndQuickSlots");
                        var pubstatic = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                        IsEquipmentSlot = pubstatic.First(t => t.Name == "IsEquipmentSlot" && t.GetParameters().Length == 1);
                        //Plugin.instance.GetLogger().LogDebug($"IsEquipmentSlot: {IsEquipmentSlot}");
                        IsQuickSlot = pubstatic.First(t => t.Name == "IsQuickSlot" && t.GetParameters().Length == 1);
                        //Plugin.instance.GetLogger().LogDebug($"IsQuickSlot: {IsQuickSlot}");
                    }
                }

                if ((bool)IsEquipmentSlot?.Invoke(null, new object[] { itemPos }))
                {
                    return true;
                }

                if ((bool)IsQuickSlot?.Invoke(null, new object[] { itemPos }))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
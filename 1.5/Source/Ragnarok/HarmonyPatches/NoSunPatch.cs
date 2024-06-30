using System.Collections;
using System.Numerics;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Vector2 = UnityEngine.Vector2;

namespace Ragnarok.HarmonyPatches;

[HarmonyPatch(typeof(WorldLayer_Sun), nameof(WorldLayer_Sun.Regenerate))]
public class NoSunPatch
{
    [HarmonyPostfix]
    public static IEnumerable Postfix(IEnumerable __result)
    {
        if (RagnarokMod.settings.removeSun) yield break;
        foreach (object item in __result)
        {
            yield return item;
        }
    }
}

[HarmonyPatch(typeof(WorldLayer_Glow), nameof(WorldLayer_Glow.Regenerate))]
public class NoGlowPatch
{
    [HarmonyPostfix]
    public static IEnumerable Postfix(IEnumerable __result)
    {
        if (RagnarokMod.settings.removeSun) yield break;
        foreach (object item in __result)
        {
            yield return item;
        }
    }
}

[HarmonyPatch(typeof(GenCelestial), nameof(GenCelestial.CelestialSunGlow), [typeof(int), typeof(int)])]
public class CelestialSunGlowPatch
{
    [HarmonyPostfix]
    public static float Postfix(float __result)
    {
        return RagnarokMod.settings.removeSun ? 0f : __result;
    }
}

[HarmonyPatch(typeof(GenCelestial), nameof(GenCelestial.IsDaytime))]
public class IsDaytimePatch
{
    [HarmonyPostfix]
    public static bool Postfix(bool __result)
    {
        return !RagnarokMod.settings.removeSun && __result;
    }
}

[HarmonyPatch(typeof(GenCelestial), nameof(GenCelestial.GetLightSourceInfo))]
public class GetLightSourceInfoPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref GenCelestial.LightInfo __result)
    {
        if (RagnarokMod.settings.removeSun) return;
        __result = new GenCelestial.LightInfo()
        {
            vector = new Vector2(0, 0),
            intensity = 0f
        };
    }
}

[HarmonyPatch(typeof(GenCelestial), "CelestialSunGlowPercent", [typeof(float), typeof(int), typeof(float)])]
public class CelestialSunGlowPercentPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref float __result)
    {
        if (RagnarokMod.settings.removeSun) return;
        __result = 0f;
    }
}

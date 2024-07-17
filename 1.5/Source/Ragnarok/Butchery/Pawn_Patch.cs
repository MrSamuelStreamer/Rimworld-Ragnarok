using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok.Butchery;

[HarmonyPatch(typeof(Pawn))]
public static class Pawn_Patch
{
    [HarmonyPatch(nameof(Pawn.ButcherProducts))]
    [HarmonyPostfix]
    public static IEnumerable<Thing> Postfix(IEnumerable<Thing> __result, Pawn __instance, Pawn butcher,
        float efficiency)
    {
        foreach (Thing thing in __result)
        {
            yield return thing;
        }

        if (!__instance.kindDef.HasModExtension<RagnarokButcherExtension>()) yield break;
        
        RagnarokButcherExtension ext = __instance.kindDef.GetModExtension<RagnarokButcherExtension>();
        foreach (ButcherThingDefScaleClass product in ext.AdditionalButcherProducts)
        {
            float randAmount = GenMath.RoundRandom(__instance.GetStatValue(StatDefOf.MeatAmount, true) * efficiency) *
                               0.1f;
            float scaledRandAmount = randAmount * product.value;

            int amount = Math.Max(1, Mathf.RoundToInt(scaledRandAmount));

            Thing thing = ThingMaker.MakeThing(product.key, null);
            thing.stackCount = amount;

            yield return thing;
        }

        foreach (ButcherThingDefCountClass product in ext.AdditionalButcherProductsFixedValue)
        {

            Thing thing = ThingMaker.MakeThing(product.key, null);
            thing.stackCount = product.value;

            yield return thing;
        }
    }
}
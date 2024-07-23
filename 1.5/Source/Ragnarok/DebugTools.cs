using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LudeonTK;
using MedievalOverhaul;
using RimWorld;
using Verse;

namespace Ragnarok;

public static class DebugTools
{
    public static IEnumerable<FieldInfo> IterativelyGetAllFields(Type type, BindingFlags flags)
    {
        // Iterate through all the parent classes to fetch all fields
        FieldInfo[] fields = type.GetFields(flags);

        return type.BaseType != null ? fields.Concat(IterativelyGetAllFields(type.BaseType, flags)) : fields;
    }

    [DebugAction("Ragnarok", "AnimaTreeFix", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    public static void AnimaTreeFix()
    {
        // This entire method is very fragile. If MO change Plant_SecondaryDrop at all and add extra fields and code in initializers, this is very likely to break
        // Luckily this should be a one-off patch as the actual XML patching is already fixed.

        // Search flags to get all the fields we can. Static may not be required, and is probably even redundant
        BindingFlags searchFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        // Just in case
        if(Find.CurrentMap == null)
            return;

        Map map = Find.CurrentMap;

        List<Thing> things = map.listerThings.ThingsOfDef(ThingDefOf.ArchonexusCore);

        foreach (Thing t in things)
        {
            if (t.TryGetComp<CompSpawnImmortalSubplantsAround>() is not { } comp || comp.Props.subplant != ThingDefOf.Plant_TreeAnima) return;
            List<IntVec3> cells = AccessTools.Field(typeof(CompSpawnImmortalSubplantsAround), "cells").GetValue(comp) as List<IntVec3>;
            if (cells == null) return;

            foreach (IntVec3 cell in cells)
            {
                if (cell.GetThingList(map).Find(thing => thing.def == ThingDefOf.Plant_TreeAnima) is not Plant plant) continue;

                int thingIdx = map.thingGrid.ThingsListAt(cell).IndexOf(plant);

                // Grab all the fields we want to copy
                Plant_SecondaryDrop plantCasted = new Plant_SecondaryDrop();
                List<FieldInfo> allFields = IterativelyGetAllFields(plant.GetType(), searchFlags).Where(f=>!(f.IsLiteral && !f.IsInitOnly)).ToList();

                foreach (FieldInfo fieldInfo in allFields)
                {
                    fieldInfo.SetValue(plantCasted, fieldInfo.GetValue(plant));
                }

                // Both appear to be required to ensure that in game trees are replaced, and saved trees.
                map.thingGrid.ThingsListAt(cell)[thingIdx] = plantCasted;
                map.listerThings.AllThings[map.listerThings.AllThings.IndexOf(plant)] = plantCasted;

            }
        }
    }
}

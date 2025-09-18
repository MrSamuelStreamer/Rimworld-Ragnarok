using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using Verse;

namespace Ragnarok;

public static class DebugTools
{
    public static List<String> SkippedFields = ["thingIDNumber", "mapIndexOrState", "holdingOwner"];

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

        Map map = Find.CurrentMap;

        // Just in case
        if(map == null)
            return;


        List<Thing> things = map.listerThings.ThingsOfDef(ThingDefOf.ArchonexusCore);

        foreach (Thing t in things)
        {
            if (t.TryGetComp<CompSpawnImmortalSubplantsAround>() is not { } comp || comp.Props.subplant != ThingDefOf.Plant_TreeAnima) return;
            List<IntVec3> cells = AccessTools.Field(typeof(CompSpawnImmortalSubplantsAround), "cells").GetValue(comp) as List<IntVec3>;
            if (cells == null) return;

            foreach (IntVec3 cell in cells)
            {
                if (cell.GetThingList(map).Find(thing => thing.def == ThingDefOf.Plant_TreeAnima) is not Plant plant) continue;

                // Grab all the fields we want to copy
                ThingWithComps newInstance = (ThingWithComps) Activator.CreateInstance(plant.def.thingClass);
                newInstance.def = plant.def;

                // Only part of postmake we need
                ThingIDMaker.GiveIDTo(newInstance);

                IEnumerable<FieldInfo> allRelevantFields = IterativelyGetAllFields(plant.GetType(), searchFlags)
                    .Where(f=>!(f.IsLiteral && !f.IsInitOnly))
                    .Where(f=>!SkippedFields.Contains(f.Name));

                // Set all the values, except the skipped ones
                foreach (FieldInfo fieldInfo in allRelevantFields)
                {
                    fieldInfo.SetValue(newInstance, fieldInfo.GetValue(plant));
                }

                // Update comp parents.
                // For anima tree comps at least, this is fine.
                foreach (ThingComp newcomp in newInstance.AllComps)
                {
                    newcomp.parent = newInstance;
                }

                //probably not needed, but just in case
                newInstance.PostPostMake();

                // must be vanish, or the anima scream will fire.
                // We should be able to skip this, as spawn will vanish the existing trees, but just in case.
                plant.Destroy(DestroyMode.Vanish);
                GenSpawn.Spawn(newInstance, cell, map, WipeMode.Vanish);
            }
        }
    }
}

using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok;

public class Settings : ModSettings
{
    public bool removeSun = true;
    public bool removePawnRewards = true;

    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new();
        options.Begin(wrect);
        if (Current.ProgramState == ProgramState.Playing &&
            ModLister.HasActiveModWithName("Medieval Overhaul") &&
            options.ButtonText("MSSRAG_Settings_RecreateAnimaTrees".Translate(), widthPct: 0.3f)) FixAnimaTrees();
        options.CheckboxLabeled("MSSRAG_Settings_RemoveSun".Translate(), ref removeSun);
        options.CheckboxLabeled("MSSRAG_Settings_RemovePawnRewards".Translate(), ref removePawnRewards);
        options.Gap();

        options.End();
    }

    private void FixAnimaTrees()
    {
        Map map = Find.CurrentMap;
        map.listerThings.ThingsOfDef(ThingDefOf.ArchonexusCore).ForEach(t =>
        {
            if (t.TryGetComp<CompSpawnImmortalSubplantsAround>() is not { } comp || comp.Props.subplant != ThingDefOf.Plant_TreeAnima) return;
            List<IntVec3> cells = AccessTools.Field(typeof(CompSpawnImmortalSubplantsAround), "cells").GetValue(comp) as List<IntVec3>;
            if (cells == null) return;
            foreach (IntVec3 cell in cells)
            {
                if (cell.GetThingList(map).Find(thing => thing.def == ThingDefOf.Plant_TreeAnima) is not Plant plant) continue;
                Plant newTree = ThingMaker.MakeThing(ThingDefOf.Plant_TreeAnima) as Plant;
                if (newTree == null) continue;
                newTree.Growth = plant.Growth;
                newTree.Age = plant.Age;
                plant.Destroy();
                GenSpawn.Spawn(newTree, cell, map, WipeMode.Vanish);
            }
        });
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref removeSun, "removeSun", true);
        Scribe_Values.Look(ref removePawnRewards, "removePawnRewards", true);
    }
}

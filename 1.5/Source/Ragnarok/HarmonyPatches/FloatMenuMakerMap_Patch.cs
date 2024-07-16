using System.Collections.Generic;
using HarmonyLib;
using Ragnarok.Anima;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Ragnarok.HarmonyPatches;

[HarmonyPatch(typeof(FloatMenuMakerMap))]
public class FloatMenuMakerMap_Patch
{
    public static TargetingParameters ForCultivating(Pawn cultivator)
    {
        return new TargetingParameters()
        {
            canTargetPawns = false,
            canTargetBloodfeeders = false,
            canTargetBuildings = false,
            mapObjectTargetsMustBeAutoAttackable = false,
            canTargetPlants = true,
            validator = targ =>
                targ is { HasThing: true, Thing: ThingWithComps thing } && thing != cultivator && 
                thing.HasComp<CompAnimaTreeCultivationConnection>() && 
                thing.TryGetComp<CompAnimaTreeCultivationConnection>().CurrentProduce != null &&
                StatDefOf.PruningSpeed.Worker.IsDisabledFor(cultivator) &&
                cultivator.GetPsylinkLevel() > 0
        };
    }

    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPostfix]
    public static void AddHumanlikeOrders_Patch(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        if (!RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished) return;
        if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) return;
        
        foreach (LocalTargetInfo dest in GenUI.TargetsAt(clickPos, ForCultivating(pawn), true))
        {
            if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly))
            {
                opts.Add(new FloatMenuOption(
                    "MSSRAG_CannotCultivate".Translate() + ": " + "NoPath".Translate().CapitalizeFirst(), null));
            }
            else
            {
                ThingWithComps thing = (ThingWithComps)dest.Thing;
                CompAnimaTreeCultivationConnection comp = thing.GetComp<CompAnimaTreeCultivationConnection>();

                opts.Add(FloatMenuUtility.DecoratePrioritizedTask(
                    new FloatMenuOption(
                        "MSSRAG_Cultivate".Translate(),
                        () =>
                        {
                            Job job = JobMaker.MakeJob(RagnarokDefOf.MSSRAG_CultivateAnimaTree, thing, pawn);
                            job.count = 1;
                            pawn.jobs.TryTakeOrderedJob(job);
                        },
                        MenuOptionPriority.High,
                        revalidateClickTarget: thing),
                    pawn,
                    (LocalTargetInfo)(Thing)thing));
            }
        }
    }

}
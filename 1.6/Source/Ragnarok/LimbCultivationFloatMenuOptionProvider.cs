using System;
using System.Collections.Generic;
using HarmonyLib;
using Ragnarok.Anima;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Ragnarok.HarmonyPatches;

public class LimbCultivationFloatMenuOptionProvider : FloatMenuOptionProvider
{
    protected override bool Drafted => true;
    protected override bool Undrafted => true;
    protected override bool Multiselect => false;
    protected override bool RequiresManipulation => true;

    protected override bool AppliesInt(FloatMenuContext context)
    {
        return RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished &&
               !StatDefOf.PruningSpeed.Worker.IsDisabledFor(context.FirstSelectedPawn) &&
               context.FirstSelectedPawn.GetPsylinkLevel() > 0;
    }

    protected override FloatMenuOption GetSingleOptionFor(
        Thing clickedThing,
        FloatMenuContext context)
    {
        if (clickedThing.TryGetComp<CompAnimaTreeCultivationConnection>()?.CurrentProduce == null) return null;

        return !context.FirstSelectedPawn.CanReach((LocalTargetInfo) clickedThing, PathEndMode.Touch, Danger.Deadly)
            ? new FloatMenuOption("MSSRAG_CannotCultivate".Translate() + ": " + "NoPath".Translate().CapitalizeFirst(), null)
            : FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(
                    "MSSRAG_Cultivate".Translate(),
                    () =>
                    {
                        Job job = JobMaker.MakeJob(RagnarokDefOf.MSSRAG_CultivateAnimaTree, clickedThing, context.FirstSelectedPawn);
                        job.count = 1;
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job);
                    },
                    MenuOptionPriority.High,
                    revalidateClickTarget: clickedThing),
                context.FirstSelectedPawn, (LocalTargetInfo) clickedThing);
    }
}

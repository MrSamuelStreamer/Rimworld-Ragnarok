using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Ragnarok.Anima;

public class JobDriver_CultivateAnimaTree : JobDriver
{
    public int numPositions = 1;
    
    public CompAnimaTreeCultivationConnection TreeConnection => job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompAnimaTreeCultivationConnection>();

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        AddFinishAction(delegate
        {
            TreeConnection.Produce();
        });
        this.FailOnDestroyedOrNull(TargetIndex.A);
        this.FailOn(() => !RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished);
        this.FailOn(() => StatDefOf.PruningSpeed.Worker.IsDisabledFor(pawn));
        this.FailOn(() => pawn.GetPsylinkLevel() <= 0);
        this.FailOn(() => TreeConnection.CurrentProduce == null);
        int ticks = Mathf.RoundToInt(2500f / pawn.GetStatValue(StatDefOf.PruningSpeed));
        Toil findAdjacentCell = Toils_General.Do(delegate
        {
            job.targetB = GetAdjacentCell(job.GetTarget(TargetIndex.A).Thing);
        });
        Toil goToAdjacentCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
        Toil prune = Toils_General.WaitWith(TargetIndex.A, ticks, true).WithEffect(EffecterDefOf.Harvest_MetaOnly, TargetIndex.A).WithEffect(EffecterDefOf.GauranlenDebris, TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Interact_Prune);
        prune.AddPreTickAction(delegate
        {
            TreeConnection.Cultivate(pawn);
            Pawn_SkillTracker skills = pawn.skills;
            if (skills != null)
            {
                skills.Learn(SkillDefOf.Plants, 0.085f);
            }
        });
        prune.activeSkill = (() => SkillDefOf.Plants);
        int num;
        for (int i = 0; i < numPositions; i = num + 1)
        {
            yield return findAdjacentCell;
            yield return goToAdjacentCell;
            yield return prune;
            num = i;
        }
        
    }
    
    private IntVec3 GetAdjacentCell(Thing treeThing)
    {
        IntVec3 result;
        return GenAdj.CellsAdjacent8Way(treeThing).Where(x => x.InBounds(pawn.Map) && !x.Fogged(pawn.Map) && !x.IsForbidden(pawn) && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Some)).TryRandomElement(out result) ? result : treeThing.Position;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref numPositions, "numPositions", 1);
    }
}
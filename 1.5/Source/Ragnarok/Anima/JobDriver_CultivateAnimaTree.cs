using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Ragnarok.Anima;

public class JobDriver_CultivateAnimaTree : JobDriver
{
    private int numPositions = 1;
    private const TargetIndex TreeIndex = TargetIndex.A;
    private const TargetIndex AdjacentCellIndex = TargetIndex.B;
    private const int DurationTicks = 2500;
    private const int MaxPositions = 8;
    
    private CompTreeConnection TreeConnection
    {
        get => this.job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompTreeConnection>();
    }
    
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);
        int ticks = Mathf.RoundToInt(2500f / this.pawn.GetStatValue(StatDefOf.PruningSpeed, true, -1));
        Toil findAdjacentCell = Toils_General.Do(delegate
        {
            this.job.targetB = this.GetAdjacentCell(this.job.GetTarget(TargetIndex.A).Thing);
        });
        Toil goToAdjacentCell = Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell).FailOn(() => this.TreeConnection.ConnectionStrength >= this.TreeConnection.DesiredConnectionStrength);
        Toil prune = Toils_General.WaitWith(TargetIndex.A, ticks, true, false, false, TargetIndex.None).WithEffect(EffecterDefOf.Harvest_MetaOnly, TargetIndex.A, null).WithEffect(EffecterDefOf.GauranlenDebris, TargetIndex.A, null).PlaySustainerOrSound(SoundDefOf.Interact_Prune, 1f);
        prune.AddPreTickAction(delegate
        {
            this.TreeConnection.Prune();
            Pawn_SkillTracker skills = this.pawn.skills;
            if (skills != null)
            {
                skills.Learn(SkillDefOf.Plants, 0.085f, false, false);
            }
            if (this.TreeConnection.ConnectionStrength >= this.TreeConnection.DesiredConnectionStrength)
            {
                base.ReadyForNextToil();
            }
        });
        prune.activeSkill = (() => SkillDefOf.Plants);
        int num;
        for (int i = 0; i < this.numPositions; i = num + 1)
        {
            yield return findAdjacentCell;
            yield return goToAdjacentCell;
            yield return prune;
            num = i;
        }
        yield break;
    }
    
    private IntVec3 GetAdjacentCell(Thing treeThing)
    {
        IntVec3 result;
        return GenAdj.CellsAdjacent8Way(treeThing).Where<IntVec3>((Func<IntVec3, bool>) (x => x.InBounds(this.pawn.Map) && !x.Fogged(this.pawn.Map) && !x.IsForbidden(this.pawn) && this.pawn.CanReserveAndReach((LocalTargetInfo) x, PathEndMode.OnCell, Danger.Some))).TryRandomElement<IntVec3>(out result) ? result : treeThing.Position;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look<int>(ref this.numPositions, "numPositions", 1);
    }
}
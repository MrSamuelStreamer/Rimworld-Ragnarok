using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok;

public class RitualOutcomeComp_SpawnItem : RitualOutcomeComp
{
    public SimpleCurve curve;
    public ThingDef itemDef;

    public override bool Applies(LordJob_Ritual ritual) => true;
}

public class RitualOutcomeEffectWorker_PruneTree : RitualOutcomeEffectWorker_FromQuality
{
    public override bool SupportsAttachableOutcomeEffect => false;

    public RitualOutcomeEffectWorker_PruneTree()
    {
    }

    public RitualOutcomeEffectWorker_PruneTree(RitualOutcomeEffectDef def)
        : base(def)
    {
    }

    public bool IsWorstNegativeOutcome(LordJob_Ritual ritual, RitualOutcomePossibility outcome) =>
        ritual.Ritual.outcomeEffect.def.outcomeChances.All(outcomeChance => outcomeChance.positivityIndex >= outcome.positivityIndex);

    public override void ApplyExtraOutcome(
        Dictionary<Pawn, int> totalPresence,
        LordJob_Ritual jobRitual,
        RitualOutcomePossibility outcome,
        out string extraOutcomeDesc,
        ref LookTargets letterLookTargets)
    {
        extraOutcomeDesc = null;
        if (!outcome.Positive && IsWorstNegativeOutcome(jobRitual, outcome) && Rand.Chance(0.3f))
        {
            jobRitual.selectedTarget.Thing.Destroy(DestroyMode.KillFinalize);
            extraOutcomeDesc = "MSSRAG_PruneTreeFail".Translate();
        }

        float quality = GetQuality(jobRitual, 1f);
        RitualOutcomeComp_SpawnItem spawnItemComp = def.comps.Find(c => c is RitualOutcomeComp_SpawnItem) as RitualOutcomeComp_SpawnItem;
        Thing tree = jobRitual.selectedTarget.Thing;
        if (spawnItemComp == null || tree == null)
            return;
        int numToSpawn = Mathf.RoundToInt(spawnItemComp.curve.Evaluate(quality));
        extraOutcomeDesc = "MSSRAG_PruneTreeSuccess".Translate(numToSpawn.ToString(), spawnItemComp.itemDef.label);
        for (int i = 0; i < numToSpawn; i++)
        {
            GenPlace.TryPlaceThing(ThingMaker.MakeThing(spawnItemComp.itemDef), tree.Position, tree.Map, ThingPlaceMode.Near);
        }
    }
}

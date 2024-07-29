using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Ragnarok;

public class CompProperties_AnimaStatBoost : CompProperties
{
    public float factorPerHediff = 0.1f;
    public List<HediffDef> relevantHediffs = [];
    public StatDef statDef = null;

    public CompProperties_AnimaStatBoost()
    {
        compClass = typeof(CompAnimaStatBoost);
    }
}

public class StatPart_AnimaStatBoost : StatPart
{
    public override void TransformValue(StatRequest req, ref float val)
    {
        if (req.Thing?.TryGetComp<CompAnimaStatBoost>() is not {} comp) return;
        val *= comp.GetAnimaFactor();
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (req.Thing?.TryGetComp<CompAnimaStatBoost>() is not {} comp) return null;
        return "MSSRAG_Stat_AnimaAttunement".Translate() + ": x" + comp.GetAnimaFactor().ToStringPercent();
    }
}

public class CompAnimaStatBoost : ThingComp
{
    public CompProperties_AnimaStatBoost Props => (CompProperties_AnimaStatBoost) props;

    public float animaHediffsFactorCache = 1;
    public int nextCacheTick = -1;

    public Pawn Holder => (parent?.ParentHolder as Pawn_EquipmentTracker)?.pawn;

    public float GetAnimaFactor()
    {
        Pawn holder = Holder;
        if (holder == null || Find.TickManager.TicksGame < nextCacheTick) return animaHediffsFactorCache;
        return RecacheAnimaFactor(holder);
    }

    public float RecacheAnimaFactor(Pawn holder)
    {
        holder ??= Holder;
        if (holder == null) return 1;
        int hediffCount = holder.health?.hediffSet?.hediffs?.Where(h => Props.relevantHediffs.Contains(h.def)).Count() ?? 0;
        animaHediffsFactorCache = 1 + hediffCount * Props.factorPerHediff;
        nextCacheTick = Find.TickManager.TicksGame + 60000;
        return animaHediffsFactorCache;
    }

    public override void Notify_Equipped(Pawn pawn)
    {
        base.Notify_Equipped(pawn);
        RecacheAnimaFactor(pawn);
    }

    public override void Notify_Unequipped(Pawn pawn)
    {
        base.Notify_Unequipped(pawn);
        animaHediffsFactorCache = 1;
    }

    public override IEnumerable<StatDrawEntry> SpecialDisplayStats()
    {
        foreach (StatDrawEntry specialDisplayStat in base.SpecialDisplayStats() ?? [])
        {
            yield return specialDisplayStat;
        }

        yield return new StatDrawEntry(StatCategoryDefOf.Weapon, RagnarokDefOf.MSSRAG_Stat_AnimaAttunement, GetAnimaFactor().ToStringPercent());
    }
}

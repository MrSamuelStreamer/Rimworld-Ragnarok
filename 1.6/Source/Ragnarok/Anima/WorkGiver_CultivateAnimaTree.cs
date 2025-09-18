using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Ragnarok.Anima;

public class WorkGiver_CultivateAnimaTree : WorkGiver_Scanner
{

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerThings.ThingsMatching(ThingRequest.ForDef(ThingDefOf.Plant_TreeAnima));
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!ModsConfig.IdeologyActive)
            return false;
        if (!RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished)
            return false;
        if (pawn.GetPsylinkLevel() <= 0)
            return false;
        CompAnimaTreeCultivationConnection comp = t.TryGetComp<CompAnimaTreeCultivationConnection>();
        return comp != null && comp.CurrentProduce != null && !t.IsForbidden(pawn) && pawn.CanReserve((LocalTargetInfo) t, ignoreOtherReservations: forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        Job job = JobMaker.MakeJob(RagnarokDefOf.MSSRAG_CultivateAnimaTree, (LocalTargetInfo) t);
        job.playerForced = forced;
        return job;
    }
    
}
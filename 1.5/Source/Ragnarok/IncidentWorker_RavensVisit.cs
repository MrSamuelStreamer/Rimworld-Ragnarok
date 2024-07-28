using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Ragnarok;

public class IncidentWorker_RavensVisit : IncidentWorker
{
    private const int PawnsStayDurationMin = 90000;
    private const int PawnsStayDurationMax = 150000;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        PawnKindDef pawnKind = parms.pawnKind ?? def.pawnKind;
        if (pawnKind == null || !base.CanFireNowSub(parms)) return false;
        Map target = (Map) parms.target;
        return target.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(pawnKind.race) &&
               TryFindEntryCell(target, out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map target = (Map) parms.target;
        if (!TryFindEntryCell(target, out IntVec3 entryCell))
            return false;
        int lurkTime = Rand.RangeInclusive(PawnsStayDurationMin, PawnsStayDurationMax);
        if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(entryCell, target, 10f, out IntVec3 location))
            location = IntVec3.Invalid;
        List<Pawn> pawns = [PawnGenerator.GeneratePawn(RagnarokDefOf.MSSRAG_Huginn), PawnGenerator.GeneratePawn(RagnarokDefOf.MSSRAG_Muninn)];
        foreach (Pawn pawn in pawns)
        {
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(entryCell, target, 10);
            GenSpawn.Spawn(pawn, loc, target, Rot4.Random);
            pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + lurkTime;
            if (location.IsValid)
                pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(location, target, 10);
        }

        SendStandardLetter(
            "MSSRAG_LetterLabelRavensVisit".Translate().CapitalizeFirst(),
            "MSSRAG_LetterRavensVisit".Translate((NamedArgument) pawns[0].LabelCap, pawns[1].LabelCap), LetterDefOf.NeutralEvent, parms,
            new LookTargets(pawns));
        return true;
    }

    private static bool TryFindEntryCell(Map map, out IntVec3 entryCell) =>
        RCellFinder.TryFindRandomPawnEntryCell(out entryCell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
}

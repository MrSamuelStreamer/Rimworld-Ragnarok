using RimWorld;
using Verse;

namespace Ragnarok;

public class IncidentWorker_DarknessSeepsIn : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms) =>
        FindRandomLightSource((Map)parms.target) != null;

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        Thing lightSource = FindRandomLightSource(map);
        if (lightSource == null)
            return false;

        CompRefuelable fuelComp = lightSource.TryGetComp<CompRefuelable>();
        fuelComp.ConsumeFuel(fuelComp.Fuel);

        SendStandardLetter(
            "MSSRAG_LetterLabel_Incident_DarknessSeepsIn".Translate(),
            "MSSRAG_Letter_Incident_DarknessSeepsIn".Translate(lightSource.def.label),
            LetterDefOf.NegativeEvent, parms, new TargetInfo(lightSource.Position, map));
        return true;
    }

    public static Thing FindRandomLightSource(Map map) =>
        map.listerBuildings
            .allBuildingsColonist
            .Find(b => b.TryGetComp<CompGlower>() is { Glows: true } &&
                       b.TryGetComp<CompRefuelable>() is { HasFuel: true });
}

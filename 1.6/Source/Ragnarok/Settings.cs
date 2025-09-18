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
            Find.CurrentMap != null &&
            options.ButtonText("MSSRAG_Settings_RecreateAnimaTrees".Translate(), widthPct: 0.3f)) DebugTools.AnimaTreeFix();
        options.CheckboxLabeled("MSSRAG_Settings_RemoveSun".Translate(), ref removeSun);
        options.CheckboxLabeled("MSSRAG_Settings_RemovePawnRewards".Translate(), ref removePawnRewards);
        options.Gap();

        options.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref removeSun, "removeSun", true);
        Scribe_Values.Look(ref removePawnRewards, "removePawnRewards", true);
    }
}

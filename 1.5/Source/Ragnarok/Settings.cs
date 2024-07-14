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

using UnityEngine;
using Verse;

namespace Ragnarok;

public class Settings : ModSettings
{
    //Use Mod.settings.setting to refer to this setting.
    public bool removeSun = true;

    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new Listing_Standard();
        options.Begin(wrect);
        
        options.CheckboxLabeled("Ragnarok_Settings_SettingName".Translate(), ref removeSun);
        options.Gap();

        options.End();
    }
    
    public override void ExposeData()
    {
        Scribe_Values.Look(ref removeSun, "removeSun", true);
    }
}

using RimWorld;
using Verse;

namespace Ragnarok.Tattoo;

public class HediffCompProperties_Tattoo : HediffCompProperties
{
    public bool OverrideExistingTattoo = false;
    public TattooType TattooType = TattooType.Body;
    public TattooDef Tattoo = null;
    
    public HediffCompProperties_Tattoo()
    {
        this.compClass = typeof (HediffComp_Tattoo);
    }
    
}
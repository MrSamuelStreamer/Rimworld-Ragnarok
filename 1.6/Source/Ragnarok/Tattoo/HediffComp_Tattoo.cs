using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Ragnarok.Tattoo;

public class HediffComp_Tattoo: HediffComp
{
    private HediffCompProperties_Tattoo Props => (HediffCompProperties_Tattoo) props;
    
    
    public override void CompPostMake()
    {
        // Don't overwrite existing tattoos, unless OverrideExistingTattoo
        if(Props.TattooType == TattooType.Body && Pawn.style.BodyTattoo != null && !Props.OverrideExistingTattoo) return;
        if(Props.TattooType == TattooType.Face && Pawn.style.FaceTattoo != null && !Props.OverrideExistingTattoo) return;

        TattooDef newTattoo;
        if (Props.Tattoo != null)
        {
            newTattoo = Props.Tattoo;
        }
        else
        {
            // Simplified version of the random tattoo getter
            IEnumerable<TattooDef> relevantTattoos =
                DefDatabase<TattooDef>.AllDefs.Where(x => PawnStyleItemChooser.WantsToUseStyle(Pawn, x, Props.TattooType));

            newTattoo = relevantTattoos.RandomElementWithFallback();
        }

        if (Props.TattooType == TattooType.Body)
        {
            // bypass setter to override ideology
            BodyTattooField.Value.SetValue(Pawn.style, newTattoo);
        }
        else
        {
            // bypass setter to override ideology
            FaceTattooField.Value.SetValue(Pawn.style, newTattoo);
        }
    }
    
    Lazy<FieldInfo> BodyTattooField = new(() => AccessTools.Field(typeof(Pawn_StyleTracker), "bodyTattoo"));
    Lazy<FieldInfo> FaceTattooField = new(() => AccessTools.Field(typeof(Pawn_StyleTracker), "faceTattoo"));

}
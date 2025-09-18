using System.Reflection;
using Verse;
using UnityEngine;
using HarmonyLib;
using RimWorld;

namespace Ragnarok;

public class RagnarokMod : Mod
{
    public static Settings settings;

    public RagnarokMod(ModContentPack content) : base(content)
    {

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("MrSamuelStreamer.rimworld.Ragnarok.main");
        harmony.PatchAll();
        SetCurvePointsToZero();
    }

    public static void SetCurvePointsToZero()
    {
        // Get the type of GenCelestial
        var genCelestialType = typeof(GenCelestial);

        // Get the fields of type SimpleCurve
        var sunPeekAroundDegreesFactorCurveField = genCelestialType.GetField("SunPeekAroundDegreesFactorCurve", BindingFlags.NonPublic | BindingFlags.Static);
        var sunOffsetFractionFromLatitudeCurveField = genCelestialType.GetField("SunOffsetFractionFromLatitudeCurve", BindingFlags.NonPublic | BindingFlags.Static);

        // Get the instances of SimpleCurve
        var sunPeekAroundDegreesFactorCurve = (SimpleCurve)sunPeekAroundDegreesFactorCurveField?.GetValue(null);
        if (sunOffsetFractionFromLatitudeCurveField != null && sunPeekAroundDegreesFactorCurveField != null)
        {
            var sunOffsetFractionFromLatitudeCurve = (SimpleCurve)sunOffsetFractionFromLatitudeCurveField.GetValue(null);

            // Modify the points of the curves
            SetAllPointsToZero(sunPeekAroundDegreesFactorCurve);
            SetAllPointsToZero(sunOffsetFractionFromLatitudeCurve);

            // Set the modified curves back to the fields
            sunPeekAroundDegreesFactorCurveField.SetValue(null, sunPeekAroundDegreesFactorCurve);
            sunOffsetFractionFromLatitudeCurveField.SetValue(null, sunOffsetFractionFromLatitudeCurve);
        }
    }

    private static void SetAllPointsToZero(SimpleCurve curve)
    {
        for (int i = 0; i < curve.PointsCount; i++)
        {
            var point = new CurvePoint(0f, 0f);
            curve[i] = point;
        }
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "MSSRAG_SettingsCategory".Translate();
    }
}

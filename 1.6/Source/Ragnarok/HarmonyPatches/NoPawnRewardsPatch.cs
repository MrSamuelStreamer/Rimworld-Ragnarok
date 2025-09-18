using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;

namespace Ragnarok.HarmonyPatches;

public static class NoPawnRewardsPatch
{
    [HarmonyPatch(typeof(QuestGen_Rewards), "GenerateRewards")]
    public static class QuestGen_Rewards_Patch
    {
        [HarmonyPrefix]
        public static bool PrefixGenerateRewards(ref RewardsGeneratorParams parmsResolved)
        {
            if (RagnarokMod.settings.removePawnRewards) parmsResolved.thingRewardItemsOnly = true;
            return true;
        }
    }
}

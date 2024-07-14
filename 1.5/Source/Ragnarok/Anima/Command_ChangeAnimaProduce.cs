using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok.Anima;

public class Command_ChangeAnimaProduce(CompAnimaTreeCultivationConnection tree) : Command_Action
{
    public readonly CompAnimaTreeCultivationConnection TreeCultivation = tree;

    public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
    {
        // TODO: Check research prerequisites
        get
        {
            if (TreeCultivation == null) return Enumerable.Empty<FloatMenuOption>();

            var defs = DefDatabase<AnimaTreeProductDef>.AllDefs.ToList();
            
            return defs
                .Select(animaTreeProductDef => new FloatMenuOption(animaTreeProductDef.Product.label, () =>
                {
                    TreeCultivation.CurrentProduce = animaTreeProductDef;
                    TreeCultivation.CultivationWork = 0;
                }));
        }
    }
}
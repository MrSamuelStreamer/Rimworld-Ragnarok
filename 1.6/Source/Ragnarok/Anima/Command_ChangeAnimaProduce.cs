using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Ragnarok.Anima;

public class Command_ChangeAnimaProduce(CompAnimaTreeCultivationConnection tree) : Command_Action
{
    public readonly CompAnimaTreeCultivationConnection TreeCultivation = tree;
    
    public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
    {
        get
        {
            yield return new FloatMenuOption("MSSRAG_NoProduce".Translate(), () =>
            {
                TreeCultivation.CurrentProduce = null;
            });
            
            
            if (TreeCultivation == null)
            {
                yield break;
            }
            
            IEnumerable<AnimaTreeProductDef> defs = DefDatabase<AnimaTreeProductDef>.AllDefs;
            
            foreach (AnimaTreeProductDef def in defs)
            {
                Action fmAction = null;
                string label = "MSSRAG_NotResearched".Translate(def.Product.LabelCap);

                if (def.ResearchPrerequisites.All(rp => rp.IsFinished))
                {
                    fmAction = () =>
                    {
                        TreeCultivation.CurrentProduce = def;
                        if (TreeCultivation.CultivationWork <= 0) TreeCultivation.CultivationWork = 0;
                    };

                    label = def.Product.LabelCap;

                }

                yield return new FloatMenuOption(label, fmAction);
            }
                
        }
    }
}
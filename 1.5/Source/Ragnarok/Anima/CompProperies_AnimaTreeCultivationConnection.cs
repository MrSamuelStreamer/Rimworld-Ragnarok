using System.Collections.Generic;
using Verse;

namespace Ragnarok.Anima;

public class CompProperies_AnimaTreeCultivationConnection: CompProperties
{
    public AnimaTreeProductDef DefaultProduce;
    
    public CompProperies_AnimaTreeCultivationConnection() => this.compClass = typeof (CompAnimaTreeCultivationConnection);
    
}
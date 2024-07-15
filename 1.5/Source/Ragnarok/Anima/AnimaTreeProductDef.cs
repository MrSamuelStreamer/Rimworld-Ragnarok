using System.Collections.Generic;
using Verse;

namespace Ragnarok.Anima;

public class AnimaTreeProductDef : Def
{
    public ThingDef Product;
    public int ProduceCount;
    public int CultivationWorkToProduce;
    public List<ResearchProjectDef> ResearchPrerequisites;
    public bool KeepProducing = false;

    public new string label => Product?.label;
    public new string description => Product?.description;
}
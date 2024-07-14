using RimWorld;
using Verse;

namespace Ragnarok.Anima;

public class AnimaTreeProductDef : Def
{
    public ThingDef Product;
    public int ProduceCount;
    public int CultivationWorkToProduce;
    private string cachedDescription;
    
    
    public string Description
    {
        get
        {
            if (cachedDescription != null) return cachedDescription;
            cachedDescription = description;

            if (Product != null && !Product.description.NullOrEmpty())
                cachedDescription = Product.description;
            return cachedDescription;
        }
    }
}
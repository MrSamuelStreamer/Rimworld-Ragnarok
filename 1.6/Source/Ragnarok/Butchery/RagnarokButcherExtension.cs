using System.Collections.Generic;
using Verse;

namespace Ragnarok.Butchery;

public class RagnarokButcherExtension : DefModExtension
{
    public List<ButcherThingDefScaleClass> AdditionalButcherProducts= new List<ButcherThingDefScaleClass>();
    public List<ButcherThingDefCountClass> AdditionalButcherProductsFixedValue= new List<ButcherThingDefCountClass>();
}
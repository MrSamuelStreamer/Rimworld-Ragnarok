using System.Xml;
using Verse;

namespace Ragnarok.Butchery;

public class ButcherThingDefScaleClass
{
    public ThingDef key;
    public float value = 1f;

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef((object) this, "key", xmlRoot.Name);
        if (xmlRoot.HasChildNodes)
            this.value = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        else
            this.value = 1f;
    }
}
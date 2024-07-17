using System.Xml;
using Verse;

namespace Ragnarok.Butchery;

public class ButcherThingDefCountClass
{
    public ThingDef key;
    public int value = 1;

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef((object) this, "key", xmlRoot.Name);
        if (xmlRoot.HasChildNodes)
            this.value = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
        else
            this.value = 1;
    }
}
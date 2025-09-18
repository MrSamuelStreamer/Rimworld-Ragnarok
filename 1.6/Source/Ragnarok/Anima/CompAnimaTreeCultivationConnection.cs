using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok.Anima;

public class CompAnimaTreeCultivationConnection : ThingComp
{
    public AnimaTreeProductDef CurrentProduce;
    public float CultivationWork = -1;
    public Effecter SpawnItemEffector;
    
    public CompProperies_AnimaTreeCultivationConnection Props => (CompProperies_AnimaTreeCultivationConnection)props;
    
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Defs.Look(ref CurrentProduce, "CurrentProduce");
        Scribe_Values.Look(ref CultivationWork, "CultivationWork", -1);
    }

    public void Cultivate(Pawn pawn)
    {
        float workSpeed = pawn.GetStatValue(StatDefOf.PlantWorkSpeed);
        CultivationWork += (0.085f * workSpeed);
        Produce();
    }

    public void Produce()
    {
        if (CultivationWork < CurrentProduce.CultivationWorkToProduce) return;
        //spawn
        Thing spawnedItem = GenSpawn.Spawn(ThingMaker.MakeThing(CurrentProduce.Product), parent.Position, parent.Map);
        spawnedItem.stackCount = CurrentProduce.ProduceCount;
        SpawnItemEffector = Props.SpawnItemEffector.Spawn();
        SpawnItemEffector.Trigger(parent, parent);
        CultivationWork -= CurrentProduce.CultivationWorkToProduce;
        if(!CurrentProduce.KeepProducing)
            CurrentProduce = null;
    }

    public override void CompTickLong()
    {
        base.CompTickLong();
        
        SpawnItemEffector?.EffectTick(parent, parent);
    }
    
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if(!RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished) yield break;
        
        Command_ChangeAnimaProduce commandAction = new Command_ChangeAnimaProduce(this);
        commandAction.defaultLabel = "MSSRAG_ChangeAnimaProduct".Translate();
        commandAction.defaultDesc = "MSSRAG_ChangeAnimaProductDesc".Translate(parent.Named("TREE"));
        commandAction.icon = CurrentProduce == null ? ContentFinder<Texture2D>.Get("UI/Buttons/MSSRAG_NoAnimaProduceSelected") : (Texture) Widgets.GetIconFor(CurrentProduce.Product);
        commandAction.action = () =>
        {
            Event.current.Use();
            Find.WindowStack.Add(new FloatMenu(commandAction.RightClickFloatMenuOptions.ToList()));
        };
        
        yield return commandAction;
    }

    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        base.PostDestroy(mode, previousMap);
        SpawnItemEffector?.Cleanup();
        SpawnItemEffector = null;
    }

    public override string CompInspectStringExtra()
    {
        StringBuilder sb = new StringBuilder(base.CompInspectStringExtra());

        if (!RagnarokDefOf.MSSRAG_LimbCultivation.IsFinished) return sb.ToString();

        if (CurrentProduce == null)
        {
            sb.Append("MSSRAG_Cultivation_None".Translate());
        }
        else
        {
            sb.AppendLine("MSSRAG_Cultivation_Item".Translate(CurrentProduce.LabelCap)); 
            sb.Append("MSSRAG_Cultivation_Level".Translate(CultivationWork, CurrentProduce.CultivationWorkToProduce));   
        }

        return sb.ToString();
    }
}
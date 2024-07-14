using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Ragnarok.Anima;

public class CompAnimaTreeCultivationConnection : ThingComp
{
    public AnimaTreeProductDef CurrentProduce;
    public int CultivationWork = -1;

    public AnimaTreeProductDef SafeCurrentProduce => CurrentProduce ?? Props.DefaultProduce;
    
    public CompProperies_AnimaTreeCultivationConnection Props => (CompProperies_AnimaTreeCultivationConnection)props;
    
    
    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Defs.Look(ref CurrentProduce, "CurrentProduce");
        Scribe_Values.Look(ref CultivationWork, "CultivationWork", -1);
    }

    

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        
        Command_ChangeAnimaProduce commandAction = new Command_ChangeAnimaProduce(this);
        commandAction.defaultLabel = (string) "MSSRAG_ChangeAnimaProduct".Translate();
        commandAction.defaultDesc = (string) "MSSRAG_ChangeAnimaProductDesc".Translate(this.parent.Named("TREE"));
        commandAction.icon = SafeCurrentProduce == null ? ContentFinder<Texture2D>.Get("UI/Gizmos/UpgradeDryads") : (Texture) Widgets.GetIconFor(SafeCurrentProduce.Product);
        commandAction.action = () =>
        {
            Event.current.Use();
            Find.WindowStack.Add(new FloatMenu(commandAction.RightClickFloatMenuOptions.ToList()));
        };
        
        yield return (Gizmo) commandAction;
        // if (this.pruningGizmo == null)
          // this.pruningGizmo = new Gizmo_PruningConfig(this);
        // yield return (Gizmo) this.pruningGizmo;

      // if (DebugSettings.ShowDevGizmos)
      // {
      //   Command_Action commandAction1 = new Command_Action();
      //   commandAction1.defaultLabel = "DEV: Spawn dryad";
      //   // ISSUE: reference to a compiler-generated method
      //   commandAction1.action = new Action(this.\u003CCompGetGizmosExtra\u003Eb__73_1);
      //   yield return (Gizmo) commandAction1;
      //   Command_Action commandAction2 = new Command_Action();
      //   commandAction2.defaultLabel = "DEV: Connection strength -10%";
      //   // ISSUE: reference to a compiler-generated method
      //   commandAction2.action = new Action(this.\u003CCompGetGizmosExtra\u003Eb__73_2);
      //   yield return (Gizmo) commandAction2;
      //   Command_Action commandAction3 = new Command_Action();
      //   commandAction3.defaultLabel = "DEV: Connection strength +10%";
      //   // ISSUE: reference to a compiler-generated method
      //   commandAction3.action = new Action(this.\u003CCompGetGizmosExtra\u003Eb__73_3);
      //   yield return (Gizmo) commandAction3;
      // }
    }
}
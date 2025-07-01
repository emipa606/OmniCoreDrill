using System.Collections.Generic;
using System.Linq;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

internal class GameMaterialParameters : GameComponent
{
    private Dictionary<ThingDef, MaterialParameters> _materialModifiers = new();

    private GameMaterialParameters()
    {
    }

    public GameMaterialParameters(Game game) : this()
    {
    }

    public MaterialParameters this[ThingDef def] => _materialModifiers.GetValueOrDefault(def);


    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Collections.Look(ref _materialModifiers, "materials");
    }

    public override void LoadedGame()
    {
        base.LoadedGame();

        prepareDrill();
    }

    public override void StartedNewGame()
    {
        base.StartedNewGame();

        prepareDrill();
    }

    private void prepareDrill()
    {
        upsertMaterialModifiers();
        ThingDefGenerator.UpdateAllGeneratedDefs();
        LoadedModManager.GetMod<OmniCoreDrillMod>().MaterialParams = this;
    }

    private void upsertMaterialModifiers()
    {
        foreach (var coreMiningDef in ThingDefGenerator.GetCoreMiningDefs())
        {
            var key = coreMiningDef.products.First().thingDef;

            if (!_materialModifiers.ContainsKey(key))
            {
                _materialModifiers.Add(key, new MaterialParameters(key));
            }
        }
    }
}
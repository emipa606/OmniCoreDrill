using System.ComponentModel;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

internal class MaterialParameters : IExposable
{
    private ThingDef _material;
    private Multiplier _work;
    private Multiplier _yield;

    private MaterialParameters()
    {
        _work = new Multiplier();
        _yield = new Multiplier();

        _work.PropertyChanged += Component_PropertyChanged;
        _yield.PropertyChanged += Component_PropertyChanged;
    }

    public MaterialParameters(ThingDef material) : this()
    {
        _material = material;
    }

    public Multiplier Work => _work;

    public Multiplier Yield => _yield;

    public void ExposeData()
    {
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            _work.PropertyChanged -= Component_PropertyChanged;
            _yield.PropertyChanged -= Component_PropertyChanged;
        }

        Scribe_Deep.Look(ref _work, "work");
        Scribe_Deep.Look(ref _yield, "yield");
        Scribe_Defs.Look(ref _material, "material");

        if (Scribe.mode != LoadSaveMode.LoadingVars)
        {
            return;
        }

        _work.PropertyChanged += Component_PropertyChanged;
        _yield.PropertyChanged += Component_PropertyChanged;
    }

    private void Component_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Multiplier.Value):
                ThingDefGenerator.UpdateGeneratedDef(_material);
                break;
        }
    }
}
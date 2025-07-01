using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

internal class Settings : ModSettings
{
    private GlobalDrillParameters _props = new();

    public GlobalDrillParameters GlobalParameters => _props;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref _props, "props");
    }
}
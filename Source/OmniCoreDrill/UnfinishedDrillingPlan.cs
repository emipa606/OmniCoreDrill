using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

public class UnfinishedDrillingPlan : UnfinishedThing
{
    public override string LabelNoCount =>
        LanguageKeys.keyed.ocd_unfinishedPlan.Translate(Recipe.products[0].thingDef.label);

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        if (Recipe != null)
        {
            return;
        }

        Destroy();
    }
}
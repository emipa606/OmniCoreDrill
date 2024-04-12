using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

public static class ThingDefGenerator
{
    private static IDictionary<ThingDef, RecipeDef> _miningRecipes;

    private static IDictionary<ThingDef, DrillingProperties> _drillProperties;

    private static IDictionary<ThingDef, string> _sourceLabels;

    public static IEnumerable<KeyValuePair<ThingDef, RecipeDef>> AllDrillRecipes => _miningRecipes;

    public static ICollection<RecipeDef> GetCoreMiningDefs()
    {
        if (_miningRecipes != null)
        {
            return _miningRecipes.Values;
        }

        _miningRecipes = new Dictionary<ThingDef, RecipeDef>();
        _drillProperties = new Dictionary<ThingDef, DrillingProperties>();
        _sourceLabels = new Dictionary<ThingDef, string>();
        var minableDefs = DefDatabase<ThingDef>.AllDefs.Where(td =>
            td.mineable && td.building?.mineableThing != null);

        foreach (var thingDef in minableDefs)
        {
            var props = CalculateDrillProperties(thingDef, out var buildingProperties);
            if (props == null)
            {
                Log.Warning(
                    $"Cannot determine hitpoints for {thingDef.defName} - skipping deep resource extraction recipe.");
                continue;
            }

            var mineableThing = buildingProperties.mineableThing;

            var recipe = new RecipeDef
            {
                efficiencyStat = StatDefOf.MiningYield,
                workSpeedStat = StatDefOf.DeepDrillingSpeed,
                effectWorking = EffecterDefOf.Drill,
                workSkillLearnFactor = 0.2f,
                workSkill = SkillDefOf.Mining,
                defName = $"OCD_MineDeep{mineableThing.defName}",
                label = LanguageKeys.keyed.ocd_label.Translate(mineableThing.LabelCap),
                jobString = LanguageKeys.keyed.ocd_jobString.Translate(mineableThing.LabelCap),
                products =
                [
                    new ThingDefCountClass
                    {
                        thingDef = mineableThing,
                        count = 0
                    }
                ],
                recipeUsers = [DefReferences.Thing_CoreDrill],
                unfinishedThingDef = DefReferences.Thing_UnfinishedDrillingPlan
            };

            UpdateGeneratedDef(mineableThing, recipe, props, thingDef.LabelCap);

            _miningRecipes[mineableThing] = recipe;
            _drillProperties[mineableThing] = props;
            _sourceLabels[mineableThing] = thingDef.LabelCap;
        }

        foreach (var mineableThing in DefDatabase<ThingDef>.AllDefs.Where(td => td.deepCommonality > 0))
        {
            if (_miningRecipes.ContainsKey(mineableThing))
            {
                continue;
            }

            var nearestThingDef = minableDefs
                .OrderBy(def => Math.Abs(def.building.mineableThing.BaseMarketValue - mineableThing.BaseMarketValue))
                .First();
            Log.Message(
                $"Using {nearestThingDef} as a substitute for {mineableThing} to calculate deep mining values.");

            var props = CalculateDrillProperties(nearestThingDef, out _);
            if (props == null)
            {
                Log.Warning(
                    $"Cannot determine hitpoints for {mineableThing.defName} - skipping deep resource extraction recipe.");
                continue;
            }

            var recipe = new RecipeDef
            {
                efficiencyStat = StatDefOf.MiningYield,
                workSpeedStat = StatDefOf.DeepDrillingSpeed,
                effectWorking = EffecterDefOf.Drill,
                workSkillLearnFactor = 0.2f,
                workSkill = SkillDefOf.Mining,
                defName = $"OCD_MineDeep{mineableThing.defName}",
                label = LanguageKeys.keyed.ocd_label.Translate(mineableThing.LabelCap),
                jobString = LanguageKeys.keyed.ocd_jobString.Translate(mineableThing.LabelCap),
                products =
                [
                    new ThingDefCountClass
                    {
                        thingDef = mineableThing,
                        count = 0
                    }
                ],
                recipeUsers = [DefReferences.Thing_CoreDrill],
                unfinishedThingDef = DefReferences.Thing_UnfinishedDrillingPlan
            };

            UpdateGeneratedDef(mineableThing, recipe, props, mineableThing.LabelCap);

            _miningRecipes[mineableThing] = recipe;
            _drillProperties[mineableThing] = props;
            _sourceLabels[mineableThing] = mineableThing.LabelCap;
        }

        return _miningRecipes.Values;
    }

    private static DrillingProperties CalculateDrillProperties(ThingDef source,
        out BuildingProperties buildingProperties)
    {
        buildingProperties = source.building;

        var hitpointsPerLump = source.statBases.FirstOrDefault(sm => sm.stat == StatDefOf.MaxHitPoints)?.value;

        return hitpointsPerLump == null ? null : new DrillingProperties(hitpointsPerLump.Value, buildingProperties);
    }

    public static void UpdateGeneratedDef(ThingDef material)
    {
        UpdateGeneratedDef(material, _miningRecipes[material], _drillProperties[material], _sourceLabels[material]);
    }

    private static void UpdateGeneratedDef(ThingDef material, RecipeDef recipe, DrillingProperties props,
        string sourceLabel)
    {
        var yield = props.Yield;
        var work = props.Work;

        var mp = Current.Game?.GetComponent<GameMaterialParameters>()[material];
        if (mp != null)
        {
            yield *= mp.Yield.Value;
            work *= mp.Work.Value;
        }

        // scale up work if extremely low yield (example 'components')
        if (yield < 1f)
        {
            work /= yield;
            yield = 1f;
        }

        var iYield = (int)Math.Round(yield);

        recipe.workAmount = work;
        recipe.description = LanguageKeys.keyed.ocd_description.Translate(sourceLabel, iYield, material.LabelCap);
        recipe.products.First().count = iYield;
    }

    public static void UpdateAllGeneratedDefs()
    {
        foreach (var miningRecipe in _miningRecipes)
        {
            UpdateGeneratedDef(miningRecipe.Key, miningRecipe.Value, _drillProperties[miningRecipe.Key],
                _sourceLabels[miningRecipe.Key]);
        }
    }
}
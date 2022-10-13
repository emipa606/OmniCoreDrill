using System;
using RimWorld;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill;

internal class DrillingProperties
{
    private const float workPerHitpoint = 2f;
    private const float densityFactor = 40f;
    private const float commonalityFactor = 2000;

    private const float minScatterCommonality = 0.001f;

    private static GlobalDrillParameters _params;

    private readonly BuildingProperties _buildingProps;

    protected readonly float _hitpointsPerLump;


    public DrillingProperties(float hitpointsPerLump, BuildingProperties buildingProps)
    {
        _hitpointsPerLump = hitpointsPerLump;
        _buildingProps = buildingProps;
    }


    private static float WorkPerHitpoint => workPerHitpoint * DrillParameters.DrillWork.Value;
    private static float DensityFactor => densityFactor / DrillParameters.Density.Value;
    private static float CommonalityFactor => commonalityFactor * DrillParameters.Commonality.Value;

    private static GlobalDrillParameters DrillParameters => _params ??
                                                            (_params = LoadedModManager.GetMod<OmniCoreDrillMod>()
                                                                .Settings.GlobalParameters);

    public float Work => (_hitpointsPerLump * WorkPerHitpoint)
                         + (_buildingProps.isResourceRock
                             ? (float)(CommonalityFactor /
                                       Math.Sqrt(Math.Max(_buildingProps.mineableScatterCommonality,
                                           minScatterCommonality)))
                             : 0);

    public float Yield => _buildingProps.isResourceRock
        ? _buildingProps.mineableScatterLumpSizeRange.Average * _buildingProps.mineableYield / DensityFactor
        : 1f;
}
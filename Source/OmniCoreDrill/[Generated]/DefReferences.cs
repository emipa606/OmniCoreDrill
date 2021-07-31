using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill
{
    [GeneratedCode("Defs.Generated.tt", "0.1")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ConvertToAutoProperty")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    public class DefReferences
    {
        private static ThingDef _Thing_CoreDrill = DefDatabase<ThingDef>.GetNamed(DefNames.Thing_CoreDrill);

        private static ThingDef _Thing_UnfinishedDrillingPlan =
            DefDatabase<ThingDef>.GetNamed(DefNames.Thing_UnfinishedDrillingPlan);

        public static ThingDef Thing_CoreDrill
        {
            get { return _Thing_CoreDrill; }
        }

        public static ThingDef Thing_UnfinishedDrillingPlan
        {
            get { return _Thing_UnfinishedDrillingPlan; }
        }
    }
}
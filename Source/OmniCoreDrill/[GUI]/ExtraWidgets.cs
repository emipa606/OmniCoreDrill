using System;
using UnityEngine;
using Verse;

namespace DoctorVanGogh.OmniCoreDrill
{
    public class ExtraWidgets
    {
        public static float LogarithmicScaleSlider(Rect rect, float value, float minValue, float maxValue,
            Func<float, string> valueFormatter, string leftAlignedLabel = null, string rightAlignedLabel = null)
        {
            return (float) Math.Exp(Widgets.HorizontalSlider(rect, (float) Math.Log(value), minValue, maxValue, true,
                valueFormatter(value), leftAlignedLabel, rightAlignedLabel));
        }
    }
}
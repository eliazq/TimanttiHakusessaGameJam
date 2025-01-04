#if UNITY_EDITOR
using System;
using System.Globalization;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal static class PicEaseLite_Color
    {
        #region Properties
        public static readonly Color PrimaryFontColor = HexToColor("#FF4DAB");
        public static readonly Color PrimaryFontColorFaded = HexToColor("#FF4DAB80");
        public static readonly Color SecondaryFontColor = HexToColor("#FFFFFF");
        public static readonly Color TertiaryFontColor = HexToColor("#FDE74C");
        public static readonly Color QuartiaryFontColor = HexToColor("#FFFFFF7E");
        public static readonly Color TooltipFontColor = HexToColor("#E7E7E7");
        public static readonly Color PrimaryPanelColorTop = HexToColor("#005789");
        public static readonly Color PrimaryPanelColorMiddle = HexToColor("#121119");
        public static readonly Color PrimaryPanelColorBottom = HexToColor("#101010");
        public static readonly Color SecondaryPanelColor = HexToColor("#00000055");
        public static readonly Color TertiaryPanelColorTop = HexToColor("#00000055");
        public static readonly Color TertiaryPanelColorMiddle = HexToColor("#00000055");
        public static readonly Color TertiaryPanelColorBottom = HexToColor("#FF4DAB55");
        public static readonly Color QuartiaryPanelColor = HexToColor("#00000032");
        #endregion

        #region Methods
        public static Color HexToColor(string hex)
        {
            try
            {
                hex = hex.Replace("0x", "").Replace("#", "");
                byte a = 255;
                byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }
                return new Color32(r, g, b, a);
            }
            catch (Exception ex)
            {
                Debug.LogError("Color parsing failed: " + ex.Message);
                return Color.white;
            }
        }
        #endregion
    }
}
#endif
#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal static class PicEaseLite_Resources
    {
        private static class ResourceNames
        {
            #region Fonts
            internal const string FontBold = "PicEase Lite Font Bold";
            internal const string FontRegular = "PicEase Lite Font Regular";
            internal const string FontSemiBold = "PicEase Lite Font Semi Bold";
            #endregion

            #region Icons
            internal const string IconAdjustments = "PicEase Lite Icon Adjustments";
            internal const string IconFilters = "PicEase Lite Icon Filters";
            internal const string IconReset = "PicEase Lite Icon Reset";
            internal const string IconTooltip = "PicEase Lite Icon Tooltip";
            #endregion

            #region Graphics
            internal const string GraphicsTitle = "PicEase Lite Graphics Title";
            internal const string GraphicsOverlay = "PicEase Lite Graphics Overlay";
            internal const string GraphicsBackground = "PicEase Lite Graphics Background";
            #endregion

            #region Promotional
            internal const string PromotionalPicEase = "PicEase Lite Promotional PicEase";
            internal const string PromotionalHierarchyDesigner = "PicEase Lite Promotional Hierarchy Designer";
            #endregion
        }

        #region Classes
        internal static class Fonts
        {
            private static readonly Lazy<Font> _bold = new(() => PicEaseLite_Texture.LoadFont(ResourceNames.FontBold));
            public static Font Bold => _bold.Value;

            private static readonly Lazy<Font> _regular = new(() => PicEaseLite_Texture.LoadFont(ResourceNames.FontRegular));
            public static Font Regular => _regular.Value;

            private static readonly Lazy<Font> _semiBold = new(() => PicEaseLite_Texture.LoadFont(ResourceNames.FontSemiBold));
            public static Font SemiBold => _semiBold.Value;
        }

        internal static class Icons
        {
            private static readonly Lazy<Texture2D> _adjustmentsIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.IconAdjustments));
            public static Texture2D Adjustments => _adjustmentsIcon.Value;

            private static readonly Lazy<Texture2D> _filtersIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.IconFilters));
            public static Texture2D Filters => _filtersIcon.Value;

            private static readonly Lazy<Texture2D> _resetIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.IconReset));
            public static Texture2D Reset => _resetIcon.Value;

            private static readonly Lazy<Texture2D> _tooltipIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.IconTooltip));
            public static Texture2D Tooltip => _tooltipIcon.Value;
        }

        internal static class Graphics
        {
            private static readonly Lazy<Texture2D> _title = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.GraphicsTitle));
            public static Texture2D Title => _title.Value;

            private static readonly Lazy<Texture2D> _overlay = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.GraphicsOverlay));
            public static Texture2D Overlay => _overlay.Value;

            private static readonly Lazy<Texture2D> _background = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.GraphicsBackground));
            public static Texture2D Background => _background.Value;
        }

        internal static class Promotional
        {
            private static readonly Lazy<Texture2D> _picEasePromotionalIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.PromotionalPicEase));
            public static Texture2D PicEasePromotionalIcon => _picEasePromotionalIcon.Value;

            private static readonly Lazy<Texture2D> _hierarchyDesignerPromotionalIcon = new(() => PicEaseLite_Texture.LoadTexture(ResourceNames.PromotionalHierarchyDesigner));
            public static Texture2D HierarchyDesignerPromotionalIcon => _hierarchyDesignerPromotionalIcon.Value;
        }
        #endregion
    }
}
#endif
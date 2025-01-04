#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal class PicEaseLite_GUI
    {
        #region Properties
        #region Labels
        private static readonly Lazy<GUIStyle> _titleLabelStyle = new(() => new(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            padding = new(0, 0, 0, 0)
        });
        public static GUIStyle TitleLabelStyle => _titleLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _versionLabelStyle = new(() => new()
        {
            stretchHeight = true,
            font = PicEaseLite_Resources.Fonts.SemiBold,
            fontSize = 12,
            fontStyle = FontStyle.Italic,
            alignment = TextAnchor.UpperCenter,
            normal = { textColor = PicEaseLite_Color.QuartiaryFontColor }
        });
        public static GUIStyle VersionLabelStyle => _versionLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _regularLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Regular,
            fontSize = 13,
            wordWrap = true,
            richText = true,
            normal = { textColor = PicEaseLite_Color.SecondaryFontColor }
        });
        public static GUIStyle RegularLabelStyle => _regularLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _boldLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 16,
            alignment = TextAnchor.UpperLeft,
            wordWrap = false,
            normal = { textColor = PicEaseLite_Color.PrimaryFontColor }
        });
        public static GUIStyle BoldLabelStyle => _boldLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _miniBoldLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 14,
            wordWrap = false,
            normal = { textColor = PicEaseLite_Color.TertiaryFontColor }
        });
        public static GUIStyle MiniBoldLabelStyle => _miniBoldLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _layoutLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Regular,
            fontSize = 13,
            wordWrap = false,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = PicEaseLite_Color.SecondaryFontColor }
        });
        public static GUIStyle LayoutLabelStyle => _layoutLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _buttonLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 11,
            wordWrap = false,
            clipping = TextClipping.Overflow,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = PicEaseLite_Color.SecondaryFontColor }
        });
        public static GUIStyle ButtonLabelStyle => _buttonLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _dragAndDropLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.SemiBold,
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEaseLite_Color.PrimaryFontColor }
        });
        public static GUIStyle DragAndDropLabelStyle => _dragAndDropLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _categoryLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEaseLite_Color.SecondaryFontColor }
        });
        public static GUIStyle CategoryLabelStyle => _categoryLabelStyle.Value;

        private static readonly Lazy<GUIStyle> _previewLabelStyleLabelStyle = new(() => new()
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 17,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = false,
            normal = { textColor = PicEaseLite_Color.PrimaryFontColor }
        });
        public static GUIStyle PreviewLabelStyle => _previewLabelStyleLabelStyle.Value;
        #endregion

        #region Panels
        private static readonly Lazy<GUIStyle> _primaryPanelStyle = new(() => new(GUI.skin.box)
        {
            stretchWidth = true,
            stretchHeight = true,
            border = new(-5, -5, -5, -5),
            normal = { background = PicEaseLite_Texture.CreateGradientTexture(2, 256, new Color[] { PicEaseLite_Color.PrimaryPanelColorBottom, PicEaseLite_Color.PrimaryPanelColorMiddle, PicEaseLite_Color.PrimaryPanelColorTop}) }
        });
        public static GUIStyle PrimaryPanelStyle => _primaryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _secondaryPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 10),
            margin = new(0, 0, 0, 0),
            normal = { background = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.SecondaryPanelColor) }
        });
        public static GUIStyle SecondaryPanelStyle => _secondaryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _menuButtonsPanelStyle = new(() => new(GUI.skin.box)
        {
            stretchWidth = true,
            fixedHeight = 30,
            normal = { background = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.SecondaryPanelColor) }
        });
        public static GUIStyle MenuButtonsPanelStyle => _menuButtonsPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _footerPanelStyle = new(() => new()
        {
            stretchWidth = true,
            fixedHeight = 16,
            padding = new(2, 2, 2, 2),
            margin = new(0, 0, 0, 0),
        });
        public static GUIStyle FooterPanelStyle => _footerPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _dragAndDropPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 10),
            normal = { background = PicEaseLite_Texture.CreateGradientTexture(2, 256, new Color[] { PicEaseLite_Color.TertiaryPanelColorBottom, PicEaseLite_Color.TertiaryPanelColorMiddle,  PicEaseLite_Color.TertiaryPanelColorTop }) }
        });
        public static GUIStyle DragAndDropPanelStyle => _dragAndDropPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorLeftPanelStyle = new(() => new(GUI.skin.box)
        {
            fixedWidth = 300,
            stretchHeight = true,
            padding = new(10, 10, 10, 10),
            margin = new(10, 10, 10, 10),
            normal = { background = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.SecondaryPanelColor) }
        });
        public static GUIStyle ImageEditorLeftPanelStyle => _imageEditorLeftPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorRightPanelStyle = new(() => new(GUI.skin.box)
        {
            padding = new(10, 10, 10, 12),
            margin = new(10, 10, 10, 10),
            stretchWidth = true,
            stretchHeight = true,
            normal = { background = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.SecondaryPanelColor) }
        });
        public static GUIStyle ImageEditorRightPanelStyle => _imageEditorRightPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _imageEditorCategoryPanelStyle = new(() => new(GUI.skin.box)
        {
            fixedHeight = 64,
            padding = new(10, 10, 7, 0),
            normal = { background = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.QuartiaryPanelColor) }
        });
        public static GUIStyle ImageEditorCategoryPanelStyle => _imageEditorCategoryPanelStyle.Value;

        private static readonly Lazy<GUIStyle> _tooltipPanel = new(() => new(GUI.skin.box)
        {
            padding = new(2, 2, 2, 2),
            wordWrap = true,
            normal = { background = Texture2D.whiteTexture, textColor = Color.black }
        });
        public static GUIStyle TooltipPanel => _tooltipPanel.Value;
        #endregion

        #region Buttons
        private static readonly Lazy<GUIStyle> _menuButtonStyle = new(() => new(GUI.skin.button)
        {
            font = PicEaseLite_Resources.Fonts.Bold,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            fixedHeight = 25,
            alignment = TextAnchor.MiddleCenter,
            normal = new()
            {
                textColor = PicEaseLite_Color.SecondaryFontColor,
                background = PicEaseLite_Texture.ClearTexture
            },
            hover = new()
            {
                textColor = PicEaseLite_Color.PrimaryFontColor,
                background = PicEaseLite_Texture.ClearTexture
            },
            active = new()
            {
                textColor = PicEaseLite_Color.PrimaryFontColorFaded,
                background = PicEaseLite_Texture.ClearTexture
            }
        });
        public static GUIStyle MenuButtonStyle => _menuButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _adjustmentsButtonStyle = new(() => CreateButtonIconStyle(PicEaseLite_Resources.Icons.Adjustments));
        public static GUIStyle AdjustmentsButtonStyle => _adjustmentsButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _filtersButtonStyle = new(() => CreateButtonIconStyle(PicEaseLite_Resources.Icons.Filters));
        public static GUIStyle FiltersButtonStyle => _filtersButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _resetButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 25,
            fixedWidth = 25,
            overflow = new(3, 3, 2, 2),
            margin = new(0, 0, -2, 0),
            normal = { background = PicEaseLite_Resources.Icons.Reset }
        });
        public static GUIStyle ResetButtonStyle => _resetButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _tooltipButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 25,
            fixedWidth = 16,
            overflow = new(9, 7, 2, 2),
            margin = new(0, 0, -2, 0),
            normal = { background = PicEaseLite_Resources.Icons.Tooltip }
        });
        public static GUIStyle TooltipButtonStyle => _tooltipButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _promotionalPicEaseButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 64,
            fixedWidth = 64,
            alignment = TextAnchor.MiddleLeft,
            normal = { background = PicEaseLite_Resources.Promotional.PicEasePromotionalIcon }
        });
        public static GUIStyle PromotionaPicEaselButtonStyle => _promotionalPicEaseButtonStyle.Value;

        private static readonly Lazy<GUIStyle> _promotionalHierarchyDesignerButtonStyle = new(() => new(GUI.skin.button)
        {
            fixedHeight = 64,
            fixedWidth = 64,
            alignment = TextAnchor.MiddleLeft,
            normal = { background = PicEaseLite_Resources.Promotional.HierarchyDesignerPromotionalIcon }
        });
        public static GUIStyle PromotionaHierarchyDesignerlButtonStyle => _promotionalHierarchyDesignerButtonStyle.Value;
        #endregion

        #region Modifiables
        private static Lazy<GUIStyle> _infoLabelStyle = new(() => CreateInfoLabelStyle());
        public static GUIStyle InfoLabelStyle => _infoLabelStyle.Value;
        #endregion
        #endregion

        #region Classes
        private sealed class TooltipPopup : PopupWindowContent
        {
            #region Properties
            private readonly string tooltipText;
            private static readonly GUIStyle style;
            private static readonly Texture2D backgroundTexture;
            private const float maxWidth = 200f;
            private const int fontSize = 12;
            #endregion

            #region Constructor
            static TooltipPopup()
            {
                backgroundTexture = PicEaseLite_Texture.CreateTexture(2, 2, PicEaseLite_Color.QuartiaryPanelColor);
                style = new(EditorStyles.helpBox)
                {
                    font = PicEaseLite_Resources.Fonts.Regular,
                    fontSize = fontSize,
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    padding = new(2, 2, 2, 3),
                    normal =
                    {
                        textColor = PicEaseLite_Color.TooltipFontColor,
                        background = backgroundTexture
                    }
                };
            }
            #endregion

            #region Accessor
            public TooltipPopup(string text)
            {
                tooltipText = text;
            }
            #endregion

            #region Override Methods
            public override void OnGUI(Rect rect)
            {
                GUILayout.Label(tooltipText, style);
            }

            public override Vector2 GetWindowSize()
            {
                GUIContent content = new(tooltipText);
                float height = style.CalcHeight(content, maxWidth);
                return new(maxWidth, height);
            }
            #endregion
        }
        #endregion

        #region Helpers
        private static GUIStyle CreateButtonIconStyle(Texture2D icon)
        {
            return new(GUI.skin.button)
            {
                fixedHeight = 32,
                fixedWidth = 32,
                normal = { background = icon }
            };
        }
        #endregion

        #region Methods
        public static float DrawFloatSlider(string label, float value, float leftValue, float rightValue, float defaultValue)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(label, RegularLabelStyle, GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            float newValue = EditorGUILayout.Slider(value, leftValue, rightValue);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            return newValue;
        }

        public static int DrawIntSlider(string label, float labelWidth, int value, int leftValue, int rightValue, int defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            int newValue = EditorGUILayout.IntSlider(value, leftValue, rightValue, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static int DrawIntField(string label, float labelWidth, int value, int defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            EditorGUILayout.BeginHorizontal();
            int newValue = EditorGUILayout.IntField(value);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static string DrawStringField(string label, float labelWidth, string value, string defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            EditorGUILayout.BeginHorizontal();
            string newValue = EditorGUILayout.TextField(value);
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static bool DrawToggle(string label, float labelWidth, bool value, bool defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            bool newValue = EditorGUILayout.Toggle(value, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newValue = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static int DrawPopup(string label, float labelWidth, int selectedIndex, string[] options, int defaultValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));

            int newIndex = EditorGUILayout.Popup(selectedIndex, options, GUILayout.ExpandWidth(true));
            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newIndex = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newIndex;
        }

        public static Color DrawColorField(string label, float labelWidth, Color value, Color defaultValue, bool showTooltip = false, string tooltipText = "")
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            Color newColor = EditorGUILayout.ColorField(value, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newColor = defaultValue;
            }

            EditorGUILayout.EndHorizontal();
            return newColor;
        }

        public static T DrawEnumPopup<T>(string label, float labelWidth, T selectedEnum, T defaultEnum, bool showTooltip = false, string tooltipText = "") where T : Enum
        {
            EditorGUILayout.BeginHorizontal();

            if (showTooltip)
            {
                DrawTooltip(tooltipText);
            }

            EditorGUILayout.LabelField(label, LayoutLabelStyle, GUILayout.Width(labelWidth));
            T newEnum = (T)EditorGUILayout.EnumPopup(selectedEnum, GUILayout.ExpandWidth(true));

            GUILayout.Space(3);
            if (GUILayout.Button(string.Empty, ResetButtonStyle))
            {
                newEnum = defaultEnum;
            }

            EditorGUILayout.EndHorizontal();
            return newEnum;
        }

        public static bool DrawButtonIcon(string label, GUIStyle buttonStyle)
        {
            float width = buttonStyle.fixedWidth;

            EditorGUILayout.BeginVertical(GUILayout.Width(width));
            bool isClicked = GUILayout.Button(string.Empty, buttonStyle, GUILayout.Width(width), GUILayout.Height(buttonStyle.fixedHeight));
            GUILayout.Space(2);
            EditorGUILayout.LabelField(label, ButtonLabelStyle, GUILayout.Width(width));
            EditorGUILayout.EndVertical();

            return isClicked;
        }
        #endregion

        #region Operations
        private static void DrawTooltip(string tooltipText)
        {
            Rect buttonRect = GUILayoutUtility.GetRect(TooltipButtonStyle.fixedWidth, TooltipButtonStyle.fixedHeight, TooltipButtonStyle, GUILayout.ExpandWidth(false));
            if (GUI.Button(buttonRect, GUIContent.none, TooltipButtonStyle))
            {
                Rect popupRect = new(buttonRect.x, buttonRect.y + buttonRect.height + 4, 0, 0);
                PopupWindow.Show(popupRect, new TooltipPopup(tooltipText));
            }
            GUILayout.Space(2);
        }
        #endregion

        #region Modifiables
        private static GUIStyle CreateInfoLabelStyle()
        {
            return new()
            {
                font = PicEaseLite_Resources.Fonts.Regular,
                fontSize = PicEaseLite_Settings.ImageEditorInfoLabelFontSize,
                fixedHeight = PicEaseLite_Window.ImageInfoLabelHeight,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = PicEaseLite_Settings.ImageEditorInfoLabelFontColor }
            };
        }

        public static void RefreshInfoLabelStyle()
        {
            _infoLabelStyle = new(() => CreateInfoLabelStyle());
        }
        #endregion
    }
}
#endif
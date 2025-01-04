#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Verpha.PicEase.Lite
{
    internal static class PicEaseLite_Settings
    {
        #region Properties
        [System.Serializable]
        private class PicEase_Settings
        {
            public FilterMode imageEditorDefaultTextureFilterMode = FilterMode.Bilinear;
            public int imageEditorDefaultTextureAnisoLevel = 1;
            public PicEaseLite_Converter.ExportImageFormat imageEditorExportImageFormat = PicEaseLite_Converter.ExportImageFormat.OriginalImage; 
            public PicEaseLite_Converter.ImageFormat imageEditorDefaultExportImageFormat = PicEaseLite_Converter.ImageFormat.PNG;
            public bool imageEditorSameExportPath = true;
            public string imageEditorDefaultExportDirectory = "";
            public bool imageEditorEnableImageInfoDisplay = true;
            public int imageEditorInfoLabelFontSize = 12;
            public Color imageEditorInfoLabelFontColor = Color.white;
            public Color imageEditorInfoBackgroundColor = new(0f, 0f, 0f, 0.5f);
            public bool imageEditorEnableDebugLogForExportedFiles = true;
            public PicEaseLite_Image.BehaviourMode imageEditorBehaviourMode = PicEaseLite_Image.BehaviourMode.Synced;
            public PicEaseLite_Image.ThreadGroupSize imageEditorThreadGroupSize = PicEaseLite_Image.ThreadGroupSize.SixteenBySixteen;
            public PicEaseLite_Image.PrecisionMode imageEditorPrecisionMode = PicEaseLite_Image.PrecisionMode.Full;
            public int sceneScreenshotDefaultImageWidth = 1920;
            public int sceneScreenshotDefaultImageHeight = 1080;
            public PicEaseLite_Converter.ImageFormat sceneScreenshotDefaultExportImageFormat = PicEaseLite_Converter.ImageFormat.PNG;
            public string sceneScreenshotDefaultExportDirectory = "";
        }
        private static PicEase_Settings settings = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();
            PicEaseLite_Image.EnsureComputeShaderProperties();
        }
        #endregion

        #region Accessors
        #region Image Editor
        public static FilterMode ImageEditorDefaultImportTextureFilterMode
        {
            get => settings.imageEditorDefaultTextureFilterMode;
            set
            {
                if (settings.imageEditorDefaultTextureFilterMode != value)
                {
                    settings.imageEditorDefaultTextureFilterMode = value;
                }
            }
        }

        public static int ImageEditorDefaultImportTextureAnisoLevel
        {
            get => settings.imageEditorDefaultTextureAnisoLevel;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, 16);
                if (settings.imageEditorDefaultTextureAnisoLevel != clampedValue)
                {
                    settings.imageEditorDefaultTextureAnisoLevel = clampedValue;
                }
            }
        }

        public static PicEaseLite_Converter.ExportImageFormat ImageEditorExportImageFormat
        {
            get => settings.imageEditorExportImageFormat;
            set
            {
                if (settings.imageEditorExportImageFormat != value)
                {
                    settings.imageEditorExportImageFormat = value;
                }
            }
        }

        public static PicEaseLite_Converter.ImageFormat ImageEditorDefaultExportImageFormat
        {
            get => settings.imageEditorDefaultExportImageFormat;
            set
            {
                if (settings.imageEditorDefaultExportImageFormat != value)
                {
                    settings.imageEditorDefaultExportImageFormat = value;
                }
            }
        }

        public static bool ImageEditorSameExportPath
        {
            get => settings.imageEditorSameExportPath;
            set
            {
                if (settings.imageEditorSameExportPath != value)
                {
                    settings.imageEditorSameExportPath = value;
                }
            }
        }

        public static string ImageEditorDefaultExportDirectory
        {
            get => settings.imageEditorDefaultExportDirectory;
            set
            {
                if (settings.imageEditorDefaultExportDirectory != value)
                {
                    settings.imageEditorDefaultExportDirectory = value;
                }
            }
        }

        public static bool ImageEditorEnableImageInfoDisplay
        {
            get => settings.imageEditorEnableImageInfoDisplay;
            set
            {
                if (settings.imageEditorEnableImageInfoDisplay != value)
                {
                    settings.imageEditorEnableImageInfoDisplay = value;
                }
            }
        }

        public static int ImageEditorInfoLabelFontSize
        {
            get => settings.imageEditorInfoLabelFontSize;
            set
            {
                int clampedValue = Mathf.Clamp(value, 7, 21);
                if (settings.imageEditorInfoLabelFontSize != clampedValue)
                {
                    settings.imageEditorInfoLabelFontSize = clampedValue;
                    PicEaseLite_GUI.RefreshInfoLabelStyle();
                }
            }
        }

        public static Color ImageEditorInfoLabelFontColor
        {
            get => settings.imageEditorInfoLabelFontColor;
            set
            {
                if (settings.imageEditorInfoLabelFontColor != value)
                {
                    settings.imageEditorInfoLabelFontColor = value;
                    PicEaseLite_GUI.RefreshInfoLabelStyle();
                }
            }
        }

        public static Color ImageEditorInfoBackgroundColor
        {
            get => settings.imageEditorInfoBackgroundColor;
            set
            {
                if (settings.imageEditorInfoBackgroundColor != value)
                {
                    settings.imageEditorInfoBackgroundColor = value;
                    PicEaseLite_GUI.RefreshInfoLabelStyle();
                }
            }
        }

        public static bool ImageEditorEnableDebugLogForExportedFiles
        {
            get => settings.imageEditorEnableDebugLogForExportedFiles;
            set
            {
                if (settings.imageEditorEnableDebugLogForExportedFiles != value)
                {
                    settings.imageEditorEnableDebugLogForExportedFiles = value;
                }
            }
        }

        public static PicEaseLite_Image.BehaviourMode ImageEditorBehaviourMode
        {
            get => settings.imageEditorBehaviourMode;
            set
            {
                if (settings.imageEditorBehaviourMode != value)
                {
                    settings.imageEditorBehaviourMode = value;
                }
            }
        }

        public static PicEaseLite_Image.ThreadGroupSize ImageEditorThreadGroupSize
        {
            get => settings.imageEditorThreadGroupSize;
            set
            {
                if (settings.imageEditorThreadGroupSize != value)
                {
                    settings.imageEditorThreadGroupSize = value;
                    PicEaseLite_Image.EnsureComputeShaderProperties();
                }
            }
        }

        public static PicEaseLite_Image.PrecisionMode ImageEditorPrecisionMode
        {
            get => settings.imageEditorPrecisionMode;
            set
            {
                if (settings.imageEditorPrecisionMode != value)
                {
                    settings.imageEditorPrecisionMode = value;
                    PicEaseLite_Image.EnsureComputeShaderProperties();
                }
            }
        }
        #endregion

        #region Scene Screenshot
        public static int SceneScreenshotDefaultImageWidth
        {
            get => settings.sceneScreenshotDefaultImageWidth;
            set
            {
                if (settings.sceneScreenshotDefaultImageWidth != value)
                {
                    settings.sceneScreenshotDefaultImageWidth = value;
                }
            }
        }

        public static int SceneScreenshotDefaultImageHeight
        {
            get => settings.sceneScreenshotDefaultImageHeight;
            set
            {
                if (settings.sceneScreenshotDefaultImageHeight != value)
                {
                    settings.sceneScreenshotDefaultImageHeight = value;
                }
            }
        }

        public static PicEaseLite_Converter.ImageFormat SceneScreenshotDefaultExportImageFormat
        {
            get => settings.sceneScreenshotDefaultExportImageFormat;
            set
            {
                if (settings.sceneScreenshotDefaultExportImageFormat != value)
                {
                    settings.sceneScreenshotDefaultExportImageFormat = value;
                }
            }
        }

        public static string SceneScreenshotDefaultExportDirectory
        {
            get => settings.sceneScreenshotDefaultExportDirectory;
            set
            {
                if (settings.sceneScreenshotDefaultExportDirectory != value)
                {
                    settings.sceneScreenshotDefaultExportDirectory = value;
                }
            }
        }
        #endregion
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = PicEaseLite_File.GetSavedDataFilePath(PicEaseLite_Constants.SettingsTextFileName);
            string json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = PicEaseLite_File.GetSavedDataFilePath(PicEaseLite_Constants.SettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                PicEase_Settings loadedSettings = JsonUtility.FromJson<PicEase_Settings>(json);
                settings = loadedSettings;
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            settings = new()
            {
                imageEditorDefaultTextureFilterMode = FilterMode.Bilinear,
                imageEditorDefaultTextureAnisoLevel = 1,
                imageEditorExportImageFormat = PicEaseLite_Converter.ExportImageFormat.OriginalImage,
                imageEditorDefaultExportImageFormat = PicEaseLite_Converter.ImageFormat.PNG,
                imageEditorSameExportPath = true,
                imageEditorDefaultExportDirectory = "",
                imageEditorEnableImageInfoDisplay = true,
                imageEditorInfoLabelFontSize = 12,
                imageEditorInfoLabelFontColor = Color.white,
                imageEditorInfoBackgroundColor = new(0f, 0f, 0f, 0.5f),
                imageEditorEnableDebugLogForExportedFiles = true,
                imageEditorBehaviourMode = PicEaseLite_Image.BehaviourMode.Synced,
                imageEditorThreadGroupSize = PicEaseLite_Image.ThreadGroupSize.SixteenBySixteen,
                imageEditorPrecisionMode = PicEaseLite_Image.PrecisionMode.Full,
                sceneScreenshotDefaultImageWidth = 1920,
                sceneScreenshotDefaultImageHeight = 1080,
                sceneScreenshotDefaultExportImageFormat = PicEaseLite_Converter.ImageFormat.PNG,
                sceneScreenshotDefaultExportDirectory = ""
            };
        }
        #endregion
    }
}
#endif
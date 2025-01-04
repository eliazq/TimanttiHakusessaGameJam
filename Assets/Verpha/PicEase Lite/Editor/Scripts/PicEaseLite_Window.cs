#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal class PicEaseLite_Window : EditorWindow
    {
        #region Properties
        #region General
        #region Layout
        private Rect screenRect;
        private Vector2 scrollHomeBody;
        private Vector2 scrollAboutWelcomePanel;
        private Vector2 scrollAboutPatchNotes;
        private Vector2 scrollAboutPromotional;
        private Vector2 scrollSettingsBody;
        private Vector2 scrollImageEditorAdjustments;
        private Vector2 scrollImageEditorFilters;
        private const float spacingMenuButtons = 20f;
        private const float labelWidthSettings = 200f;
        private const float labelWidthSceneScreenshot = 95f;
        #endregion

        #region View Mode
        private enum ViewMode { Home, About, Settings, ImageEditor, SceneScreenshot }
        private ViewMode currentViewMode = ViewMode.Home;
        #endregion
        #endregion

        #region Home
        private const float titleAspectRatio = 0.5f;
        private const float titleWidthPercentage = 0.8f;
        private const float titleMinWidth = 300f;
        private const float titleMaxWidth = 1024f;
        private const float titleMinHeight = 150f;
        private const float titleMaxHeight = 512f;
        #endregion

        #region About
        private const string imageEditorText =
            "The Image Editor allows you to easily modify and enhance images, such as textures, sprites, or any other type of image.\n";

        private const string sceneScreenshotText =
            "The Scene Screenshot allows you to quickly capture screenshots from any scene's camera view.\n";

        private const string additionalNotesText =
            "PicEase Lite is a free, simplified version of PicEase. It will always include approximately 30% to 50% of PicEase's features, but never more.\n\n" +
            "PicEase Lite will receive more updates as PicEase continues to grow.\n\n" +
            "PicEase and its Lite version are editor-only tools, meaning they will not be included in your build.\n" +
            "It also has no dependencies, such as packages, external libraries, prefabs, and so on.\n\n" +
            "If you have any questions, would like to report a bug, or have suggestions for improvements,\n" +
            "you may email me at: VerphaSuporte@outlook.com\n";

        private const string picEaseFullVersion = "The full version of PicEase includes the following additional features:\n\n" +
            "<b>Image Editor:</b>\n" +
            "• More adjustments.\n" +
            "• More filters.\n" +
            "• Shortcuts.\n" +
            "• Colorize: Easily replace the colors in your images.\n" +
            "• Save Custom Filters: Save your adjustment values as custom filters for quick reuse.\n" +
            "• .tiff File Format Support.\n\n" +
            "<b>Map Generator:</b>\n" +
            "Generate texture maps using only an albedo/base/diffuse map as the source.\n\n" +
            "More features coming soon!";

        private string patchNotes = string.Empty;
        #endregion

        #region Settings
        #region Image Editor
        private FilterMode imageEditorDefaultTextureFilterMode;
        private int imageEditorDefaultTextureAnisoLevel;
        private PicEaseLite_Converter.ExportImageFormat imageEditorExportImageFormat;
        private PicEaseLite_Converter.ImageFormat imageEditorDefaultExportImageFormat;
        private bool imageEditorSameExportPath;
        private string imageEditorDefaultExportDirectory;
        private bool imageEditorEnableImageInfoDisplay;
        private int imageEditorInfoLabelFontSize;
        private Color imageEditorInfoLabelFontColor;
        private Color imageEditorInfoBackgroundColor;
        private bool imageEditorEnableDebugLogForExportedFiles;
        PicEaseLite_Image.BehaviourMode imageEditorBehaviourMode;
        PicEaseLite_Image.ThreadGroupSize imageEditorThreadGroupSize;
        PicEaseLite_Image.PrecisionMode imageEditorPrecisionMode;
        #endregion

        #region Scene Screenshot
        private int sceneScreenshotDefaultImageWidth;
        private int sceneScreenshotDefaultImageHeight;
        private PicEaseLite_Converter.ImageFormat sceneScreenshotDefaultExportImageFormat;
        private string sceneScreenshotDefaultExportDirectory;
        #endregion
        #endregion

        #region Image Editor
        private enum ImageEditorCategory { Adjustments, Filters }
        private ImageEditorCategory currentImageEditorCategory = ImageEditorCategory.Adjustments;
        private string cachedCategoryLabel;

        private Texture2D originalImage;
        private TextureImporterSettings originalImageImporterSettings;
        private int originalMaxTextureSize;
        private TextureImporterCompression originalTextureCompression;
        private ComputeShader computeShader;
        private PicEaseLite_Image imageEffectsManager;

        #region Adjustments
        private const float adjustmentSliderDefaultValue = 0f;
        private const float adjustmentSliderMinValue = -100f;
        private const float adjustmentSliderMaxValue = 100f;
        #endregion

        private bool isComparing = false;
        private bool isComparingDoOnce = false;
        private string currentLoadedImageName = "Image";
        private string currentImagePath = string.Empty;
        private string currentImageFormat = "Unknown";
        private int currentImageWidth = 0;
        private int currentImageHeight = 0;
        private string currentImageDirectory = string.Empty;
        private PicEaseLite_Converter.ImageFormat imageEditorCurrentExportFormat;

        private const float imageInfoLabelPadding = 2f;
        internal const float ImageInfoLabelHeight = 25f;
        #endregion

        #region Scene Screenshot
        private Camera[] availableCameras;
        private string[] cameraNames;
        private int selectedCameraIndex = 0;

        private Texture2D sceneScreenshot;
        private Texture2D exportScreenshot;

        private int sceneScreenshotCurrentImageWidth;
        private int sceneScreenshotCurrentImageHeight;

        private bool hdrExport = false;
        private PicEaseLite_Converter.ImageFormat sceneScreenshotCurrentExportFormat;
        #endregion
        #endregion

        [MenuItem(PicEaseLite_Constants.MenuLocation, false, PicEaseLite_Constants.MenuPriority)]
        private static void OpenWindow()
        {
            PicEaseLite_Window editorWindow = GetWindow<PicEaseLite_Window>(PicEaseLite_Constants.AssetName);
            editorWindow.minSize = new(600, 400);
        }

        #region Initialization
        private void OnEnable()
        {
            InitializeHome();
            InitializeSettings();
            InitializeImageEditor();
            InitializeSceneScreenshot();
        }

        private void InitializeHome()
        {
            if (!PicEaseLite_Session.instance.IsPatchNotesLoaded)
            {
                patchNotes = PicEaseLite_File.GetPatchNotesData();
                PicEaseLite_Session.instance.PatchNotesContent = patchNotes;
                PicEaseLite_Session.instance.IsPatchNotesLoaded = true;
            }
            else
            {
                patchNotes = PicEaseLite_Session.instance.PatchNotesContent;
            }
        }

        private void InitializeSettings()
        {
            PicEaseLite_Settings.LoadSettings();
            imageEditorDefaultTextureFilterMode = PicEaseLite_Settings.ImageEditorDefaultImportTextureFilterMode;
            imageEditorDefaultTextureAnisoLevel = PicEaseLite_Settings.ImageEditorDefaultImportTextureAnisoLevel;
            imageEditorExportImageFormat = PicEaseLite_Settings.ImageEditorExportImageFormat;
            imageEditorDefaultExportImageFormat = PicEaseLite_Settings.ImageEditorDefaultExportImageFormat;
            imageEditorSameExportPath = PicEaseLite_Settings.ImageEditorSameExportPath;
            imageEditorDefaultExportDirectory = PicEaseLite_Settings.ImageEditorDefaultExportDirectory;
            imageEditorEnableImageInfoDisplay = PicEaseLite_Settings.ImageEditorEnableImageInfoDisplay;
            imageEditorBehaviourMode = PicEaseLite_Settings.ImageEditorBehaviourMode;
            imageEditorThreadGroupSize = PicEaseLite_Settings.ImageEditorThreadGroupSize;
            imageEditorPrecisionMode = PicEaseLite_Settings.ImageEditorPrecisionMode;
            imageEditorInfoLabelFontSize = PicEaseLite_Settings.ImageEditorInfoLabelFontSize;
            imageEditorInfoLabelFontColor = PicEaseLite_Settings.ImageEditorInfoLabelFontColor;
            imageEditorInfoBackgroundColor = PicEaseLite_Settings.ImageEditorInfoBackgroundColor;
            imageEditorEnableDebugLogForExportedFiles = PicEaseLite_Settings.ImageEditorEnableDebugLogForExportedFiles;
            sceneScreenshotDefaultImageWidth = PicEaseLite_Settings.SceneScreenshotDefaultImageWidth;
            sceneScreenshotDefaultImageHeight = PicEaseLite_Settings.SceneScreenshotDefaultImageHeight;
            sceneScreenshotDefaultExportImageFormat = PicEaseLite_Settings.SceneScreenshotDefaultExportImageFormat;
            sceneScreenshotDefaultExportDirectory = PicEaseLite_Settings.SceneScreenshotDefaultExportDirectory;
        }

        private void InitializeImageEditor()
        {
            if (PicEaseLite_Session.instance.ComputeShaderImageProcessing == null)
            {
                string shaderPath = PicEaseLite_File.GetShaderFilePath(PicEaseLite_Constants.ComputeShader);
                computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
                PicEaseLite_Session.instance.ComputeShaderImageProcessing = computeShader;
                PicEaseLite_Session.instance.IsComputeShaderImageProcessingLoaded = true;
            }
            else
            {
                computeShader = PicEaseLite_Session.instance.ComputeShaderImageProcessing;
            }

            if (imageEffectsManager != null)
            {
                imageEffectsManager.ResetImageEffects();
            }

            UpdateImageEditorInitializedFields();
        }

        private void InitializeSceneScreenshot()
        {
            UpdateSceneScreenshotInitializedFields();
        }
        #endregion

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.PrimaryPanelStyle);
            screenRect = new(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(screenRect, PicEaseLite_Resources.Graphics.Background, ScaleMode.ScaleAndCrop);

            #region Body
            EditorGUILayout.BeginVertical();
            switch (currentViewMode)
            {
                case ViewMode.Home:
                    DrawHomeTab();
                    break;
                case ViewMode.About:
                    DrawAboutTab();
                    break;
                case ViewMode.Settings:
                    DrawSettingsTab();
                    break;
                case ViewMode.ImageEditor:
                    DrawImageEditorTab();
                    break;
                case ViewMode.SceneScreenshot:
                    DrawSceneScreenshotTab();
                    break;
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Footer
            if (currentViewMode != ViewMode.Home)
            {
                EditorGUILayout.BeginHorizontal(PicEaseLite_GUI.MenuButtonsPanelStyle);
                DrawMenuButtons();
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            EditorGUILayout.EndVertical();
        }

        #region Methods
        #region General
        private void DrawMenuButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Image Editor", PicEaseLite_GUI.MenuButtonStyle))
            {
                SelectImageEditorTab();
            }
            GUILayout.Space(spacingMenuButtons);
            if (GUILayout.Button("About", PicEaseLite_GUI.MenuButtonStyle))
            {
                SelectAboutTab();
            }
            GUILayout.Space(spacingMenuButtons);
            if (GUILayout.Button("Home", PicEaseLite_GUI.MenuButtonStyle))
            {
                SelectHomeTab();
            }
            GUILayout.Space(spacingMenuButtons);
            if (GUILayout.Button("Settings", PicEaseLite_GUI.MenuButtonStyle))
            {
                SelectSettingsTab();
            }
            GUILayout.Space(spacingMenuButtons);
            if (GUILayout.Button("Scene Screenshot", PicEaseLite_GUI.MenuButtonStyle))
            {
                SelectSceneScreenshotTab();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void SelectHomeTab()
        {
            SwitchViewMode(ViewMode.Home);
        }

        private void SelectAboutTab()
        {
            SwitchViewMode(ViewMode.About);
        }

        private void SelectSettingsTab()
        {
            SwitchViewMode(ViewMode.Settings);
        }

        private void SelectImageEditorTab()
        {
            SwitchViewMode(ViewMode.ImageEditor, UpdateImageEditorCategoryLabel);
        }

        private void SelectSceneScreenshotTab()
        {
            SwitchViewMode(ViewMode.SceneScreenshot, RefreshCameraList);
        }

        private void SwitchViewMode(ViewMode newViewMode, Action extraAction = null)
        {
            if (currentViewMode == newViewMode) return;

            extraAction?.Invoke();
            currentViewMode = newViewMode;
        }
        #endregion

        #region Home
        private void DrawHomeTab()
        {
            DrawOverlay();

            scrollHomeBody = EditorGUILayout.BeginScrollView(scrollHomeBody, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawAssetTitle();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
            DrawMenuButtons();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            DrawAssetVersion();
        }

        private void DrawOverlay()
        {
            GUI.DrawTexture(screenRect, PicEaseLite_Resources.Graphics.Overlay, ScaleMode.ScaleAndCrop);
        }

        private void DrawAssetTitle()
        {
            float labelWidth = position.width * titleWidthPercentage;
            float labelHeight = labelWidth * titleAspectRatio;

            labelWidth = Mathf.Clamp(labelWidth, titleMinWidth, titleMaxWidth);
            labelHeight = Mathf.Clamp(labelHeight, titleMinHeight, titleMaxHeight);

            GUILayout.Label(PicEaseLite_Resources.Graphics.Title, PicEaseLite_GUI.TitleLabelStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
        }

        private void DrawAssetVersion()
        {
            EditorGUILayout.BeginHorizontal(PicEaseLite_GUI.FooterPanelStyle);
            GUILayout.Label(PicEaseLite_Constants.AssetVersion, PicEaseLite_GUI.VersionLabelStyle);
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region About
        private void DrawAboutTab()
        {
            GUILayout.Space(30);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(25);
            DrawWelcomePanel();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();
            DrawPatchNotesPanel();
            GUILayout.Space(5);
            DrawPromotionalPanel();
            EditorGUILayout.EndVertical();

            GUILayout.Space(25);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(30);
        }

        private void DrawWelcomePanel()
        {
            scrollAboutWelcomePanel = EditorGUILayout.BeginScrollView(scrollAboutWelcomePanel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label("Features Breakdown", PicEaseLite_GUI.BoldLabelStyle);

            GUILayout.Label("Image Editor", PicEaseLite_GUI.MiniBoldLabelStyle);
            GUILayout.Label(imageEditorText, PicEaseLite_GUI.RegularLabelStyle);

            GUILayout.Label("Scene Screenshot", PicEaseLite_GUI.MiniBoldLabelStyle);
            GUILayout.Label(sceneScreenshotText, PicEaseLite_GUI.RegularLabelStyle);

            GUILayout.Label("Additional Notes", PicEaseLite_GUI.BoldLabelStyle);
            GUILayout.Label(additionalNotesText, PicEaseLite_GUI.RegularLabelStyle);

            GUILayout.Label("PicEase Full Version", PicEaseLite_GUI.BoldLabelStyle);
            GUILayout.Label(picEaseFullVersion, PicEaseLite_GUI.RegularLabelStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawPatchNotesPanel()
        {
            scrollAboutPatchNotes = EditorGUILayout.BeginScrollView(scrollAboutPatchNotes, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Patch Notes", PicEaseLite_GUI.BoldLabelStyle);
            GUILayout.Label(patchNotes, PicEaseLite_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawPromotionalPanel()
        {
            scrollAboutPromotional = EditorGUILayout.BeginScrollView(scrollAboutPromotional, GUILayout.MinHeight(200), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("My Other Assets", PicEaseLite_GUI.BoldLabelStyle);
            GUILayout.Space(5);

            #region PicEase
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Empty, PicEaseLite_GUI.PromotionaPicEaselButtonStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/picease-297051");
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(2);
            GUILayout.Label("PicEase", PicEaseLite_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);
            GUILayout.Label("An image editor, map generator and screenshot tool.", PicEaseLite_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10);

            #region Hierarchy Designer
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(string.Empty, PicEaseLite_GUI.PromotionaHierarchyDesignerlButtonStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/hierarchy-designer-273928");
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(2);
            GUILayout.Label("Hierarchy Designer", PicEaseLite_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);
            GUILayout.Label("An editor tool to enhance your hierarchy window.", PicEaseLite_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Settings
        private void DrawSettingsTab()
        {
            EditorGUILayout.Space(10);
            scrollSettingsBody = EditorGUILayout.BeginScrollView(scrollSettingsBody, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Image Editor
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Image Editor", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField("File Settings", PicEaseLite_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(2);
            imageEditorDefaultTextureFilterMode = PicEaseLite_GUI.DrawEnumPopup("Default Texture Filter Mode", labelWidthSettings, imageEditorDefaultTextureFilterMode, FilterMode.Bilinear, true, "The texture filter mode value applied to images loaded from outside your Unity Project.");
            imageEditorDefaultTextureAnisoLevel = PicEaseLite_GUI.DrawIntSlider("Default Texture Aniso Level", labelWidthSettings, imageEditorDefaultTextureAnisoLevel, 0, 16, 1, true, "The aniso level value applied to images loaded from outside your Unity Project.");
            imageEditorExportImageFormat = PicEaseLite_GUI.DrawEnumPopup("Export Image Format Type", labelWidthSettings, imageEditorExportImageFormat, PicEaseLite_Converter.ExportImageFormat.OriginalImage, true, "Export Values' Image Format Behavior:\n\nOriginal: Initializes the export image format field to match the file format of the original image (e.g., PNG, JPG).\n\nSettings: Initializes the export image format field with the default value specified in the export settings.");
            imageEditorDefaultExportImageFormat = PicEaseLite_GUI.DrawEnumPopup("Default Export Image Format", labelWidthSettings, imageEditorDefaultExportImageFormat, PicEaseLite_Converter.ImageFormat.PNG, true, "The default initialized value of the Image Format in the Export Field.");
            imageEditorSameExportPath = PicEaseLite_GUI.DrawToggle("Same Export Path", labelWidthSettings, imageEditorSameExportPath, true, true, "The directory will open in the same path as the input (original) image that was loaded.\n\nNote: This overrides the default export directory.");
            imageEditorDefaultExportDirectory = PicEaseLite_GUI.DrawStringField("Default Export Directory", labelWidthSettings, imageEditorDefaultExportDirectory, "", true, @"The default directory initialized for the exported image. When exporting an image, this directory will be set as the default path if valid (e.g., D:\...\ProjectName\Assets).");
            EditorGUILayout.Space(2);

            EditorGUILayout.LabelField("Miscellaneous Settings", PicEaseLite_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(2);
            imageEditorEnableImageInfoDisplay = PicEaseLite_GUI.DrawToggle("Enable Image Info Display", labelWidthSettings, imageEditorEnableImageInfoDisplay, true, true, "Display image info (name, dimensions, format, and source) while in compare mode?");
            imageEditorInfoLabelFontSize = PicEaseLite_GUI.DrawIntSlider("Image Info Label Font Size", labelWidthSettings, imageEditorInfoLabelFontSize, 7, 21, 12, true, "The label font size for the image information displayed in compare mode.");
            imageEditorInfoLabelFontColor = PicEaseLite_GUI.DrawColorField("Image Info Label Font Color", labelWidthSettings, imageEditorInfoLabelFontColor, Color.white, true, "The label font color for the image information displayed in compare mode.");
            imageEditorInfoBackgroundColor = PicEaseLite_GUI.DrawColorField("Image Info Background Color", labelWidthSettings, imageEditorInfoBackgroundColor, new(0f, 0f, 0f, 0.25f), true, "The background color for the image information displayed in compare mode.");
            imageEditorEnableDebugLogForExportedFiles = PicEaseLite_GUI.DrawToggle("Enable Debug.Log for Exports", labelWidthSettings, imageEditorEnableDebugLogForExportedFiles, true, true, "Display a Debug.Log message showing the directory where the edited image was exported.");
            EditorGUILayout.Space(2);

            EditorGUILayout.LabelField("Performance Settings", PicEaseLite_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(2);
            imageEditorBehaviourMode = PicEaseLite_GUI.DrawEnumPopup("Behaviour Mode", labelWidthSettings, imageEditorBehaviourMode, PicEaseLite_Image.BehaviourMode.Synced, true, "Choose the Behaviour Mode for image processing:\n\n" +
                "• Synced – GPU and CPU are synchronized. Image processing uses AsyncGPUReadbackRequest, which prevents freezes during readback but introduces a slight FPS latency, particularly noticeable on high-resolution images and lower-end devices.\n\n" +
                "• Unsynced – Uses the previous behavior. Image processing may cause occasional minor freezes, with minimal to no FPS latency.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            imageEditorThreadGroupSize = PicEaseLite_GUI.DrawEnumPopup("Thread Group Size", labelWidthSettings, imageEditorThreadGroupSize, PicEaseLite_Image.ThreadGroupSize.SixteenBySixteen, true, "Adjust the number of threads used for processing (8x8, 16x16, 32x32). More threads can improve performance on powerful hardware, while fewer threads may reduce resource usage.\n\n*Re-open the PicEase window for this change to take effect.*");
            imageEditorPrecisionMode = PicEaseLite_GUI.DrawEnumPopup("Compute Precision Mode", labelWidthSettings, imageEditorPrecisionMode, PicEaseLite_Image.PrecisionMode.Full, true, "Choose the precision mode for image processing:\n\n" +
                "• Half precision (half) offers faster performance and uses less memory but may result in reduced color accuracy or subtle artifacts in fine adjustments, especially in complex images.\n\n" +
                "• Full precision (float) provides the highest color accuracy and quality, ensuring smoother blending and more precise adjustments, though it may come with a slight performance cost, particularly with large images.\n\n" +
                "*Re-open the PicEase window for this change to take effect.*");
            EditorGUILayout.Space(2);

            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.Space(5);

            #region Scene Screenshot
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Scene Screenshot", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(4);

            EditorGUILayout.LabelField("File Settings", PicEaseLite_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(2);
            sceneScreenshotDefaultImageWidth = PicEaseLite_GUI.DrawIntField("Default Image Width", labelWidthSettings, sceneScreenshotDefaultImageWidth, 1920, true, "The default initialized value of the Image Width in the Input Field.");
            sceneScreenshotDefaultImageHeight = PicEaseLite_GUI.DrawIntField("Default Image Height", labelWidthSettings, sceneScreenshotDefaultImageHeight, 1080, true, "The default initialized value of the Image Height in the Input Field.");
            sceneScreenshotDefaultExportImageFormat = PicEaseLite_GUI.DrawEnumPopup("Default Export Image Format", labelWidthSettings, sceneScreenshotDefaultExportImageFormat, PicEaseLite_Converter.ImageFormat.PNG, true, "The default initialized value of the Image Format in the Input Field.");
            sceneScreenshotDefaultExportDirectory = PicEaseLite_GUI.DrawStringField("Default Export Directory", labelWidthSettings, sceneScreenshotDefaultExportDirectory, "", true, @"The default directory initialized for the exported screenshot. When exporting a screenshot, this directory will be set as the default path if valid (e.g., C:\Users\...\Desktop).");
            EditorGUILayout.Space(2);

            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
            {
                SaveSettings();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void SaveSettings()
        {
            PicEaseLite_Settings.ImageEditorDefaultImportTextureFilterMode = imageEditorDefaultTextureFilterMode;
            PicEaseLite_Settings.ImageEditorDefaultImportTextureAnisoLevel = imageEditorDefaultTextureAnisoLevel;
            PicEaseLite_Settings.ImageEditorExportImageFormat = imageEditorExportImageFormat;
            PicEaseLite_Settings.ImageEditorDefaultExportImageFormat = imageEditorDefaultExportImageFormat;
            PicEaseLite_Settings.ImageEditorSameExportPath = imageEditorSameExportPath;
            PicEaseLite_Settings.ImageEditorDefaultExportDirectory = imageEditorDefaultExportDirectory;
            PicEaseLite_Settings.ImageEditorEnableImageInfoDisplay = imageEditorEnableImageInfoDisplay;
            PicEaseLite_Settings.ImageEditorInfoLabelFontSize = imageEditorInfoLabelFontSize;
            PicEaseLite_Settings.ImageEditorInfoLabelFontColor = imageEditorInfoLabelFontColor;
            PicEaseLite_Settings.ImageEditorInfoBackgroundColor = imageEditorInfoBackgroundColor;
            PicEaseLite_Settings.ImageEditorEnableDebugLogForExportedFiles = imageEditorEnableDebugLogForExportedFiles;
            PicEaseLite_Settings.ImageEditorBehaviourMode = imageEditorBehaviourMode;
            PicEaseLite_Settings.ImageEditorThreadGroupSize = imageEditorThreadGroupSize;
            PicEaseLite_Settings.ImageEditorPrecisionMode = imageEditorPrecisionMode;
            PicEaseLite_Settings.SceneScreenshotDefaultImageWidth = sceneScreenshotDefaultImageWidth;
            PicEaseLite_Settings.SceneScreenshotDefaultImageHeight = sceneScreenshotDefaultImageHeight;
            PicEaseLite_Settings.SceneScreenshotDefaultExportImageFormat = sceneScreenshotDefaultExportImageFormat;
            PicEaseLite_Settings.SceneScreenshotDefaultExportDirectory = sceneScreenshotDefaultExportDirectory;
            PicEaseLite_Settings.SaveSettings();

            UpdateImageEditorInitializedFields();
            UpdateSceneScreenshotInitializedFields();
        }

        private void UpdateImageEditorInitializedFields()
        {
            imageEditorCurrentExportFormat = imageEditorDefaultExportImageFormat;
        }

        private void UpdateSceneScreenshotInitializedFields()
        {
            sceneScreenshotCurrentImageWidth = sceneScreenshotDefaultImageWidth;
            sceneScreenshotCurrentImageHeight = sceneScreenshotDefaultImageHeight;
            sceneScreenshotCurrentExportFormat = sceneScreenshotDefaultExportImageFormat;
        }
        #endregion

        #region Image Editor
        private void DrawImageEditorTab()
        {
            if (originalImage == null)
            {
                InsertImage();
            }
            else
            {
                if (imageEffectsManager == null)
                {
                    imageEffectsManager = new(originalImage, computeShader, this, originalImageImporterSettings, originalMaxTextureSize, originalTextureCompression);
                }

                EditorGUILayout.BeginHorizontal();

                #region Left Panel
                EditorGUILayout.BeginVertical(PicEaseLite_GUI.ImageEditorLeftPanelStyle);

                #region Header
                DrawImageEditorCategoriesButtons();
                #endregion

                #region Body
                EditorGUILayout.Space(4);
                GUILayout.Label(cachedCategoryLabel, PicEaseLite_GUI.CategoryLabelStyle);
                EditorGUILayout.Space(2);

                switch (currentImageEditorCategory)
                {
                    case ImageEditorCategory.Adjustments:
                        DrawImageAdjustValueFields();
                        break;
                    case ImageEditorCategory.Filters:
                        DrawImageFiltersFields();
                        break;
                }
                #endregion

                EditorGUILayout.Space(6);
                GUILayout.FlexibleSpace();

                #region Footer
                DrawImageEditorButtons();
                #endregion

                EditorGUILayout.EndVertical();
                #endregion

                #region Right Panel
                DrawPreviewImage();
                #endregion

               EditorGUILayout.EndHorizontal();
            }
        }

        private void InsertImage()
        {
            GUILayout.BeginVertical(PicEaseLite_GUI.DragAndDropPanelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(50);
                        GUILayout.Label("Drag & Drop Your Image Here", PicEaseLite_GUI.DragAndDropLabelStyle, GUILayout.ExpandWidth(true));
                        GUILayout.Space(5);
                        if (GUILayout.Button("Upload Image From Device", GUILayout.Height(30), GUILayout.ExpandWidth(true)))
                        {
                            string path = EditorUtility.OpenFilePanel("PicEase Lite Image Editor - Load Image", "", PicEaseLite_Converter.GetImageFormatString());
                            LoadImageFromPath(path, false);
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();

            Rect dropArea = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                if (dropArea.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Texture2D texture)
                            {
                                string path = AssetDatabase.GetAssetPath(texture);
                                LoadImageFromPath(path, !string.IsNullOrEmpty(path));
                                break;
                            }
                        }
                    }
                    Event.current.Use();
                }
            }
        }

        private void LoadImageFromPath(string path, bool isFromAssetDatabase)
        {
            if (!string.IsNullOrEmpty(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                string extension = Path.GetExtension(path);

                Texture2D tex;
                bool isTextureLoaded;

                if (extension == ".tga" || extension == ".TGA")
                {
                    tex = PicEaseLite_Converter.LoadImageTGA(fileData);
                    isTextureLoaded = tex != null;
                }
                else
                {
                    tex = new(2, 2, TextureFormat.RGBA32, false);
                    isTextureLoaded = tex.LoadImage(fileData, false);
                }

                if (isTextureLoaded)
                {
                    if (isFromAssetDatabase)
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (importer != null)
                        {
                            tex.filterMode = importer.filterMode;
                            tex.anisoLevel = importer.anisoLevel;
                            
                            originalImageImporterSettings = new();
                            importer.ReadTextureSettings(originalImageImporterSettings);
                            originalMaxTextureSize = importer.maxTextureSize;
                            originalTextureCompression = importer.textureCompression;
                        }
                        else
                        {
                            tex.filterMode = PicEaseLite_Settings.ImageEditorDefaultImportTextureFilterMode;
                            tex.anisoLevel = PicEaseLite_Settings.ImageEditorDefaultImportTextureAnisoLevel;

                            originalImageImporterSettings = null;
                            originalMaxTextureSize = 2048;
                            originalTextureCompression = TextureImporterCompression.Compressed;
                        }
                    }
                    else
                    {
                        tex.filterMode = PicEaseLite_Settings.ImageEditorDefaultImportTextureFilterMode;
                        tex.anisoLevel = PicEaseLite_Settings.ImageEditorDefaultImportTextureAnisoLevel;

                        originalImageImporterSettings = null;
                        originalMaxTextureSize = 2048;
                        originalTextureCompression = TextureImporterCompression.Compressed;
                    }
                    tex.Apply();

                    ClearImage();

                    originalImage = tex;

                    imageEffectsManager = new(originalImage, computeShader, this, originalImageImporterSettings, originalMaxTextureSize, originalTextureCompression);
                    imageEffectsManager.ResetImageEffects();

                    currentLoadedImageName = Path.GetFileNameWithoutExtension(path);
                    currentImagePath = isFromAssetDatabase ? path : Path.GetFullPath(path);
                    currentImageFormat = extension.TrimStart('.').ToUpper();
                    currentImageWidth = tex.width;
                    currentImageHeight = tex.height;
                    currentImageDirectory = Path.GetDirectoryName(currentImagePath);
                    if (imageEditorExportImageFormat == PicEaseLite_Converter.ExportImageFormat.OriginalImage)
                    {
                        imageEditorCurrentExportFormat = PicEaseLite_Converter.GetImageFormatFromExtension(extension);
                    }

                    Repaint();
                }
                else
                {
                    Debug.LogError("Failed to load image. Most likely an unsupported file format.");
                    DestroyImmediate(tex);
                }
            }
        }

        private void DrawImageEditorCategoriesButtons()
        {
            EditorGUILayout.BeginHorizontal(PicEaseLite_GUI.ImageEditorCategoryPanelStyle);
            GUILayout.FlexibleSpace();

            if (PicEaseLite_GUI.DrawButtonIcon("Adjustments", PicEaseLite_GUI.AdjustmentsButtonStyle))
            {
                SelectAdjustmentCategory();
            }
            GUILayout.Space(35);
            if (PicEaseLite_GUI.DrawButtonIcon(" Filters", PicEaseLite_GUI.FiltersButtonStyle))
            {
                SelectFiltersCategory();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawImageAdjustValueFields()
        {
            scrollImageEditorAdjustments = EditorGUILayout.BeginScrollView(scrollImageEditorAdjustments, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region Color
            GUILayout.Label("Color", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageEffectsManager.VibranceMagnitude = PicEaseLite_GUI.DrawFloatSlider("Vibrance", imageEffectsManager.VibranceMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.SaturationMagnitude = PicEaseLite_GUI.DrawFloatSlider("Saturation", imageEffectsManager.SaturationMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.TemperatureMagnitude = PicEaseLite_GUI.DrawFloatSlider("Temperature", imageEffectsManager.TemperatureMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.TintMagnitude = PicEaseLite_GUI.DrawFloatSlider("Tint", imageEffectsManager.TintMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.HueMagnitude = PicEaseLite_GUI.DrawFloatSlider("Hue", imageEffectsManager.HueMagnitude, -180, 180, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageEffectsManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Light
            GUILayout.Label("Light", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageEffectsManager.BrightnessMagnitude = PicEaseLite_GUI.DrawFloatSlider("Brightness", imageEffectsManager.BrightnessMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.ExposureMagnitude = PicEaseLite_GUI.DrawFloatSlider("Exposure", imageEffectsManager.ExposureMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.ContrastMagnitude = PicEaseLite_GUI.DrawFloatSlider("Contrast", imageEffectsManager.ContrastMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.BlackMagnitude = PicEaseLite_GUI.DrawFloatSlider("Black", imageEffectsManager.BlackMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.WhiteMagnitude = PicEaseLite_GUI.DrawFloatSlider("White", imageEffectsManager.WhiteMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            imageEffectsManager.ShadowMagnitude = PicEaseLite_GUI.DrawFloatSlider("Shadow", imageEffectsManager.ShadowMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageEffectsManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Detail
            GUILayout.Label("Detail", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageEffectsManager.SharpenMagnitude = PicEaseLite_GUI.DrawFloatSlider("Sharpen", imageEffectsManager.SharpenMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageEffectsManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            #region Style
            GUILayout.Label("Style", PicEaseLite_GUI.BoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            imageEffectsManager.PixelateMagnitude = PicEaseLite_GUI.DrawFloatSlider("Pixelate", imageEffectsManager.PixelateMagnitude, adjustmentSliderMinValue, adjustmentSliderMaxValue, adjustmentSliderDefaultValue);
            if (EditorGUI.EndChangeCheck())
            {
                imageEffectsManager.ApplyAdjustments();
            }

            EditorGUILayout.Space(6);
            #endregion

            EditorGUILayout.EndScrollView();
        }

        private void DrawImageFiltersFields()
        {
            scrollImageEditorFilters = EditorGUILayout.BeginScrollView(scrollImageEditorFilters, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            foreach (string filterName in PicEaseLite_Image.GetAvailableFilters())
            {
                if (GUILayout.Button(filterName, GUILayout.Height(30)))
                {
                    imageEffectsManager.ApplyFilter(filterName);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawImageEditorButtons()
        {
            #region Hold To Compare Button
            Rect buttonRect = GUILayoutUtility.GetRect(new("Hold To Compare"), GUI.skin.button, GUILayout.Height(30));
            if (Event.current.type == EventType.MouseDown && buttonRect.Contains(Event.current.mousePosition))
            {
                if (!isComparing)
                {
                    isComparing = true;
                    Repaint();
                    isComparingDoOnce = true;
                }
                Event.current.Use();
            }
            if (Event.current.type == EventType.MouseUp && isComparing)
            {
                isComparing = false;
                if (isComparingDoOnce)
                {
                    Repaint();
                    isComparingDoOnce = false;
                }
            }
            GUI.Button(buttonRect, "Hold To Compare");
            #endregion

            if (GUILayout.Button("Reset All Values", GUILayout.Height(30)))
            {
                ResetEffects();
            }
            if (GUILayout.Button("Clear & Load New Image", GUILayout.Height(30)))
            {
                ClearImage();
                Repaint();
            }
        }

        private void DrawPreviewImage()
        {
            if (originalImage == null) return;

            EditorGUILayout.BeginVertical(PicEaseLite_GUI.ImageEditorRightPanelStyle);

            bool comparing = isComparing;
            GUILayout.Label(comparing ? "Original Image" : "Preview Image", PicEaseLite_GUI.PreviewLabelStyle);
            EditorGUILayout.Space(2);

            Texture imageToDisplay = comparing ? originalImage : imageEffectsManager.GetResultTexture();
            if (imageToDisplay != null)
            {
                Rect panelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                float imageAspect = (float)imageToDisplay.width / imageToDisplay.height;
                float panelAspect = panelRect.width / panelRect.height;
                float displayWidth, displayHeight;

                if (imageAspect > panelAspect)
                {
                    displayWidth = panelRect.width;
                    displayHeight = displayWidth / imageAspect;
                }
                else
                {
                    displayHeight = panelRect.height;
                    displayWidth = displayHeight * imageAspect;
                }

                float offsetX = (panelRect.width - displayWidth) * 0.5f;
                float offsetY = (panelRect.height - displayHeight) * 0.5f;
                Rect imageRect = new(panelRect.x + offsetX, panelRect.y + offsetY, displayWidth, displayHeight);

                GUI.DrawTexture(imageRect, imageToDisplay, ScaleMode.ScaleToFit, true);

                if (imageEditorEnableImageInfoDisplay && comparing)
                {
                    Rect labelRect = new(imageRect.x + imageInfoLabelPadding, imageRect.y + imageInfoLabelPadding, imageRect.width - 2 * imageInfoLabelPadding, ImageInfoLabelHeight);

                    Color originalColor = GUI.color;
                    GUI.color = imageEditorInfoBackgroundColor;
                    GUI.DrawTexture(labelRect, EditorGUIUtility.whiteTexture);
                    GUI.color = originalColor;

                    string infoText = $"Name: {currentLoadedImageName}  •  " +
                                      $"Dimensions: {currentImageWidth} x {currentImageHeight}  •  " +
                                      $"Format: {currentImageFormat}  •  " +
                                      $"Source: {currentImagePath}";

                    GUI.Label(labelRect, infoText, PicEaseLite_GUI.InfoLabelStyle);
                }

                EditorGUILayout.Space(2);
                DrawImageEditorExportFields();
            }

            EditorGUILayout.EndVertical();
        }

        private void ClearImage()
        {
            if (imageEffectsManager != null)
            {
                imageEffectsManager.Dispose();
                imageEffectsManager = null;
            }

            if (originalImage != null)
            {
                DestroyImmediate(originalImage);
                originalImage = null;
            }

            currentImagePath = string.Empty;
            currentImageFormat = "Unknown";
            currentImageWidth = 0;
            currentImageHeight = 0;
            currentImageDirectory = string.Empty;
        }

        private void ResetEffects()
        {
            imageEffectsManager.ResetImageEffects();
        }

        private void DrawImageEditorExportFields()
        {
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            GUILayout.Label("Export Values", PicEaseLite_GUI.BoldLabelStyle);

            imageEditorCurrentExportFormat = PicEaseLite_GUI.DrawEnumPopup("Image Format", 100, imageEditorCurrentExportFormat, imageEditorDefaultExportImageFormat);
            GUILayout.Space(4);
            if (GUILayout.Button("Export Image", GUILayout.Height(30)))
            {
                ExportImage();
            }
            EditorGUILayout.EndVertical();
        }

        private void ExportImage()
        {
            string exportDirectory = "";
            if (PicEaseLite_Settings.ImageEditorSameExportPath)
            {
                exportDirectory = currentImageDirectory;
            }
            else
            {
                exportDirectory = Directory.Exists(PicEaseLite_Settings.ImageEditorDefaultExportDirectory) ? PicEaseLite_Settings.ImageEditorDefaultExportDirectory : "";
            }
            string extension = PicEaseLite_Converter.GetExtension(imageEditorCurrentExportFormat);
            string path = EditorUtility.SaveFilePanel("PicEase Lite Image Editor - Export Edited Image", exportDirectory, $"{currentLoadedImageName} Edited {DateTime.Now.ToString(PicEaseLite_Constants.DateFormat)}.{extension}", extension);

            if (!string.IsNullOrEmpty(path))
            {
                Texture2D exportTexture = imageEffectsManager.GetFinalTexture2D();
                byte[] bytes = PicEaseLite_Converter.EncodeImage(exportTexture, imageEditorCurrentExportFormat);
                try
                {
                    File.WriteAllBytes(path, bytes);
                    if (imageEditorEnableDebugLogForExportedFiles)
                    {
                        Debug.Log($"PicEase Lite edited image exported to {path}");
                    }
                    RefreshAssetDatabase(path);

                    if (originalImageImporterSettings != null && path.StartsWith(Application.dataPath))
                    {
                        string relativePath = "Assets" + path[Application.dataPath.Length..];
                        EditorApplication.delayCall += () =>
                        {
                            imageEffectsManager.SetTextureImporterSettings(relativePath);
                        };
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to export edited image: {ex.Message}");
                }
            }
        }

        private void SelectAdjustmentCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.Adjustments);
        }

        private void SelectFiltersCategory()
        {
            SwitchImageEditorCategory(ImageEditorCategory.Filters);
        }

        private void SwitchImageEditorCategory(ImageEditorCategory newImageEditorCategory, Action extraAction = null)
        {
            if (currentImageEditorCategory == newImageEditorCategory) return;

            extraAction?.Invoke();
            currentImageEditorCategory = newImageEditorCategory;
            UpdateImageEditorCategoryLabel();
        }

        private void UpdateImageEditorCategoryLabel()
        {
            cachedCategoryLabel = currentImageEditorCategory.ToString();
        }
        #endregion

        #region Scene Screenshot
        private void DrawSceneScreenshotTab()
        {
            #region Header
            GUILayout.Space(6);
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene Screenshot", PicEaseLite_GUI.BoldLabelStyle, GUILayout.Width(150));

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh Cameras List", GUILayout.Height(25), GUILayout.ExpandWidth(false), GUILayout.Width(140)))
            {
                RefreshCameraList();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            DrawScreenshotFields();

            EditorGUILayout.EndVertical();
            #endregion

            #region Body
            if (sceneScreenshot != null)
            {
                GUILayout.Space(5);
                DrawScreenshotPreview();
                GUILayout.Space(5);
                DrawScreenshotExportFields();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
            #endregion
        }

        private void RefreshCameraList()
        {
            int cameraCount = Camera.allCamerasCount;
            availableCameras = new Camera[cameraCount];
            Camera.GetAllCameras(availableCameras);

            cameraNames = new string[cameraCount];
            for (int i = 0; i < cameraCount; i++)
            {
                cameraNames[i] = availableCameras[i].name;
            }

            selectedCameraIndex = 0;
        }

        private void DrawScreenshotFields()
        {
            if (availableCameras.Length > 0)
            {
                selectedCameraIndex = PicEaseLite_GUI.DrawPopup("Select Camera", labelWidthSceneScreenshot, selectedCameraIndex, cameraNames, 0);
                sceneScreenshotCurrentImageWidth = Mathf.Min(16384, PicEaseLite_GUI.DrawIntField("Image Width", labelWidthSceneScreenshot, sceneScreenshotCurrentImageWidth, sceneScreenshotDefaultImageWidth));
                sceneScreenshotCurrentImageHeight = Mathf.Min(16384, PicEaseLite_GUI.DrawIntField("Image Height", labelWidthSceneScreenshot, sceneScreenshotCurrentImageHeight, sceneScreenshotDefaultImageHeight));

                GUILayout.Space(4);
                if (GUILayout.Button("Take Screenshot", GUILayout.Height(30)))
                {
                    if (sceneScreenshotCurrentImageWidth >= 4096 || sceneScreenshotCurrentImageHeight >= 4096)
                    {
                        bool confirm = EditorUtility.DisplayDialog(
                            "High Resolution Warning!",
                            "A resolution of 4096 or higher may cause freezing and could crash on lower-end devices. Are you sure you want to continue?",
                            "Yes", "No"
                        );

                        if (!confirm)
                        {
                            return;
                        }
                    }

                    CaptureSceneScreenshotForPreview();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No cameras were found in the scene.", MessageType.Warning);
            }
        }

        private void CaptureSceneScreenshotForPreview()
        {
            if (availableCameras.Length > 0 && selectedCameraIndex < availableCameras.Length)
            {
                Camera cam = availableCameras[selectedCameraIndex];
                if (cam != null)
                {
                    RenderTexture renderTexture = null;
                    try
                    {
                        renderTexture = new(sceneScreenshotCurrentImageWidth, sceneScreenshotCurrentImageHeight, 24, RenderTextureFormat.DefaultHDR);
                        cam.targetTexture = renderTexture;
                        cam.Render();

                        RenderTexture.active = renderTexture;
                        sceneScreenshot = new(renderTexture.width, renderTexture.height, TextureFormat.RGB9e5Float, false);
                        sceneScreenshot.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                        sceneScreenshot.Apply();

                        Repaint();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to capture screenshot: {ex.Message}");
                    }
                    finally
                    {
                        if (cam != null)
                        {
                            cam.targetTexture = null;
                        }
                        if (RenderTexture.active == renderTexture)
                        {
                            RenderTexture.active = null;
                        }
                        if (renderTexture != null)
                        {
                            DestroyImmediate(renderTexture);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("PicEase Lite Scene Screenshot", "The selected camera is null.", "OK");
                }
            }
        }

        private void DrawScreenshotPreview()
        {
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            GUILayout.Label("Screenshot Preview", PicEaseLite_GUI.BoldLabelStyle);
            Rect panelRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.DrawTexture(panelRect, sceneScreenshot, ScaleMode.ScaleToFit, true);
            EditorGUILayout.EndVertical();
        }

        private void DrawScreenshotExportFields()
        {
            EditorGUILayout.BeginVertical(PicEaseLite_GUI.SecondaryPanelStyle);
            GUILayout.Label("Export Values", PicEaseLite_GUI.BoldLabelStyle);

            sceneScreenshotCurrentExportFormat = PicEaseLite_GUI.DrawEnumPopup("Image Format", 100, sceneScreenshotCurrentExportFormat, sceneScreenshotDefaultExportImageFormat);
            GUILayout.Space(4);
            hdrExport = PicEaseLite_GUI.DrawToggle("Enhance Image with HDR", 165, hdrExport, false);
            GUILayout.Space(4);

            if (GUILayout.Button("Export Screenshot", GUILayout.Height(30)))
            {
                ExportScreenshot();
            }
            EditorGUILayout.EndVertical();
        }

        private void ExportScreenshot()
        {
            string exportDirectory = Directory.Exists(PicEaseLite_Settings.SceneScreenshotDefaultExportDirectory) ? PicEaseLite_Settings.SceneScreenshotDefaultExportDirectory : "";
            string extension = PicEaseLite_Converter.GetExtension(sceneScreenshotCurrentExportFormat);
            string path = EditorUtility.SaveFilePanel("Export Screenshot", exportDirectory, $"PicEaseLite_Screenshot_{DateTime.Now.ToString(PicEaseLite_Constants.DateFormat)}.{extension}", extension);

            if (!string.IsNullOrEmpty(path))
            {
                exportScreenshot = hdrExport ? sceneScreenshot : CaptureSceneScreenshotForExport();
                if (exportScreenshot != null)
                {
                    byte[] bytes = PicEaseLite_Converter.EncodeImage(exportScreenshot, sceneScreenshotCurrentExportFormat);
                    try
                    {
                        File.WriteAllBytes(path, bytes);
                        Debug.Log($"PicEase Lite Screenshot exported to {path}");
                        RefreshAssetDatabase(path);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to export screenshot: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError("Failed to capture screenshot for export.");
                }
            }
        }

        private Texture2D CaptureSceneScreenshotForExport()
        {
            if (availableCameras.Length > 0 && selectedCameraIndex < availableCameras.Length)
            {
                Camera cam = availableCameras[selectedCameraIndex];
                if (cam != null)
                {
                    RenderTexture renderTexture = null;
                    try
                    {
                        RenderTextureFormat format = hdrExport ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                        renderTexture = new(sceneScreenshotCurrentImageWidth, sceneScreenshotCurrentImageHeight, 24, format);
                        cam.targetTexture = renderTexture;
                        cam.Render();

                        RenderTexture.active = renderTexture;
                        Texture2D exportScreenshot = new(renderTexture.width, renderTexture.height, TextureFormat.RGB9e5Float, false);
                        exportScreenshot.ReadPixels(new(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                        exportScreenshot.Apply();

                        return exportScreenshot;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to capture screenshot for export: {ex.Message}");
                        return null;
                    }
                    finally
                    {
                        if (cam != null)
                        {
                            cam.targetTexture = null;
                        }
                        if (RenderTexture.active == renderTexture)
                        {
                            RenderTexture.active = null;
                        }
                        if (renderTexture != null)
                        {
                            DestroyImmediate(renderTexture);
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("PicEase Lite Scene Screenshot", "The selected camera is null.", "OK");
                    return null;
                }
            }
            return null;
        }

        private void ClearSceneScreenshotReferences()
        {
            availableCameras = null;
            cameraNames = null;
            selectedCameraIndex = 0;
            if (sceneScreenshot != null) DestroyImmediate(sceneScreenshot);
            if (exportScreenshot != null) DestroyImmediate(exportScreenshot);
        }
        #endregion

        #region Shared Operations
        private void RefreshAssetDatabase(string path)
        {
            if (path.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }
        }
        #endregion
        #endregion

        private void OnDestroy()
        {
            ClearImage();
            ClearSceneScreenshotReferences();
        }
    }
}
#endif
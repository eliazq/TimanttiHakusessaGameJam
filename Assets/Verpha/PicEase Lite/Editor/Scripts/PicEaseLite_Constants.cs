#if UNITY_EDITOR
namespace Verpha.PicEase.Lite
{
    internal static class PicEaseLite_Constants
    {
        #region Properties
        #region Asset Info
        public const string AssetName = "PicEase Lite";
        public const string AssetVersion = "Version 1.4.0";
        #endregion

        #region Menu Info
        public const string MenuLocation = "Tools/" + AssetName;
        public const int MenuPriority = 0;
        #endregion

        #region Format Info
        public const string DateFormat = "yyyy-MM-dd_hhmmtt";
        #endregion

        #region Folder Names
        public const string EditorFolderName = "Editor";
        public const string DocumentationFolderName = "Documentation";
        public const string SavedDataFolderName = "Saved Data";
        public const string ShaderFolderName = "Shaders";
        #endregion

        #region File Names
        public const string ComputeShader = "PicEaseLite_Image.compute";
        public const string PatchNotesTextFileName = "PicEase Lite Patch Notes.txt";
        public const string SettingsTextFileName = "PicEaseLite_SavedData_Settings.json";
        #endregion
        #endregion
    }
}
#endif
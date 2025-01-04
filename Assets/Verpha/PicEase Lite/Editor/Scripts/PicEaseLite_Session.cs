#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal class PicEaseLite_Session : ScriptableSingleton<PicEaseLite_Session>
    {
        #region Properties
        #region General
        public bool IsInitialized = false;
        #endregion

        #region PicEaseLite_Window_Main
        public bool IsPatchNotesLoaded = false;
        public string PatchNotesContent = string.Empty;
        public bool IsComputeShaderImageProcessingLoaded = false;
        public ComputeShader ComputeShaderImageProcessing = null;
        #endregion

        #region PicEaseLite_Shared_Texture
        public Font FallbackFont = null;
        public Texture2D FallbackTexture = null;
        #endregion
        #endregion
    }
}
#endif
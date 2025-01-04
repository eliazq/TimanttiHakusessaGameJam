#if UNITY_EDITOR
using UnityEditor;

namespace Verpha.PicEase.Lite
{
    [InitializeOnLoad]
    internal static class PicEaseLite_Initializer
    {
        static PicEaseLite_Initializer()
        {
            if (!PicEaseLite_Session.instance.IsInitialized)
            {
                PicEaseLite_Settings.Initialize();
                PicEaseLite_Session.instance.IsInitialized = true;
            }
        }
    }
}
#endif
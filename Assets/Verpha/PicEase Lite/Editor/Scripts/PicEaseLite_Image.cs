#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Verpha.PicEase.Lite
{
    internal class PicEaseLite_Image : IDisposable
    {
        #region Properties
        public enum BehaviourMode { Synced, Unsynced }
        public enum ThreadGroupSize { EightByEight, SixteenBySixteen, ThirtyTwoByThirtyTwo }
        public enum PrecisionMode { Full, Half }

        private readonly BehaviourMode behaviourMode;
        private readonly Texture2D originalImage;
        private readonly ComputeShader computeShader;
        private readonly EditorWindow editorWindow;
        private readonly TextureImporterSettings originalImageImporterSettings;
        private readonly int originalMaxTextureSize;
        private readonly TextureImporterCompression originalTextureCompression;

        private RenderTexture resultTexture;
        private Texture2D readbackTexture;

        private readonly int mainKernelHandle;
        private readonly int threadGroupsX;
        private readonly int threadGroupsY;

        private bool isReadbackInProgress = false;

        #region Adjustments
        public float VibranceMagnitude { get; set; } = 0f;
        public float SaturationMagnitude { get; set; } = 0f;
        public float TemperatureMagnitude { get; set; } = 0f;
        public float TintMagnitude { get; set; } = 0f;
        public float HueMagnitude { get; set; } = 0f;
        public float BrightnessMagnitude { get; set; } = 0f;
        public float ExposureMagnitude { get; set; } = 0f;
        public float ContrastMagnitude { get; set; } = 0f;
        public float BlackMagnitude { get; set; } = 0f;
        public float WhiteMagnitude { get; set; } = 0f;
        public float ShadowMagnitude { get; set; } = 0f;
        public float PixelateMagnitude { get; set; } = 0f;
        public float SharpenMagnitude { get; set; } = 0f;

        private float prevVibranceMagnitude;
        private float prevSaturationMagnitude;
        private float prevTemperatureMagnitude;
        private float prevTintMagnitude;
        private float prevHueMagnitude;
        private float prevBrightnessMagnitude;
        private float prevExposureMagnitude;
        private float prevContrastMagnitude;
        private float prevBlackMagnitude;
        private float prevWhiteMagnitude;
        private float prevShadowMagnitude;
        private float prevPixelateMagnitude;
        private float prevSharpenMagnitude;
        #endregion

        #region Filters
        public readonly struct Filter
        {
            public string Name { get; }
            public float Vibrance { get; }
            public float Saturation { get; }
            public float Temperature { get; }
            public float Tint { get; }
            public float Hue { get; }
            public float Brightness { get; }
            public float Exposure { get; }
            public float Contrast { get; }
            public float Black { get; }
            public float White { get; }
            public float Shadow { get; }
            public float Sharpen { get; }
            public float Pixelate { get; }

            public Filter(string name, float vibrance, float saturation, float temperature, float tint, float hue, float brightness, float exposure, float contrast, float black, float white, float shadow, float sharpen, float pixelate)
            {
                Name = name;
                Vibrance = vibrance;
                Saturation = saturation;
                Temperature = temperature;
                Tint = tint;
                Hue = hue;
                Brightness = brightness;
                Exposure = exposure;
                Contrast = contrast;
                Black = black;
                White = white;
                Shadow = shadow;
                Sharpen = sharpen;
                Pixelate = pixelate;
            }

            public void ApplyTo(PicEaseLite_Image manager)
            {
                manager.VibranceMagnitude = Vibrance;
                manager.SaturationMagnitude = Saturation;
                manager.TemperatureMagnitude = Temperature;
                manager.TintMagnitude = Tint;
                manager.HueMagnitude = Hue;
                manager.BrightnessMagnitude = Brightness;
                manager.ExposureMagnitude = Exposure;
                manager.ContrastMagnitude = Contrast;
                manager.BlackMagnitude = Black;
                manager.WhiteMagnitude = White;
                manager.ShadowMagnitude = Shadow;
                manager.SharpenMagnitude = Sharpen;
                manager.PixelateMagnitude = Pixelate;
            }
        }

        private static readonly Dictionary<string, Filter> filters = new()
        {
            { "Black and White", new("Black and White", 0f, -100f, 0f, 0f, 0f, 20f, 5f, 5f, 0f, 10f, -2f, 0f, 0f) },
            { "Bleach", new("Bleach", -20f, -5f, 0f, 0f, 0f, 5f, 20f, -5f, -10f, 60f, 50f,  0f, 0f) },
            { "Cinematic I", new("Cinematic I", -60f, -20f, -15f, 0f, 0f, 5f, 65f, -10f, -10f, -20f, -40f, 0f, 0f) },
            { "Cinematic II", new("Cinematic II", -50, -5f, -100f, 0f, 0f, 5f, 50f, -25f, -10f, -20f, -40f, 0f, 0f) },
            { "Cool Blues", new("Cool Blues", -10f, 10f, -100f, 0f, 0f, 5f, -20f, 50f, -5f, 45f, 30f, 5f, 0f) },
            { "Dark Gothic", new("Dark Gothic", -100f, 0f, -20f, 0f, 0f, 0f, 20f, 10f, 5f, 10f, -60f, 0f,  0f) },
            { "High Contrast", new("High Contrast", 0f, 0f, 0f, 0f, 0f, 0f, 5f, 75f, 5f, 5f, 1f, 0f, 0f) },
            { "Night Vision", new("Night Vision", -25f, 0f, 0f, 100f, 0f, -15f, 85f, 25f, -20f, 5f, 10f,  0f, 0f) },
            { "Sepia", new("Sepia", -10f, -50f, 100f, 0f, 0f, 0f, 25f, 15f, -5f, 2f, 0f, 0f, 0f) },
            { "Sketch", new("Sketch", 0f, -100f, 0f, 0f, 0f, -10f, -10f, 100f, 15f, 100f, 100f, 0f, 0f) },
            { "Underworld", new("Underworld", 0f, -100f, 0f, 0f, 0f, -15f, 100f, 100f, 0f, 5f, 0f, 0f, 0f) },
            { "Vintage Sepia", new("Vintage Sepia", -15f, -60f, 100f, -5f, 0f, 0f, 30f, 50f, 0f, 10f, 5f, 0f, 0f) },
            { "Vivid", new("Vivid", 50f, 30f, 0f, 0f, 0f, 25f, 25f, 2f, 0f, 5f, 0f, 0f, 0f) },
            { "16-Bit", new("16-Bit", 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, -100f) },
        };
        #endregion
        #endregion

        #region Constructor
        public PicEaseLite_Image(Texture2D originalImage, ComputeShader shader, EditorWindow window, TextureImporterSettings originalImporterSettings, int originalMaxTextureSize, TextureImporterCompression originalTextureCompression)
        {
            this.behaviourMode = PicEaseLite_Settings.ImageEditorBehaviourMode;
            this.originalImage = originalImage != null ? originalImage : throw new ArgumentNullException(nameof(originalImage));
            this.computeShader = shader != null ? shader : throw new ArgumentNullException(nameof(shader));
            this.editorWindow = window;
            this.originalImageImporterSettings = originalImporterSettings;
            this.originalMaxTextureSize = originalMaxTextureSize;
            this.originalTextureCompression = originalTextureCompression;

            resultTexture = new(originalImage.width, originalImage.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };
            resultTexture.Create();
            resultTexture.filterMode = originalImage.filterMode;

            Graphics.Blit(originalImage, resultTexture);

            if (behaviourMode == BehaviourMode.Synced)
            {
                readbackTexture = new(resultTexture.width, resultTexture.height, TextureFormat.RGBAFloat, false)
                {
                    filterMode = originalImage.filterMode
                };
            }

            mainKernelHandle = computeShader.FindKernel("CSMain");

            computeShader.SetTexture(mainKernelHandle, "InputTexture", originalImage);
            computeShader.SetTexture(mainKernelHandle, "Result", resultTexture);

            ThreadGroupSize threadGroupSize = PicEaseLite_Settings.ImageEditorThreadGroupSize;
            float divisionFactor = threadGroupSize switch
            {
                ThreadGroupSize.EightByEight => 8.0f,
                ThreadGroupSize.SixteenBySixteen => 16.0f,
                ThreadGroupSize.ThirtyTwoByThirtyTwo => 32.0f,
                _ => 16.0f,
            };
            threadGroupsX = Mathf.CeilToInt(originalImage.width / divisionFactor);
            threadGroupsY = Mathf.CeilToInt(originalImage.height / divisionFactor);

            ResetComputeShaderParameters();
            CacheAdjustmentsParameters();
        }
        #endregion

        #region Accessors
        public Texture GetResultTexture()
        {
            if (behaviourMode == BehaviourMode.Synced)
            {
                return readbackTexture != null ? readbackTexture : originalImage;
            }
            return resultTexture;
        }

        public Texture2D GetFinalTexture2D()
        {
            ApplyAdjustments();

            RenderTexture tempRT = RenderTexture.GetTemporary(resultTexture.width, resultTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            Graphics.Blit(resultTexture, tempRT);

            Texture2D exportTexture = new(resultTexture.width, resultTexture.height, originalImage.format, false)
            {
                alphaIsTransparency = originalImage.alphaIsTransparency,
                wrapMode = originalImage.wrapMode,
                filterMode = originalImage.filterMode,
            };

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = tempRT;
            exportTexture.ReadPixels(new(0, 0, tempRT.width, tempRT.height), 0, 0);
            exportTexture.Apply();
            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(tempRT);

            return exportTexture;
        }

        public void SetTextureImporterSettings(string relativePath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (importer != null)
            {
                if (originalImageImporterSettings != null)
                {
                    importer.SetTextureSettings(originalImageImporterSettings);
                    importer.maxTextureSize = originalMaxTextureSize;
                    importer.textureCompression = originalTextureCompression;
                    importer.SaveAndReimport();
                }
            }
        }

        public static List<string> GetAvailableFilters()
        {
            return new(filters.Keys);
        }
        #endregion

        #region Methods
        public void ApplyAdjustments()
        {
            if (behaviourMode == BehaviourMode.Synced)
            {
                if (isReadbackInProgress) return;
                isReadbackInProgress = true;
            }

            SetShaderFloatIfChanged("VibranceMagnitude", VibranceMagnitude, ref prevVibranceMagnitude);
            SetShaderFloatIfChanged("SaturationMagnitude", SaturationMagnitude, ref prevSaturationMagnitude);
            SetShaderFloatIfChanged("TemperatureMagnitude", TemperatureMagnitude, ref prevTemperatureMagnitude);
            SetShaderFloatIfChanged("TintMagnitude", TintMagnitude, ref prevTintMagnitude);
            SetShaderFloatIfChanged("HueMagnitude", HueMagnitude, ref prevHueMagnitude);
            SetShaderFloatIfChanged("BrightnessMagnitude", BrightnessMagnitude, ref prevBrightnessMagnitude);
            SetShaderFloatIfChanged("ExposureMagnitude", ExposureMagnitude, ref prevExposureMagnitude);
            SetShaderFloatIfChanged("ContrastMagnitude", ContrastMagnitude, ref prevContrastMagnitude);
            SetShaderFloatIfChanged("BlackMagnitude", BlackMagnitude, ref prevBlackMagnitude);
            SetShaderFloatIfChanged("WhiteMagnitude", WhiteMagnitude, ref prevWhiteMagnitude);
            SetShaderFloatIfChanged("ShadowMagnitude", ShadowMagnitude, ref prevShadowMagnitude);
            SetShaderFloatIfChanged("PixelateMagnitude", PixelateMagnitude, ref prevPixelateMagnitude);
            SetShaderFloatIfChanged("SharpenMagnitude", SharpenMagnitude, ref prevSharpenMagnitude);

            computeShader.Dispatch(mainKernelHandle, threadGroupsX, threadGroupsY, 1);

            if (behaviourMode == BehaviourMode.Synced)
            {
                AsyncGPUReadback.Request(resultTexture, 0, TextureFormat.RGBAFloat, OnCompleteReadback);
            }
            else
            {
                editorWindow.Repaint();
            }
        }

        private void SetShaderFloatIfChanged(string propertyName, float currentValue, ref float previousValue)
        {
            if (currentValue != previousValue)
            {
                computeShader.SetFloat(propertyName, currentValue);
                previousValue = currentValue;
            }
        }

        private void OnCompleteReadback(AsyncGPUReadbackRequest request)
        {
            isReadbackInProgress = false;

            if (request.hasError)
            {
                Debug.LogError("GPU readback error detected.");
                return;
            }

            readbackTexture.LoadRawTextureData(request.GetData<byte>());
            readbackTexture.Apply();

            EditorApplication.delayCall += () =>
            {
                editorWindow.Repaint();
            };
        }

        public void ApplyFilter(string filterName)
        {
            if (filters.TryGetValue(filterName, out Filter filter))
            {
                filter.ApplyTo(this);
                ApplyAdjustments();
            }
            else
            {
                Debug.LogWarning($"Filter '{filterName}' not found.");
            }
        }

        public void ResetImageEffects()
        {
            ResetMagnitudeValues();
            ResetComputeShaderParameters();
            ApplyAdjustments();
            Graphics.Blit(originalImage, resultTexture);
        }

        private void ResetMagnitudeValues()
        {
            VibranceMagnitude = 0f;
            SaturationMagnitude = 0f;
            TemperatureMagnitude = 0f;
            TintMagnitude = 0f;
            HueMagnitude = 0f;
            BrightnessMagnitude = 0f;
            ExposureMagnitude = 0f;
            ContrastMagnitude = 0f;
            BlackMagnitude = 0f;
            WhiteMagnitude = 0f;
            ShadowMagnitude = 0f;
            PixelateMagnitude = 0f;
            SharpenMagnitude = 0f;
        }

        private void ResetComputeShaderParameters()
        {
            computeShader.SetFloat("VibranceMagnitude", VibranceMagnitude);
            computeShader.SetFloat("SaturationMagnitude", SaturationMagnitude);
            computeShader.SetFloat("TemperatureMagnitude", TemperatureMagnitude);
            computeShader.SetFloat("TintMagnitude", TintMagnitude);
            computeShader.SetFloat("HueMagnitude", HueMagnitude);
            computeShader.SetFloat("BrightnessMagnitude", BrightnessMagnitude);
            computeShader.SetFloat("ExposureMagnitude", ExposureMagnitude);
            computeShader.SetFloat("ContrastMagnitude", ContrastMagnitude);
            computeShader.SetFloat("BlackMagnitude", BlackMagnitude);
            computeShader.SetFloat("WhiteMagnitude", WhiteMagnitude);
            computeShader.SetFloat("ShadowMagnitude", ShadowMagnitude);
            computeShader.SetFloat("PixelateMagnitude", PixelateMagnitude);
            computeShader.SetFloat("SharpenMagnitude", SharpenMagnitude);
        }

        private void CacheAdjustmentsParameters()
        {
            prevVibranceMagnitude = VibranceMagnitude;
            prevSaturationMagnitude = SaturationMagnitude;
            prevTemperatureMagnitude = TemperatureMagnitude;
            prevTintMagnitude = TintMagnitude;
            prevHueMagnitude = HueMagnitude;
            prevBrightnessMagnitude = BrightnessMagnitude;
            prevExposureMagnitude = ExposureMagnitude;
            prevContrastMagnitude = ContrastMagnitude;
            prevBlackMagnitude = BlackMagnitude;
            prevWhiteMagnitude = WhiteMagnitude;
            prevShadowMagnitude = ShadowMagnitude;
            prevPixelateMagnitude = PixelateMagnitude;
            prevSharpenMagnitude = SharpenMagnitude;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            ResetMagnitudeValues();
            ResetComputeShaderParameters();

            if (resultTexture != null)
            {
                if (RenderTexture.active == resultTexture) { RenderTexture.active = null; }
                resultTexture.Release();
                UnityEngine.Object.DestroyImmediate(resultTexture);
                resultTexture = null;
            }

            if (readbackTexture != null)
            {
                UnityEngine.Object.DestroyImmediate(readbackTexture);
                readbackTexture = null;
            }
        }
        #endregion

        #region Modifiables
        public static void EnsureComputeShaderProperties()
        {
            string computeShaderPath = PicEaseLite_File.GetShaderFilePath(PicEaseLite_Constants.ComputeShader);
            if (string.IsNullOrEmpty(computeShaderPath))
            {
                Debug.LogError("Compute shader path is null or empty.");
                return;
            }

            string[] shaderLines = File.ReadAllLines(computeShaderPath);

            string currentPrecision = null;
            int currentThreadGroupSizeX = 0;
            int currentThreadGroupSizeY = 0;
            int currentThreadGroupSizeZ = 0;

            for (int i = 0; i < shaderLines.Length; i++)
            {
                string line = shaderLines[i];
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//") || trimmedLine.StartsWith("/*")) continue;

                Match matchPrecision = Regex.Match(trimmedLine, @"^(float|half)\s+\w+\s*;");
                if (matchPrecision.Success && currentPrecision == null)
                {
                    currentPrecision = matchPrecision.Groups[1].Value;
                    continue;
                }

                Match matchNumThreads = Regex.Match(trimmedLine, @"\[\s*numthreads\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*\]");
                if (matchNumThreads.Success)
                {
                    currentThreadGroupSizeX = int.Parse(matchNumThreads.Groups[1].Value);
                    currentThreadGroupSizeY = int.Parse(matchNumThreads.Groups[2].Value);
                    currentThreadGroupSizeZ = int.Parse(matchNumThreads.Groups[3].Value);
                    continue;
                }
            }

            string desiredPrecision = null;
            PrecisionMode precisionMode = PicEaseLite_Settings.ImageEditorPrecisionMode;
            if (precisionMode == PrecisionMode.Full)
            {
                desiredPrecision = "float";
            }
            else if (precisionMode == PrecisionMode.Half)
            {
                desiredPrecision = "half";
            }

            ThreadGroupSize threadGroupSize = PicEaseLite_Settings.ImageEditorThreadGroupSize;
            int desiredThreadGroupSizeX, desiredThreadGroupSizeY, desiredThreadGroupSizeZ = 1;
            switch (threadGroupSize)
            {
                case ThreadGroupSize.EightByEight:
                    desiredThreadGroupSizeX = 8;
                    desiredThreadGroupSizeY = 8;
                    break;
                case ThreadGroupSize.SixteenBySixteen:
                    desiredThreadGroupSizeX = 16;
                    desiredThreadGroupSizeY = 16;
                    break;
                case ThreadGroupSize.ThirtyTwoByThirtyTwo:
                    desiredThreadGroupSizeX = 32;
                    desiredThreadGroupSizeY = 32;
                    break;
                default:
                    desiredThreadGroupSizeX = 16;
                    desiredThreadGroupSizeY = 16;
                    break;
            }

            bool needsUpdate = false;
            if (currentPrecision != desiredPrecision)
            {
                needsUpdate = true;
            }

            if (currentThreadGroupSizeX != desiredThreadGroupSizeX || currentThreadGroupSizeY != desiredThreadGroupSizeY || currentThreadGroupSizeZ != desiredThreadGroupSizeZ)
            {
                needsUpdate = true;
            }

            if (!needsUpdate)
            {
                return;
            }

            StringBuilder modifiedShader = new();
            for (int i = 0; i < shaderLines.Length; i++)
            {
                string line = shaderLines[i];
                string modifiedLine = line;

                if (currentPrecision != desiredPrecision)
                {
                    if (currentPrecision == "float" && desiredPrecision == "half")
                    {
                        modifiedLine = ReplacePrecision(modifiedLine, "float", "half");
                    }
                    else if (currentPrecision == "half" && desiredPrecision == "float")
                    {
                        modifiedLine = ReplacePrecision(modifiedLine, "half", "float");
                    }
                }

                Match matchNumThreads = Regex.Match(line, @"\[\s*numthreads\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*\]");
                if (matchNumThreads.Success)
                {
                    modifiedLine = $"[numthreads({desiredThreadGroupSizeX}, {desiredThreadGroupSizeY}, {desiredThreadGroupSizeZ})]";
                }

                modifiedShader.AppendLine(modifiedLine);
            }

            File.WriteAllText(computeShaderPath, modifiedShader.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
            UpdateComputeShaderInstanceSessionCache(computeShaderPath);
        }

        private static string ReplacePrecision(string line, string fromPrecision, string toPrecision)
        {
            string[] patterns = { $"{fromPrecision}4", $"{fromPrecision}3", $"{fromPrecision}2", fromPrecision };
            foreach (string pattern in patterns)
            {
                string replacement = pattern.Replace(fromPrecision, toPrecision);
                line = Regex.Replace(line, $@"\b{pattern}\b", replacement);
            }
            return line;
        }

        private static void UpdateComputeShaderInstanceSessionCache(string shaderPath)
        {
            ComputeShader updatedComputeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(shaderPath);
            if (updatedComputeShader != null)
            {
                PicEaseLite_Session.instance.ComputeShaderImageProcessing = updatedComputeShader;
                PicEaseLite_Session.instance.IsComputeShaderImageProcessingLoaded = true;
            }
            else
            {
                Debug.LogError("Failed to reload the compute shader after modification.");
            }
        }
        #endregion
    }
}
#endif
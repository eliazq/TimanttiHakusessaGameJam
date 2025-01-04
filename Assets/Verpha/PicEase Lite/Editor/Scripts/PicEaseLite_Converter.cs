#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace Verpha.PicEase.Lite
{
    internal static class PicEaseLite_Converter
    {
        #region Properties
        public enum ExportImageFormat { OriginalImage, Settings}
        public enum ImageFormat { PNG, JPEG, JPG, TGA }
        #endregion

        #region Methods
        public static string GetImageFormatString()
        {
            Array imageFormats = Enum.GetValues(typeof(ImageFormat));
            string[] formatStrings = new string[imageFormats.Length];

            for (int i = 0; i < imageFormats.Length; i++)
            {
                formatStrings[i] = GetExtension((ImageFormat)imageFormats.GetValue(i));
            }

            return string.Join(",", formatStrings);
        }

        public static string GetExtension(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => "png",
                ImageFormat.JPG or ImageFormat.JPEG => "jpg",
                ImageFormat.TGA => "tga",
                _ => "png",
            };
        }

        public static ImageFormat GetImageFormatFromExtension(string extension)
        {
            return extension switch
            {
                ".png" => ImageFormat.PNG,
                ".jpg" or "jpeg" => ImageFormat.JPG,
                ".tga" => ImageFormat.TGA,
                _ => PicEaseLite_Settings.ImageEditorDefaultExportImageFormat,
            };
        }

        public static byte[] EncodeImage(Texture2D image, ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => image.EncodeToPNG(),
                ImageFormat.JPG or ImageFormat.JPEG => image.EncodeToJPG(),
                ImageFormat.TGA => image.EncodeToTGA(),
                _ => image.EncodeToPNG(),
            };
        }

        #region TGA
        public static Texture2D LoadImageTGA(byte[] fileData)
        {
            using BinaryReader reader = new(new MemoryStream(fileData));

            byte idLength = reader.ReadByte();
            byte colorMapType = reader.ReadByte();
            byte imageType = reader.ReadByte();
            short colorMapFirstEntryIndex = reader.ReadInt16();
            short colorMapLength = reader.ReadInt16();
            byte colorMapEntrySize = reader.ReadByte();
            short xOrigin = reader.ReadInt16();
            short yOrigin = reader.ReadInt16();
            short width = reader.ReadInt16();
            short height = reader.ReadInt16();
            byte pixelDepth = reader.ReadByte();
            byte imageDescriptor = reader.ReadByte();

            if (idLength > 0)
            {
                reader.ReadBytes(idLength);
            }

            byte[] colorMapData = null;
            if (colorMapType == 1)
            {
                int colorMapEntryByteSize = colorMapEntrySize / 8;
                int colorMapSize = colorMapLength * colorMapEntryByteSize;
                colorMapData = reader.ReadBytes(colorMapSize);
            }

            switch (imageType)
            {
                case 0:
                    Debug.LogError("No image data included in TGA file.");
                    return null;
                case 1:
                case 2:
                case 3:
                    return LoadUncompressedTGA(reader, imageType, width, height, pixelDepth, imageDescriptor, colorMapData, colorMapEntrySize, colorMapLength);
                case 9:
                case 10:
                case 11:
                    return LoadCompressedTGA(reader, imageType, width, height, pixelDepth, imageDescriptor, colorMapData, colorMapEntrySize, colorMapLength);
                case 32:
                case 33:
                    Debug.LogError($"Unsupported TGA data type (Huffman compressed): {imageType}");
                    return null;
                default:
                    Debug.LogError($"Unsupported TGA data type: {imageType}");
                    return null;
            }
        }

        private static Texture2D LoadUncompressedTGA(BinaryReader reader, byte imageType, int width, int height, byte pixelDepth, byte imageDescriptor, byte[] colorMapData, byte colorMapEntrySize, int colorMapLength)
        {
            int bytesPerPixel = pixelDepth / 8;
            int imageSize = width * height;

            bool flipVertically = (imageDescriptor & 0x20) != 0;
            bool flipHorizontally = (imageDescriptor & 0x10) != 0;

            byte[] imageData;
            if (imageType == 2)
            {
                imageData = reader.ReadBytes(width * height * bytesPerPixel);
                for (int i = 0; i < imageData.Length; i += bytesPerPixel)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
            }
            else if (imageType == 3)
            {
                if (pixelDepth != 8)
                {
                    Debug.LogError("Unsupported pixel depth for grayscale image.");
                    return null;
                }

                byte[] grayscaleData = reader.ReadBytes(imageSize);
                imageData = new byte[imageSize * 3];
                for (int i = 0, j = 0; i < grayscaleData.Length; i++, j += 3)
                {
                    byte gray = grayscaleData[i];
                    imageData[j] = gray;
                    imageData[j + 1] = gray;
                    imageData[j + 2] = gray;
                }
                bytesPerPixel = 3;
            }
            else if (imageType == 1)
            {
                int indexBytes = pixelDepth / 8;
                byte[] imageIndices = reader.ReadBytes(imageSize * indexBytes);

                int colorMapEntryBytes = colorMapEntrySize / 8;
                if (colorMapData == null)
                {
                    Debug.LogError("Color map data is missing for color-mapped image.");
                    return null;
                }

                imageData = new byte[imageSize * colorMapEntryBytes];
                for (int i = 0; i < imageSize; i++)
                {
                    int index = 0;
                    if (indexBytes == 1)
                    {
                        index = imageIndices[i];
                    }
                    else if (indexBytes == 2)
                    {
                        index = BitConverter.ToUInt16(imageIndices, i * 2);
                    }

                    if (index >= colorMapLength)
                    {
                        Debug.LogError("Color map index out of range.");
                        return null;
                    }
                    Array.Copy(colorMapData, index * colorMapEntryBytes, imageData, i * colorMapEntryBytes, colorMapEntryBytes);
                }

                for (int i = 0; i < imageData.Length; i += colorMapEntryBytes)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
                bytesPerPixel = colorMapEntryBytes;
            }
            else
            {
                Debug.LogError($"Unsupported uncompressed image type: {imageType}");
                return null;
            }

            if (flipVertically)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    Array.Copy(imageData, row * rowSize, flippedImageData, (height - row - 1) * rowSize, rowSize);
                }
                imageData = flippedImageData;
            }
            if (flipHorizontally)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int srcIndex = row * rowSize + col * bytesPerPixel;
                        int destIndex = row * rowSize + (width - col - 1) * bytesPerPixel;
                        Array.Copy(imageData, srcIndex, flippedImageData, destIndex, bytesPerPixel);
                    }
                }
                imageData = flippedImageData;
            }

            TextureFormat format;
            if (bytesPerPixel == 4)
            {
                format = TextureFormat.RGBA32;
            }
            else if (bytesPerPixel == 3)
            {
                format = TextureFormat.RGB24;
            }
            else
            {
                Debug.LogError("Unsupported bytes per pixel.");
                return null;
            }

            Texture2D texture = new(width, height, format, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }

        private static Texture2D LoadCompressedTGA(BinaryReader reader, byte imageType, int width, int height, byte pixelDepth, byte imageDescriptor, byte[] colorMapData, byte colorMapEntrySize, int colorMapLength)
        {
            int bytesPerPixel = pixelDepth / 8;
            int pixelCount = width * height;
            int currentPixel = 0;
            int currentByte = 0;
            byte[] imageData = new byte[pixelCount * bytesPerPixel];

            bool flipVertically = (imageDescriptor & 0x20) != 0;
            bool flipHorizontally = (imageDescriptor & 0x10) != 0;

            try
            {
                while (currentPixel < pixelCount)
                {
                    byte packetHeader = reader.ReadByte();
                    int packetType = packetHeader & 0x80;
                    int packetSize = (packetHeader & 0x7F) + 1;

                    if (packetType != 0)
                    {
                        byte[] pixelData = reader.ReadBytes(bytesPerPixel);
                        for (int i = 0; i < packetSize; i++)
                        {
                            Array.Copy(pixelData, 0, imageData, currentByte, bytesPerPixel);
                            currentByte += bytesPerPixel;
                            currentPixel++;
                        }
                    }
                    else
                    {
                        int bytesToRead = packetSize * bytesPerPixel;
                        byte[] rawData = reader.ReadBytes(bytesToRead);
                        Array.Copy(rawData, 0, imageData, currentByte, bytesToRead);
                        currentByte += bytesToRead;
                        currentPixel += packetSize;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error decoding RLE compressed TGA: " + ex.Message);
                return null;
            }

            if (imageType == 10)
            {
                for (int i = 0; i < imageData.Length; i += bytesPerPixel)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
            }
            else if (imageType == 11)
            {
                if (pixelDepth != 8)
                {
                    Debug.LogError("Unsupported pixel depth for grayscale image.");
                    return null;
                }

                byte[] grayscaleData = imageData;
                imageData = new byte[pixelCount * 3];
                for (int i = 0, j = 0; i < grayscaleData.Length; i++, j += 3)
                {
                    byte gray = grayscaleData[i];
                    imageData[j] = gray;
                    imageData[j + 1] = gray;
                    imageData[j + 2] = gray;
                }
                bytesPerPixel = 3;
            }
            else if (imageType == 9)
            {
                int colorMapEntryBytes = colorMapEntrySize / 8;
                if (colorMapData == null)
                {
                    Debug.LogError("Color map data is missing for color-mapped image.");
                    return null;
                }
                byte[] indicesData = imageData;
                imageData = new byte[pixelCount * colorMapEntryBytes];
                for (int i = 0; i < pixelCount; i++)
                {
                    int index = indicesData[i];
                    if (index >= colorMapLength)
                    {
                        Debug.LogError("Color map index out of range.");
                        return null;
                    }
                    Array.Copy(colorMapData, index * colorMapEntryBytes, imageData, i * colorMapEntryBytes, colorMapEntryBytes);
                }
                for (int i = 0; i < imageData.Length; i += colorMapEntryBytes)
                {
                    byte temp = imageData[i];
                    imageData[i] = imageData[i + 2];
                    imageData[i + 2] = temp;
                }
                bytesPerPixel = colorMapEntryBytes;
            }

            if (flipVertically)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    Array.Copy(imageData, row * rowSize, flippedImageData, (height - row - 1) * rowSize, rowSize);
                }
                imageData = flippedImageData;
            }
            if (flipHorizontally)
            {
                int rowSize = width * bytesPerPixel;
                byte[] flippedImageData = new byte[imageData.Length];
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        int srcIndex = row * rowSize + col * bytesPerPixel;
                        int destIndex = row * rowSize + (width - col - 1) * bytesPerPixel;
                        Array.Copy(imageData, srcIndex, flippedImageData, destIndex, bytesPerPixel);
                    }
                }
                imageData = flippedImageData;
            }

            TextureFormat format;
            if (bytesPerPixel == 4)
            {
                format = TextureFormat.RGBA32;
            }
            else if (bytesPerPixel == 3)
            {
                format = TextureFormat.RGB24;
            }
            else
            {
                Debug.LogError("Unsupported bytes per pixel.");
                return null;
            }

            Texture2D texture = new(width, height, format, false);
            texture.LoadRawTextureData(imageData);
            texture.Apply();
            return texture;
        }
        #endregion
        #endregion
    }
}
#endif
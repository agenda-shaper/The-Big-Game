using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class BatchTextureImporter : EditorWindow
{
    private string directoryPath = "Assets/Resources/img";

    [MenuItem("Tools/Batch Texture Importer")]
    public static void ShowWindow()
    {
        GetWindow<BatchTextureImporter>("Batch Texture Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Import PNGs with Custom Settings", EditorStyles.boldLabel);
        
        GUILayout.Label($"Using directory: {directoryPath}");

        if (GUILayout.Button("Import PNGs"))
        {
            ImportPNGs();
        }
    }

    private void ImportPNGs()
    {
        if (string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath))
        {
            Debug.LogError("Invalid directory path.");
            return;
        }

        string[] pngFiles = Directory.GetFiles(directoryPath, "*.png");

        foreach (var pngPath in pngFiles)
        {
            string assetPath = pngPath.Replace(Directory.GetCurrentDirectory() + "\\", "").Replace("\\", "/");

            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Bilinear;

                // Apply changes
                AssetDatabase.ImportAsset(assetPath);
            }
        }

        AssetDatabase.Refresh();
    }
}

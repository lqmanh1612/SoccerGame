using UnityEditor;
using UnityEngine;

public class ConvertJammoToHumanoid {
    [MenuItem("Tools/Convert Jammo to Humanoid")]
    public static void DoConvert() {
        string[] searchPaths = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Jammo-Character" });
        foreach(var guid in searchPaths) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null && importer.animationType != ModelImporterAnimationType.Human) {
                importer.animationType = ModelImporterAnimationType.Human;
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.SaveAndReimport();
                Debug.Log($"Converted {path} to Humanoid");
            }
        }
        Debug.Log("Conversion complete!");
    }
}
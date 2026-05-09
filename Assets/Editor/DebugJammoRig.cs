using UnityEditor;
using UnityEngine;

public class DebugJammoRig {
    [MenuItem("Tools/Debug Jammo Rig")]
    public static void DebugRig() {
        string[] searchPaths = AssetDatabase.FindAssets("Jammo t:Model");
        foreach(var guid in searchPaths) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer != null) {
                Debug.Log($"Model: {path}, Animation Type: {importer.animationType}");
            }
        }
    }
}
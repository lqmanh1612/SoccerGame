using UnityEditor;
using UnityEngine;
using System.Text;
using System.IO;

public class DumpBonesToFile {
    [MenuItem("Tools/Dump Bones To File")]
    public static void DoDump() {
        StringBuilder sb = new StringBuilder();
        DumpModel("Assets/Jammo-Character/Models/Jammo_LowPoly.fbx", sb);
        DumpModel("Assets/Res/Kick Soccerball.fbx", sb);
        File.WriteAllText("bone_dump.txt", sb.ToString());
        Debug.Log("Dumped to bone_dump.txt");
    }

    static void DumpModel(string path, StringBuilder sb) {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) return;
        sb.AppendLine("Bones for " + go.name + ":");
        DumpTransform(go.transform, "", sb);
    }

    static void DumpTransform(Transform t, string path, StringBuilder sb) {
        string currentPath = string.IsNullOrEmpty(path) ? t.name : path + "/" + t.name;
        sb.AppendLine(currentPath);
        foreach (Transform child in t) {
            DumpTransform(child, currentPath, sb);
        }
    }
}
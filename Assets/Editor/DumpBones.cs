using UnityEditor;
using UnityEngine;
using System.Text;

public class DumpBones {
    [MenuItem("Tools/Dump Bones")]
    public static void DoDump() {
        DumpModel("Assets/Jammo-Character/Models/Jammo_LowPoly.fbx");
        DumpModel("Assets/Res/Kick Soccerball.fbx");
    }

    static void DumpModel(string path) {
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (go == null) return;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Bones for " + go.name + ":");
        DumpTransform(go.transform, "", sb);
        Debug.Log(sb.ToString());
    }

    static void DumpTransform(Transform t, string path, StringBuilder sb) {
        string currentPath = string.IsNullOrEmpty(path) ? t.name : path + "/" + t.name;
        sb.AppendLine(currentPath);
        foreach (Transform child in t) {
            DumpTransform(child, currentPath, sb);
        }
    }
}
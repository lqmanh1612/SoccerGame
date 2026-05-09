using UnityEditor;
using UnityEngine;
using System.IO;

public class CheckFBX {
    [MenuItem("Tools/Check FBX Bindings")]
    public static void DoCheck() {
        string fbxPath = "Assets/Res/Kick Soccerball.fbx";
        string report = "FBX Path: " + fbxPath + "\n";

        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer != null) {
            report += "Animation Type: " + importer.animationType + "\n";
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        AnimationClip clip = null;
        foreach (var a in assets) {
            if (a is AnimationClip && !a.name.StartsWith("__preview__")) {
                clip = (AnimationClip)a;
                break;
            }
        }

        if (clip != null) {
            report += "Clip Name: " + clip.name + "\n";
            var bindings = AnimationUtility.GetCurveBindings(clip);
            report += "Total Bindings: " + bindings.Length + "\n";
            
            for (int i = 0; i < bindings.Length; i++) {
                if (!string.IsNullOrEmpty(bindings[i].path)) {
                    report += "First Non-Empty Path at [" + i + "]: " + bindings[i].path + "\n";
                    break;
                }
            }
            
            int count = 0;
            for (int i = 0; i < bindings.Length && count < 10; i++) {
                if (!string.IsNullOrEmpty(bindings[i].path)) {
                    report += "Binding[" + i + "]: " + bindings[i].path + "\n";
                    count++;
                }
            }
        } else {
            report += "No AnimationClip found in FBX.\n";
        }

        string jammoPath = "Assets/Jammo-Character/Models/Jammo_LowPoly.fbx";
        GameObject jammo = AssetDatabase.LoadAssetAtPath<GameObject>(jammoPath);
        if (jammo != null) {
            report += "\nJammo Structure:\n";
            Transform armature = jammo.transform.Find("Armature.001");
            if (armature != null) {
                report += "Found Armature.001\n";
                if (armature.childCount > 0) {
                    report += "First Child of Armature.001: " + armature.GetChild(0).name + "\n";
                }
            } else {
                report += "Armature.001 NOT found.\n";
            }
        }

        File.WriteAllText("Assets/Res/FBX_Report.txt", report);
        Debug.Log("FBX Report written to Assets/Res/FBX_Report.txt");
    }
}

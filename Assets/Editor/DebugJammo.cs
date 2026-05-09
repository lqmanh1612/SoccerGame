using UnityEditor;
using UnityEngine;

public class DebugJammo {
    [MenuItem("Tools/Debug Jammo Rig")]
    public static void PrintRig() {
        string fbxPath = "Assets/Res/Kick Soccerball.fbx";
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        AnimationClip sourceClip = null;
        foreach (Object asset in assets) {
            if (asset is AnimationClip && !asset.name.StartsWith("__preview__")) {
                sourceClip = asset as AnimationClip;
                break;
            }
        }
        
        string result = "";
        if (sourceClip != null) {
            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(sourceClip);
            if (bindings.Length > 0) {
                result += "FIRST KICK BONE: " + bindings[0].path + "\n";
            }
        }

        string jammoPath = "Assets/Jammo-Character/Models/Jammo_LowPoly.fbx";
        GameObject jammo = AssetDatabase.LoadAssetAtPath<GameObject>(jammoPath);
        if (jammo != null) {
            Transform armature = jammo.transform.Find("Armature.001");
            if (armature != null && armature.childCount > 0) {
                result += "JAMMO FIRST BONE: " + armature.GetChild(0).name + "\n";
            }
        }
        System.IO.File.WriteAllText("Assets/Res/BoneDebug.txt", result);
    }
}

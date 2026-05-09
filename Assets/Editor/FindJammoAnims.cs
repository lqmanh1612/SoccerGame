using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

public class FindJammoAnims {
    [MenuItem("Tools/Find Jammo Anims")]
    public static void FindAnims() {
        string controllerPath = "Assets/Jammo-Character/Animations/AnimatorController_Jamo.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null) return;
        
        HashSet<string> paths = new HashSet<string>();
        foreach (var clip in controller.animationClips) {
            if (clip != null) {
                string path = AssetDatabase.GetAssetPath(clip);
                paths.Add(path);
                Debug.Log($"Clip: {clip.name}, Path: {path}");
            }
        }
    }
}
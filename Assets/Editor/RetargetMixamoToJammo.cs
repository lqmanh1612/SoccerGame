using UnityEditor;
using UnityEngine;

public class RetargetMixamoToJammo {
    [MenuItem("Tools/Retarget Kick To Jammo")]
    public static void DoRetarget() {
        string fbxPath = "Assets/Res/Kick Soccerball.fbx";
        string newAnimPath = "Assets/Res/Kick_Generic.anim";
        
        // 1. Revert Jammo back to Generic
        string jammoPath = "Assets/Jammo-Character/Models/Jammo_LowPoly.fbx";
        ModelImporter jammoImporter = AssetImporter.GetAtPath(jammoPath) as ModelImporter;
        if (jammoImporter != null && jammoImporter.animationType != ModelImporterAnimationType.Generic) {
            jammoImporter.animationType = ModelImporterAnimationType.Generic;
            jammoImporter.SaveAndReimport();
            Debug.Log("Reverted Jammo to Generic.");
        }

        // 1b. Force Source FBX to Generic
        ModelImporter sourceImporter = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (sourceImporter != null && sourceImporter.animationType != ModelImporterAnimationType.Generic) {
            sourceImporter.animationType = ModelImporterAnimationType.Generic;
            sourceImporter.SaveAndReimport();
            Debug.Log("Set Source FBX to Generic.");
        }

        // 2. Find clip in FBX
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        AnimationClip sourceClip = null;
        foreach (Object asset in assets) {
            if (asset is AnimationClip && !asset.name.StartsWith("__preview__")) {
                sourceClip = asset as AnimationClip;
                break;
            }
        }

        if (sourceClip == null) {
            Debug.LogError("No source clip found!");
            return;
        }

        // 3. Create new clip and copy curves with modified paths
        AnimationClip newClip = new AnimationClip();
        newClip.name = "Kick_Generic";
        // Copy settings
        AnimationUtility.SetAnimationClipSettings(newClip, AnimationUtility.GetAnimationClipSettings(sourceClip));
        newClip.frameRate = sourceClip.frameRate;

        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(sourceClip);
        if (bindings.Length > 0) Debug.Log("First original path: " + bindings[0].path);
        
        foreach (var binding in bindings) {
            string newPath = binding.path;
            if (newPath == "") {
                newPath = "Armature.001";
            } else {
                string[] parts = newPath.Split('/');
                for (int i = 0; i < parts.Length; i++) {
                    if (!parts[i].StartsWith("mixamorig:")) {
                        parts[i] = "mixamorig:" + parts[i];
                    }
                }
                newPath = "Armature.001/" + string.Join("/", parts);
            }
            EditorCurveBinding newBinding = binding;
            newBinding.path = newPath;
            AnimationCurve curve = AnimationUtility.GetEditorCurve(sourceClip, binding);
            AnimationUtility.SetEditorCurve(newClip, newBinding, curve);
        }

        if (bindings.Length > 0) Debug.Log("First new path: " + bindings[0].path);

        AssetDatabase.CreateAsset(newClip, newAnimPath);
        AssetDatabase.SaveAssets();
        Debug.Log("Created generic kick animation at " + newAnimPath);

        // 4. Update Animator Controller
        string controllerPath = "Assets/Jammo-Character/Animations/AnimatorController_Jamo.controller";
        UnityEditor.Animations.AnimatorController controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
        if (controller != null) {
            foreach (var state in controller.layers[0].stateMachine.states) {
                if (state.state.name == "Kick") {
                    state.state.motion = newClip;
                    EditorUtility.SetDirty(controller);
                    break;
                }
            }
            AssetDatabase.SaveAssets();
            Debug.Log("Updated Animator Controller!");
        }
    }
}
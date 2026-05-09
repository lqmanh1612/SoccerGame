using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class SetupKickAnim {
    [MenuItem("Tools/Setup Kick Animation")]
    public static void DoSetup() {
        string fbxPath = "Assets/Res/Kick Soccerball.fbx";
        string controllerPath = "Assets/Jammo-Character/Animations/AnimatorController_Jamo.controller";

        // 1. Ensure FBX is set to Humanoid
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer != null) {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.SaveAndReimport();
            Debug.Log("Set FBX to Humanoid.");
        } else {
            Debug.LogError("Could not find FBX at " + fbxPath);
        }

        // 2. Find Animation Clip in FBX
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        AnimationClip kickClip = null;
        foreach (Object asset in assets) {
            if (asset is AnimationClip && !asset.name.StartsWith("__preview__")) {
                kickClip = asset as AnimationClip;
                break;
            }
        }

        if (kickClip == null) {
            Debug.LogError("No Animation Clip found in FBX!");
            return;
        }
        Debug.Log("Found clip: " + kickClip.name);

        // 3. Add to Animator Controller
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null) {
            Debug.LogError("Could not find AnimatorController at " + controllerPath);
            return;
        }

        // Check if parameter exists
        bool hasParam = false;
        foreach (var p in controller.parameters) {
            if (p.name == "Kick") {
                hasParam = true;
                break;
            }
        }
        if (!hasParam) {
            controller.AddParameter("Kick", AnimatorControllerParameterType.Trigger);
        }

        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        // Check if state exists
        AnimatorState kickState = null;
        foreach (var childState in rootStateMachine.states) {
            if (childState.state.name == "Kick") {
                kickState = childState.state;
                break;
            }
        }

        if (kickState == null) {
            kickState = rootStateMachine.AddState("Kick");
        }
        kickState.motion = kickClip;

        // Add transitions
        // AnyState -> Kick
        AnimatorStateTransition anyToKick = null;
        foreach (var t in rootStateMachine.anyStateTransitions) {
            if (t.destinationState == kickState) {
                anyToKick = t;
                break;
            }
        }

        if (anyToKick == null) {
            anyToKick = rootStateMachine.AddAnyStateTransition(kickState);
            anyToKick.AddCondition(AnimatorConditionMode.If, 0, "Kick");
            anyToKick.hasExitTime = false;
            anyToKick.duration = 0.1f;
        }

        // Kick -> NormalStatus (assuming NormalStatus is the default state)
        AnimatorState normalState = null;
        foreach (var childState in rootStateMachine.states) {
            if (childState.state.name == "NormalStatus") {
                normalState = childState.state;
                break;
            }
        }

        if (normalState != null) {
            bool hasExit = false;
            foreach (var t in kickState.transitions) {
                if (t.destinationState == normalState) {
                    hasExit = true;
                    break;
                }
            }
            if (!hasExit) {
                AnimatorStateTransition exitTransition = kickState.AddTransition(normalState);
                exitTransition.hasExitTime = true;
                exitTransition.exitTime = 0.85f; // Wait till almost end
                exitTransition.duration = 0.15f;
            }
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        Debug.Log("Kick animation setup complete!");
    }
}
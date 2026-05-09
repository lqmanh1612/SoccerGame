using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class DebugKickAnim {
    [MenuItem("Tools/Debug Kick Animation")]
    public static void DebugAnim() {
        string fbxPath = "Assets/Res/Kick Soccerball.fbx";
        
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer != null) {
            Debug.Log("FBX Animation Type: " + importer.animationType);
            Debug.Log("FBX Avatar Setup: " + importer.avatarSetup);
            if (importer.avatarSetup != ModelImporterAvatarSetup.CreateFromThisModel) {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.SaveAndReimport();
                Debug.Log("Forced Avatar Setup to CreateFromThisModel.");
            }
        }
        
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
        foreach (Object asset in assets) {
            if (asset is AnimationClip) {
                AnimationClip clip = asset as AnimationClip;
                Debug.Log($"Clip: {clip.name}, Length: {clip.length}, Legacy: {clip.legacy}");
            }
            if (asset is Avatar) {
                Avatar avatar = asset as Avatar;
                Debug.Log($"Avatar: {avatar.name}, isValid: {avatar.isValid}, isHuman: {avatar.isHuman}");
            }
        }
    }
}
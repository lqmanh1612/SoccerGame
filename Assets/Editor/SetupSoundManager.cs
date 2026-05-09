using UnityEditor;
using UnityEngine;

public class SetupSoundManager {
    [MenuItem("Tools/Setup Sound Manager")]
    public static void Setup() {
        SoundManager existing = Object.FindAnyObjectByType<SoundManager>();
        if (existing != null) {
            Debug.Log("SoundManager already exists in scene.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        GameObject smObj = new GameObject("SoundManager", typeof(SoundManager));
        SoundManager sm = smObj.GetComponent<SoundManager>();
        
        // Try to auto-assign the kick sound found in Assets
        string kickPath = "Assets/Res/SoundEffect/Football kick  Football dribbling  Ball kick - sound effect.mp3";
        AudioClip kickClip = AssetDatabase.LoadAssetAtPath<AudioClip>(kickPath);
        if (kickClip != null) {
            sm.kickClip = kickClip;
            Debug.Log("Auto-assigned kick sound effect.");
        }

        Undo.RegisterCreatedObjectUndo(smObj, "Create SoundManager");
        Selection.activeGameObject = smObj;
        Debug.Log("SoundManager created in scene. Please assign Ambient and Goal clips in the Inspector.");
    }
}

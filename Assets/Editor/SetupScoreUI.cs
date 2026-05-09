using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SetupScoreUI {
    [MenuItem("Tools/Setup Score UI")]
    public static void Setup() {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null) {
            Debug.LogError("No Canvas found in scene!");
            return;
        }

        SoccerUIManager manager = Object.FindAnyObjectByType<SoccerUIManager>();
        if (manager == null) {
            Debug.LogError("No SoccerUIManager found in scene!");
            return;
        }

        // 1. Create Score Text GameObject
        GameObject scoreObj = new GameObject("ScoreText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
        scoreObj.transform.SetParent(canvas.transform, false);
        
        Text txt = scoreObj.GetComponent<Text>();
        txt.text = "0-0";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 36;
        txt.fontStyle = FontStyle.Bold;
        txt.alignment = TextAnchor.UpperCenter;
        txt.color = new Color(1f, 0.8f, 0f); // Gold color
        
        Outline outline = scoreObj.GetComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        RectTransform rt = scoreObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 50);
        rt.anchorMin = new Vector2(0.5f, 1); // Top Center
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -20);
        
        // 2. Link to Manager
        manager.scoreText = txt;
        EditorUtility.SetDirty(manager);
        
        Selection.activeGameObject = scoreObj;
        Debug.Log("Score UI created and linked to SoccerUIManager!");
    }
}

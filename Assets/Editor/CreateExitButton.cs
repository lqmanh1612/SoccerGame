using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

public class CreateExitButton {
    [MenuItem("Tools/Create Exit Button")]
    public static void Create() {
        // 1. Find Canvas and Manager
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

        // 2. Create Button GameObject
        GameObject btnObj = new GameObject("ExitButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(canvas.transform, false);
        
        // 3. Setup Button Visuals (Reddish button)
        Image img = btnObj.GetComponent<Image>();
        img.color = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Reddish
        
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 40);
        rt.anchorMin = new Vector2(1, 1); // Top Right
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-20, -20);
        
        // 4. Create Text Child
        GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        txtObj.transform.SetParent(btnObj.transform, false);
        
        Text txt = txtObj.GetComponent<Text>();
        txt.text = "EXIT";
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.resizeTextForBestFit = true;
        
        RectTransform rtTxt = txtObj.GetComponent<RectTransform>();
        rtTxt.anchorMin = Vector2.zero;
        rtTxt.anchorMax = Vector2.one;
        rtTxt.sizeDelta = Vector2.zero;

        // 5. Link Event
        Button btn = btnObj.GetComponent<Button>();
        UnityEventTools.AddVoidPersistentListener(btn.onClick, manager.ExitGame);
        
        // 6. Assign to Manager field
        manager.exitButton = btn;
        EditorUtility.SetDirty(manager);
        
        Selection.activeGameObject = btnObj;
        Debug.Log("Exit Button created and linked to SoccerUIManager!");
    }
}

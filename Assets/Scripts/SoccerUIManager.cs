using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SoccerUIManager : MonoBehaviour
{
    [Header("References")]
    public PlayerBallInteraction playerInteraction;
    public Button kickButton;
    public Button autoKickButton;
    public Button resetButton;
    public GameObject goalEffectPrefab;

    [Header("Settings")]
    public float kickForce = 60f;
    public string goalNamePrefix = "soccer goal";
    public float goalDetectionDistance = 15f;

    private List<Transform> goals = new List<Transform>();

    void Start()
    {
        if (playerInteraction == null)
        {
            playerInteraction = FindObjectOfType<PlayerBallInteraction>();
        }

        InitializeGoals();

        if (kickButton != null)
        {
            kickButton.onClick.AddListener(KickNearestBall);
            kickButton.gameObject.SetActive(false);
        }
        
        if (autoKickButton != null)
        {
            autoKickButton.onClick.AddListener(AutoKickFurthestBall);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetScene);
        }
        
        Debug.Log($"[SoccerUI] Manager started. Goals found: {goals.Count}");
    }

    void InitializeGoals()
    {
        goals.Clear();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags != HideFlags.None || obj.scene.name == null) continue;
            if (obj.name.ToLower().StartsWith(goalNamePrefix.ToLower()))
            {
                goals.Add(obj.transform);
                Debug.Log($"[SoccerUI] Found goal: {obj.name} at {obj.transform.position}");
            }
        }
    }

    void Update()
    {
        if (playerInteraction == null) return;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (kickButton != null)
        {
            bool isNearBall = playerInteraction.NearestBall != null;
            if (kickButton.gameObject.activeSelf != isNearBall)
            {
                kickButton.gameObject.SetActive(isNearBall);
            }
        }
    }

    public void KickNearestBall()
    {
        if (playerInteraction == null || playerInteraction.NearestBall == null) return;
        Debug.Log("[SoccerUI] Kicking nearest ball.");
        KickBall(playerInteraction.NearestBall);
    }

    public void AutoKickFurthestBall()
    {
        if (playerInteraction == null) return;

        Rigidbody furthestBall = null;
        float maxDistance = -1f;

        foreach (Rigidbody ball in playerInteraction.AllBalls)
        {
            if (ball == null) continue;
            float distance = Vector3.Distance(playerInteraction.transform.position, ball.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestBall = ball;
            }
        }

        if (furthestBall != null) 
        {
            Debug.Log($"[SoccerUI] Auto-kicking furthest ball: {furthestBall.name}");
            KickBall(furthestBall);
        }
    }

    void KickBall(Rigidbody ball)
    {
        Transform nearestGoal = null;
        float minGoalDistance = float.MaxValue;

        foreach (Transform goal in goals)
        {
            if (goal == null) continue;
            float distance = Vector3.Distance(ball.position, goal.position);
            if (distance < minGoalDistance)
            {
                minGoalDistance = distance;
                nearestGoal = goal;
            }
        }

        if (nearestGoal != null)
        {
            Vector3 targetPos = nearestGoal.position;
            targetPos.y += 1.0f; 
            Vector3 direction = (targetPos - ball.position).normalized;
            ball.linearVelocity = Vector3.zero;
            ball.angularVelocity = Vector3.zero;
            ball.AddForce(direction * kickForce, ForceMode.Impulse);

            Debug.Log($"[SoccerUI] Ball kicked towards {nearestGoal.name}. Direction: {direction}");
            StartCoroutine(HandleCameraFollow(ball));
        }
        else
        {
            Debug.LogWarning("[SoccerUI] No goals found to kick towards!");
        }
    }

    private IEnumerator HandleCameraFollow(Rigidbody ball)
    {
        JammoSoccerController controller = playerInteraction.GetComponent<JammoSoccerController>();
        if (controller != null)
        {
            Debug.Log("[SoccerUI] Camera following ball...");
            controller.SetCameraTarget(ball.transform);
            
            float timer = 0;
            // Follow for up to 5 seconds as long as the ball is moving significantly
            while (timer < 5f && ball.linearVelocity.magnitude > 0.1f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"[SoccerUI] Ball slowed down or time out. Final Ball Pos: {ball.position}");

            bool effectTriggered = false;
            foreach (Transform goal in goals)
            {
                if (goal == null) continue;
                float distToGoal = Vector3.Distance(ball.position, goal.position);
                Debug.Log($"[SoccerUI] Distance to {goal.name}: {distToGoal} (Current Threshold: {goalDetectionDistance})");

                if (distToGoal < goalDetectionDistance)
                {
                    Debug.Log($"[SoccerUI] Goal proximity detected by UI for {goal.name}.");
                    effectTriggered = true;
                    break;
                }
            }

            if (!effectTriggered)
            {
                Debug.Log("[SoccerUI] No goal detected (ball too far from goals).");
            }
            
            yield return new WaitForSeconds(2f);
            controller.ResetCameraTarget();
            Debug.Log("[SoccerUI] Camera reset to player.");
        }
    }

    public void ResetScene()
    {
        Debug.Log("[SoccerUI] Resetting scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

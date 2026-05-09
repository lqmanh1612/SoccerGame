using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [Header("Settings")]
    public GameObject confettiPrefab;
    public string ballNamePrefix = "Soccer Ball";
    public float effectDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is a soccer ball
        if (other.name.StartsWith(ballNamePrefix))
        {
            Debug.Log("[GoalDetector] Goal Scored! Spawning confetti.");
            
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayGoal();

            if (SoccerUIManager.Instance != null)
                SoccerUIManager.Instance.RegisterGoal();
            
            // Spawn confetti at the goal's position
            if (confettiPrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 3f;
                GameObject confetti = Instantiate(confettiPrefab, spawnPos, Quaternion.identity);
                
                // Force play particle systems
                ParticleSystem[] systems = confetti.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in systems)
                {
                    ps.Play();
                }

                Destroy(confetti, effectDuration);
            }
            else
            {
                Debug.LogWarning("[GoalDetector] Confetti prefab is not assigned!");
            }
            
            // Optional: Reset ball position or play sound
            // ResetBall(other.gameObject);
        }
    }

    private void ResetBall(GameObject ball)
    {
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        ball.transform.position = Vector3.zero; // Spawn at center
    }
}

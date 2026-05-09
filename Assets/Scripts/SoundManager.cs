using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip ambientClip;
    public AudioClip kickClip;
    public AudioClip goalClip;
    public AudioClip resetClip;
    public AudioClip easterEggClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayAmbient();
    }

    private void SetupSources()
    {
        if (ambientSource == null)
        {
            ambientSource = gameObject.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayAmbient()
    {
        if (ambientClip != null && ambientSource != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.Play();
        }
    }

    public void PlayKick()
    {
        if (kickClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(kickClip);
        }
    }

    public void PlayGoal()
    {
        if (goalClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(goalClip);
        }
    }

    public void PlayReset()
    {
        if (resetClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(resetClip);
        }
        ResetAmbient();
    }

    public void ResetAmbient()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
            ambientSource.clip = ambientClip; // Restore original clip
            ambientSource.loop = true;        // Ensure it loops
            ambientSource.time = 0;
            ambientSource.Play();
        }
    }

    public void PlayEasterEgg()
    {
        if (easterEggClip != null && ambientSource != null)
        {
            StopAllCoroutines();
            StartCoroutine(EasterEggSequence());
        }
    }

    private System.Collections.IEnumerator EasterEggSequence()
    {
        ambientSource.Stop();
        ambientSource.clip = easterEggClip;
        ambientSource.loop = false; // Play once
        ambientSource.Play();

        // Wait until it finishes
        yield return new WaitForSeconds(easterEggClip.length);

        // Return to original ambient
        ResetAmbient();
    }

    public void StopAll()
    {
        ambientSource?.Stop();
        sfxSource?.Stop();
    }
}

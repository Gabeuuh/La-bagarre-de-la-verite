using UnityEngine;
using System.Collections;

[System.Serializable]
public class SlotVoice
{
    [Header("Identification du slot")]
    public string slotId;              // "SUJET", "ACTION", "LIEU", "MOTIF_VRAI", "FAKE_NEWS", "OBJECTIF_REGIME"

    [Header("Voix principale (histoire)")]
    public AudioClip storyClip;        // Ex : "Le 12 mars 2023..., le gardien..."

    [Header("Variantes de rappel")]
    public AudioClip[] repeatClips;    // Ex : "J'ai dit le gardien.", "C'est le gardien qui m'a agressé."
}

public class NarrationManager : MonoBehaviour
{
    public static NarrationManager Instance { get; private set; }

    [Header("Source audio")]
    public AudioSource audioSource;

    [Header("Voix par slot")]
    public SlotVoice[] slotVoices;

    [Header("Timing")]
    [Tooltip("Temps de silence entre deux répétitions de rappel.")]
    public float repeatDelay = 1.0f;

    private Coroutine _slotRoutine;
    private string _currentSlotId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// Lance la narration pour un slot donné (histoire + répétitions).
    /// </summary>
    public void StartSlotNarration(string slotId)
    {
        _currentSlotId = slotId;

        if (_slotRoutine != null)
            StopCoroutine(_slotRoutine);

        _slotRoutine = StartCoroutine(SlotRoutine(slotId));
    }

    /// <summary>
    /// Stoppe toute narration en cours.
    /// </summary>
    public void StopSlotNarration()
    {
        if (_slotRoutine != null)
        {
            StopCoroutine(_slotRoutine);
            _slotRoutine = null;
        }

        if (audioSource != null)
            audioSource.Stop();
    }

    /// <summary>
    /// Coroutine qui lit l'histoire une fois, puis répète des rappels
    /// tant que le slot n'est pas rempli dans le LetterManager.
    /// </summary>
    private IEnumerator SlotRoutine(string slotId)
    {
        SlotVoice voice = GetSlotVoice(slotId);
        if (voice == null)
            yield break;

        // 1) Lire l’histoire avec le vrai mot
        if (voice.storyClip != null)
        {
            PlayClip(voice.storyClip);
            yield return new WaitForSeconds(voice.storyClip.length + 0.2f);
        }

        // 2) Tant que le slot n'est pas rempli, on rejoue un des repeatClips
        while (LetterManager.Instance != null &&
               !LetterManager.Instance.IsSlotFilled(slotId))
        {
            AudioClip repeat = GetRandomRepeatClip(voice);
            if (repeat != null)
            {
                PlayClip(repeat);
                yield return new WaitForSeconds(repeat.length + repeatDelay);
            }
            else
            {
                // S'il n'y a aucun repeatClip configuré, on évite un while ultra-rapide
                yield return new WaitForSeconds(1f);
            }
        }

        _slotRoutine = null;
    }

    private SlotVoice GetSlotVoice(string slotId)
    {
        if (slotVoices == null)
            return null;

        foreach (var sv in slotVoices)
        {
            if (sv.slotId == slotId)
                return sv;
        }

        Debug.LogWarning("[NarrationManager] Pas de SlotVoice configuré pour slotId : " + slotId);
        return null;
    }

    private AudioClip GetRandomRepeatClip(SlotVoice voice)
    {
        if (voice.repeatClips == null || voice.repeatClips.Length == 0)
            return null;

        int index = Random.Range(0, voice.repeatClips.Length);
        return voice.repeatClips[index];
    }

    /// <summary>
    /// Appelé par le LetterManager quand un slot est enfin rempli
    /// (le bon mot a été posé et accepté).
    /// </summary>
    public void OnSlotFilled(string slotId)
    {
        if (slotId == _currentSlotId)
        {
            StopSlotNarration();
        }
    }
}

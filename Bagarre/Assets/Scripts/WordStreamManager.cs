using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class WordOption
{
    public string text;     // texte affiché sur le mot
    public string slotId;   // "SUJET", "ACTION", "LIEU", "MOTIF_VRAI", "FAKE_NEWS", "OBJECTIF_REGIME"
    public bool isTrueWord;
}

public class WordStreamManager : MonoBehaviour
{
    [Header("Références")]
    public WordData wordPrefab;
    public Transform spawnPoint;

    [Header("Déplacement")]
    public Vector3 moveDirection = Vector3.left;
    public float moveSpeed = 0.5f;
    public float wordLifeTime = 8f;

    [Header("Spawn")]
    public float spawnInterval = 1.5f;
    public int maxSimultaneousWords = 8;
    public WordOption[] options;

    [Header("Ordre des slots")]
    public string[] slotOrder = new string[]
    {
        "SUJET",
        "ACTION",
        "LIEU",
        "MOTIF_VRAI",
        "FAKE_NEWS",
        "OBJECTIF_REGIME"
    };

    private readonly List<WordData> _activeWords = new List<WordData>();
    private int _currentSlotIndex = 0;

    private void Start()
    {
        if (wordPrefab == null || spawnPoint == null || options == null || options.Length == 0)
        {
            Debug.LogWarning("[WordStreamManager] mal configuré, pas de spawn.");
            enabled = false;
            return;
        }

        Debug.Log($"[WordStreamManager] Start avec {options.Length} options. Slots order = {string.Join(",", slotOrder)}");
        StartCoroutine(SpawnLoop());

        if (slotOrder != null && slotOrder.Length > 0)
        {
            NarrationManager.Instance?.StartSlotNarration(slotOrder[_currentSlotIndex]);
        }
    }


    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            _activeWords.RemoveAll(w => w == null);

            if (LetterManager.Instance != null && LetterManager.Instance.IsSentenceComplete())
            {
                NarrationManager.Instance?.StopSlotNarration();
                yield break;
            }

            if (LetterManager.Instance != null && slotOrder != null && slotOrder.Length > 0)
            {
                bool slotChanged = false;

                // Avancer tant que les slots déjà remplis sont devant nous
                while (_currentSlotIndex < slotOrder.Length &&
                       LetterManager.Instance.IsSlotFilled(slotOrder[_currentSlotIndex]))
                {
                    _currentSlotIndex++;
                    slotChanged = true;
                }

                // Tous les slots de l’ordre sont remplis
                if (_currentSlotIndex >= slotOrder.Length)
                {
                    NarrationManager.Instance?.StopSlotNarration();
                    yield break;
                }

                // 👉 si on vient de changer de slot : lancer la narration pour ce nouveau type de mot
                if (slotChanged)
                {
                    string newSlotId = slotOrder[_currentSlotIndex];
                    NarrationManager.Instance?.StartSlotNarration(newSlotId);
                }
            }

            if (_activeWords.Count < maxSimultaneousWords)
            {
                SpawnWordForCurrentSlot();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnWordForCurrentSlot()
    {
        if (options == null || options.Length == 0)
            return;

        if (slotOrder == null || slotOrder.Length == 0)
        {
            Debug.LogWarning("[WordStreamManager] slotOrder vide, pas de spawn séquentiel.");
            return;
        }

        string targetSlotId = slotOrder[_currentSlotIndex];

        List<WordOption> candidates = new List<WordOption>();
        foreach (var opt in options)
        {
            if (opt.slotId == targetSlotId)
            {
                candidates.Add(opt);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"[WordStreamManager] Aucune WordOption pour slotId '{targetSlotId}'.");
            return;
        }

        WordOption option = candidates[Random.Range(0, candidates.Count)];

        WordData wd = Instantiate(wordPrefab, spawnPoint.position, spawnPoint.rotation);

        wd.slotId = option.slotId;
        wd.isTrueWord = option.isTrueWord;

        var tmp = wd.GetComponentInChildren<TextMeshPro>();
        if (tmp != null)
            tmp.text = option.text;

        var rb = wd.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        var mover = wd.gameObject.AddComponent<WordMover>();
        mover.Init(moveDirection, moveSpeed, wordLifeTime);

        _activeWords.Add(wd);
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;


public class LetterManager : MonoBehaviour
{
    public static LetterManager Instance { get; private set; }

    [Header("Texte sur la feuille")]
    public TextMeshPro letterText;   // LetterText sur la feuille

    [Header("Lettre de base")]
    [TextArea(3, 6)]
    public string baseSentence =
        "Le 12 mars 2023, vers 21 heures, alors que je revenais de la promenade, le _____SUJET_____ m’a _____ACTION_____ dans _____LIEU_____, alors que je _____MOTIF_VRAI_____.\nPourtant, dans le rapport officiel diffusé par l’administration, ils affirment que _____FAKE_NEWS_____, pour faire croire que _____OBJECTIF_REGIME_____.\nSi vous lisez cette lettre, c’est que leur censure n’a pas totalement réussi.";

    // on stocke les mots choisis par slotId
    private readonly Dictionary<string, string> _words = new Dictionary<string, string>();

    [Header("Décor / sortie")]
    public GameObject floorWithoutHole;
    public GameObject floorWithHole;
    public GameObject ladder;

    [Header("Joueur après le jeu")]
    public XROrigin xrOrigin;          // ton Perso XR origin
    public Transform spawnAfterGame;   // l’empty "SpawnAfterGame"

    private bool _winTriggered = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateLetter();
    }

    public void OnWordPlaced(WordData word, WordSlot slot)
    {

        var tmp = word.GetComponentInChildren<TextMeshPro>();
        string text = tmp != null ? tmp.text : word.name;

        _words[slot.slotId] = text;
        UpdateLetter();

        // 👉 prévenir la narration que ce slot est rempli
        NarrationManager.Instance?.OnSlotFilled(slot.slotId);
    }

    private void UpdateLetter()
    {
        string s = baseSentence;

        // Remplacements pour tous les types de slots
        if (_words.TryGetValue("SUJET", out var sujet))
            s = s.Replace("_____SUJET_____", sujet);

        if (_words.TryGetValue("ACTION", out var action))
            s = s.Replace("_____ACTION_____", action);

        if (_words.TryGetValue("LIEU", out var lieu))
            s = s.Replace("_____LIEU_____", lieu);

        if (_words.TryGetValue("MOTIF_VRAI", out var motifVrai))
            s = s.Replace("_____MOTIF_VRAI_____", motifVrai);

        if (_words.TryGetValue("FAKE_NEWS", out var fakeNews))
            s = s.Replace("_____FAKE_NEWS_____", fakeNews);

        if (_words.TryGetValue("OBJECTIF_REGIME", out var objectifRegime))
            s = s.Replace("_____OBJECTIF_REGIME_____", objectifRegime);

        if (letterText != null)
        {
            letterText.text = s;
            Debug.Log($"Lettre mise à jour : {s}");
        }
        else
        {
            Debug.LogError("LetterManager : letterText n'est PAS assigné dans l'inspector !");
        }


        if (!_winTriggered && IsSentenceComplete())
        {
            _winTriggered = true;
            Debug.Log("[LetterManager] Lettre complète -> ouverture de la sortie.");
            UnlockEscape();
        }
    }


    public bool IsSentenceComplete()
    {
        // à adapter si certains slots sont optionnels
        return _words.ContainsKey("SUJET")
            && _words.ContainsKey("ACTION")
            && _words.ContainsKey("LIEU")
            && _words.ContainsKey("MOTIF_VRAI")
            && _words.ContainsKey("FAKE_NEWS")
            && _words.ContainsKey("OBJECTIF_REGIME");
    }

    public bool IsSlotFilled(string slotId)
    {
        return _words.ContainsKey(slotId);
    }

    private void UnlockEscape()
    {
        if (floorWithoutHole != null)
            floorWithoutHole.SetActive(false);

        if (floorWithHole != null)
            floorWithHole.SetActive(true);

        if (ladder != null)
            ladder.SetActive(true);

        if (xrOrigin != null && spawnAfterGame != null)
        {
            xrOrigin.transform.position = spawnAfterGame.position;
            xrOrigin.transform.rotation = spawnAfterGame.rotation;
            Debug.Log("[LetterManager] Joueur téléporté sur SpawnAfterGame.");
        }

        if (xrOrigin != null)
        {
            var cc = xrOrigin.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = true;

            var behaviours = xrOrigin.GetComponents<MonoBehaviour>();
            foreach (var b in behaviours)
            {
                if (b == null) continue;
                string typeName = b.GetType().Name;

                if (typeName == "XRInputModalityManager" ||
                    typeName == "XRGazeAssistance")
                {
                    b.enabled = true;
                    Debug.Log("[LetterManager] Réactivation de " + typeName);
                }
            }
        }

        if (MistakeManager.Instance != null)
        {
            MistakeManager.Instance.ResetMistakes();
        }
    }


}

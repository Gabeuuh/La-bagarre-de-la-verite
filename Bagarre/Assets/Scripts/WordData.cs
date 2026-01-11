using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WordData : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    public string slotId = "SUJET";   // type de slot cible
    public bool isTrueWord = true;    // est-ce le bon mot pour ce slot ?

    WordSlot _slotCandidate;
    Rigidbody _rb;
    bool _isSnapped = false;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        throwOnDetach = false; // on ne lance jamais
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (_isSnapped) return;   // on ne peut plus le reprendre une fois posé
        base.OnSelectEntered(args);
    }

    private void SnapToSlot(WordSlot slot)
    {
        _isSnapped = true;
        _rb.isKinematic = true;

        if (slot.snapPoint == null)
        {
            Debug.LogError($"Slot {slot.slotId} n'a PAS de snapPoint assigné !");
            return;
        }

        transform.position = slot.snapPoint.position;
        transform.rotation = slot.snapPoint.rotation;
        transform.SetParent(slot.snapPoint);

        // On prévient le manager uniquement si c'est un vrai mot
        if (LetterManager.Instance != null && isTrueWord)
        {
            LetterManager.Instance.OnWordPlaced(this, slot);
        }

        slot.currentWord = null;

        interactionLayers = 0;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("[WordData] SnapToSlot -> destruction du mot valide.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var slot = other.GetComponent<WordSlot>();
        if (slot != null)
        {
            Debug.Log("Word ENTER slot " + slot.slotId);

            // On ne garde comme candidat que les slots qui correspondent à CE type de mot
            if (slot.slotId == slotId)
            {
                _slotCandidate = slot;
                Debug.Log(" -> Candidat retenu pour moi (" + slotId + ")");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var slot = other.GetComponent<WordSlot>();
        if (slot != null)
        {
            Debug.Log("Word EXIT slot " + slot.slotId);

            if (slot == _slotCandidate)
            {
                _slotCandidate = null;
                Debug.Log(" -> Candidat perdu (" + slot.slotId + ")");
            }
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (_isSnapped) return;

        base.OnSelectExited(args);

        string candidateId = _slotCandidate != null ? _slotCandidate.slotId : "NULL";
        Debug.Log($"OnSelectExited : my slotId = {slotId}, slotCandidate = {candidateId}, isTrueWord = {isTrueWord}");

        // On a lâché le mot AU-DESSUS de la feuille (slotId correspondant)
        if (_slotCandidate != null)
        {
            if (isTrueWord)
            {
                // ✅ Bon mot pour ce slot : on valide
                Debug.Log("[WordData] Bon mot pour ce slot -> SnapToSlot");
                SnapToSlot(_slotCandidate);
            }
            else
            {
                // ❌ Mauvais mot pour ce slot -> erreur (pour TOUS les slots maintenant)
                Debug.Log("[WordData] MAUVAIS mot pour ce slot -> erreur (slotId = " + slotId + ")");

                if (MistakeManager.Instance == null)
                {
                    Debug.LogWarning("[WordData] MistakeManager.Instance est NULL, aucune erreur enregistrée !");
                }
                else
                {
                    MistakeManager.Instance.RegisterMistake();
                }

                Debug.Log("[WordData] Destruction du mot après erreur.");
                Destroy(gameObject);
            }
        }
        else if (_rb != null)
        {
            // Lâché sans être au-dessus d'un slot -> il tombe, pas d'erreur
            Debug.Log("[WordData] Lâché sans slot -> gravité, aucune erreur.");
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }
    }
}

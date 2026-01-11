using UnityEngine;
using UnityEngine.Events;

public class HandQTEShapes : MonoBehaviour
{
    public enum QteShape
    {
        Circle,
        Square,
        Cross
    }

    [Header("Références")]
    public Transform hand;              // Main du joueur (RightHand Controller par ex.)
    public Transform circleParent;      // Parent des points du cercle
    public Transform squareParent;      // Parent des points du carré
    public Transform crossParent;       // Parent des points de la croix

    [Header("Paramètres")]
    public float radius = 0.15f;        // Rayon de validation autour d'un point
    public float timeLimit = 3f;        // Temps max pour un QTE

    [Header("Événements")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    Transform[] activeCheckpoints;
    Transform activeParent;
    int currentIndex;
    float timer;
    bool running;

    // Méthodes publiques pratiques
    public void StartCircleQTE() => StartQTE(QteShape.Circle);
    public void StartSquareQTE() => StartQTE(QteShape.Square);
    public void StartCrossQTE() => StartQTE(QteShape.Cross);

    public void StartQTE(QteShape shape)
    {
        // désactiver tous les parents au début
        if (circleParent) circleParent.gameObject.SetActive(false);
        if (squareParent) squareParent.gameObject.SetActive(false);
        if (crossParent) crossParent.gameObject.SetActive(false);

        switch (shape)
        {
            case QteShape.Circle:
                activeParent = circleParent;
                break;
            case QteShape.Square:
                activeParent = squareParent;
                break;
            case QteShape.Cross:
                activeParent = crossParent;
                break;
        }

        if (activeParent == null)
        {
            Debug.LogWarning($"Pas de parent défini pour le QTE {shape}");
            return;
        }

        // 🔥 On place le QTE autour de la main
        Vector3 startPos = hand.position + hand.forward * 0.4f; // 40 cm devant la main
        activeParent.position = startPos;

        int count = activeParent.childCount;
        if (count == 0)
        {
            Debug.LogWarning($"Aucun checkpoint enfant sous {activeParent.name}");
            return;
        }

        activeCheckpoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            activeCheckpoints[i] = activeParent.GetChild(i);
        }

        activeParent.gameObject.SetActive(true);

        currentIndex = 0;
        timer = 0f;
        running = true;
    }


    void Update()
    {
        if (!running || hand == null || activeCheckpoints == null || activeCheckpoints.Length == 0)
            return;

        timer += Time.deltaTime;
        if (timer > timeLimit)
        {
            Fail();
            return;
        }

        Vector3 handPos = hand.position;
        Vector3 targetPos = activeCheckpoints[currentIndex].position;

        Debug.Log(
            $"Hand = {handPos} | Target = {targetPos} | Dist = {Vector3.Distance(handPos, targetPos)}"
        );

        if (Vector3.Distance(handPos, targetPos) <= radius)
        {
            currentIndex++;

            if (currentIndex >= activeCheckpoints.Length)
            {
                Success();
            }
        }
    }



    void Success()
    {
        running = false;
        if (activeParent) activeParent.gameObject.SetActive(false);
        onSuccess?.Invoke();
        Debug.Log("QTE réussi !");
    }

    void Fail()
    {
        running = false;
        if (activeParent) activeParent.gameObject.SetActive(false);
        onFail?.Invoke();
        Debug.Log("QTE raté...");
    }

    void Start()
    {
        StartCircleQTE();
    }

}

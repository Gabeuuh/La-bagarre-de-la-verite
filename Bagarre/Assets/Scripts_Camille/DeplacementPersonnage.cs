using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DeplacementPersonnage : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float vitesseDeplacement = 2f;
    [SerializeField] private float distanceDeplacement = 5f;

    [Header("Animation (optionnel)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string nomParametreMarche = "IsWalking";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonBadGuy;


    [Header("Événements")]
    public UnityEvent onDeplacementTermine;
    public UnityEvent onAvancementFileTermine;

    private bool enDeplacement = false;
    private Vector3 positionCible;
    private Quaternion rotationCible;
    private Vector3 positionInitiale;
    private bool rotationCibleDefinie = false;
    private bool estAvancementFile = false; 
    public bool EnDeplacement => enDeplacement;

    // Propriétés publiques pour configuration dynamique
    public float VitesseDeplacement { get => vitesseDeplacement; set => vitesseDeplacement = value; }
    public float DistanceDeplacement { get => distanceDeplacement; set => distanceDeplacement = value; }
    public Animator AnimatorReference { get => animator; set => animator = value; }

    void Start()
    {
        positionInitiale = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enDeplacement)
        {
            // Déplacer vers la position cible
            transform.position = Vector3.MoveTowards(transform.position, positionCible, vitesseDeplacement * Time.deltaTime);

            // Si une rotation cible est définie, interpoler vers cette rotation
            if (rotationCibleDefinie)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationCible, 360f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, positionCible) < 0.1f)
            {
                enDeplacement = false;
                bool etaitAvancementFile = estAvancementFile;
                estAvancementFile = false;

                // S'assurer que la position et rotation finales sont exactes
                transform.position = positionCible;
                if (rotationCibleDefinie)
                    transform.rotation = rotationCible;

                rotationCibleDefinie = false;

                if (animator != null)
                    animator.SetBool(nomParametreMarche, false);

                // Déclencher l'événement approprié selon le type de déplacement
                if (etaitAvancementFile)
                {
                    onAvancementFileTermine?.Invoke();
                }
                else
                {
                    onDeplacementTermine?.Invoke();
                }
            }
        }
    }

    public void MarcherVersLaGauche()
    {
        // Rotation vers la gauche (180 degrés)
        transform.rotation = Quaternion.Euler(0, 180, 0);

        // Déplacement dans la direction où le personnage regarde (forward)
        positionCible = transform.position + transform.forward * distanceDeplacement;

        DemarrerDeplacement();
    }

    private void JouerSonFinBadGuy()
    {
        if (audioSource != null && sonBadGuy != null)
        {
            audioSource.PlayOneShot(sonBadGuy);
        }
    }

    private IEnumerator JouerSonAvecDelai(float delay)
    {
        yield return new WaitForSeconds(delay);
        JouerSonFinBadGuy();
    }

    public void MarcherVersLaDroite()
    {
        // Rotation vers la droite (0 degrés pour aller vers la droite en axe Z-)
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Déplacement dans la direction où le personnage regarde (forward)
        positionCible = transform.position + transform.forward * distanceDeplacement;
        DemarrerDeplacement();
        StartCoroutine(JouerSonAvecDelai(2f));
    }

    private void DemarrerDeplacement()
    {
        enDeplacement = true;

        if (animator != null)
            animator.SetBool(nomParametreMarche, true);
    }

    public void ReinitialiserPosition()
    {
        transform.position = positionInitiale;
        transform.rotation = Quaternion.identity;
        enDeplacement = false;
        rotationCibleDefinie = false;

        if (animator != null)
            animator.SetBool(nomParametreMarche, false);
    }

    /// <summary>
    /// Déplace le personnage vers une position et rotation spécifiques
    /// Utilisé par le GestionnairePersonnages pour l'avancement de la file
    /// </summary>
    public void DeplacerVersPosition(Vector3 nouvellePosition, Quaternion nouvelleRotation, bool avancementFile = true)
    {
        if (enDeplacement)
        {
            return;
        }

        positionCible = nouvellePosition;
        rotationCible = nouvelleRotation;
        rotationCibleDefinie = true;
        estAvancementFile = avancementFile; // Indiquer qu'il s'agit d'un avancement de file

        DemarrerDeplacement();
    }

    /// <summary>
    /// Arrête immédiatement le déplacement en cours
    /// </summary>
    public void ArreterDeplacement()
    {
        enDeplacement = false;
        rotationCibleDefinie = false;

        if (animator != null)
            animator.SetBool(nomParametreMarche, false);
    }
}

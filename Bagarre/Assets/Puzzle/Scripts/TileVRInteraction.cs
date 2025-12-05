using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TileVRInteraction : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    public Tile tile { get; set; }

    public delegate void DelegateOnTileInPlace(TileVRInteraction tm);
    public DelegateOnTileInPlace onTileInPlace;

    [SerializeField]
    private float snapDistance = 0.2f; // distance de snap en unités monde, ajuste si besoin

    private Vector3 GetCorrectPosition()
    {
        // même logique que le TileMovement d'origine : xIndex * 100, yIndex * 100
        return new Vector3(tile.xIndex * Tile.tileSize, tile.yIndex * Tile.tileSize, 0f);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (tile == null) return;

        float dist = Vector3.Distance(transform.position, GetCorrectPosition());
        if (dist < snapDistance)
        {
            // On “snap” la pièce à sa position correcte
            transform.position = GetCorrectPosition();
            transform.rotation = Quaternion.identity;

            // On verrouille la pièce (plus grabbable)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            enabled = false;

            // On prévient le BoardGen
            onTileInPlace?.Invoke(this);
        }
    }
}

using UnityEngine;

public class Door : MonoBehaviour
{
    public string requiDoorKeyID = "KeyDoor";
    public Transform doorPivot;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    bool isOpen = false;

    void OnTriggerEnter(Collider other)
    {
        Key key = other.GetComponent<Key>();

        if (key != null && key.keyID == requiDoorKeyID)
        {
            isOpen = true;
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        if (isOpen)
        {
            Quaternion target = Quaternion.Euler(0, openAngle, 0);
            doorPivot.localRotation =
                Quaternion.Slerp(
                    doorPivot.localRotation,
                    target,
                    Time.deltaTime * openSpeed);
        }
    }
}







/*using UnityEngine;

public class DoorScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}*/

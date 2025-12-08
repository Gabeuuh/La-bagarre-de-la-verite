using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character_jump : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public float jumpforce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if(Input.GetKeyDown(KeyCode.Space) && GroundCheck())
        {
            rb.AddForce(transform.up*jumpforce,ForceMode.Impulse);
        }        
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawCube(transform.position-transform.up*maxDistance,boxSize);
    }
    bool GroundCheck()
    {
        if(Physics.BoxCast(transform.position,boxSize,-transform.up,transform.rotation,maxDistance,layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
     
    
}

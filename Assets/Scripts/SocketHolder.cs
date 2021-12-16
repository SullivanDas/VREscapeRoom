using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketHolder : MonoBehaviour
{
    public FlashlightSelect grabber;
    private SphereCollider coll;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
 
        if(other.GetComponent<Grabbable>() && other.tag != "Clone" && (grabber.GrabbedObject && grabber.GrabbedObject.gameObject != other.gameObject || grabber.GrabbedObject == null))
        {

            other.transform.parent = transform;
            other.transform.localPosition = Vector3.zero;
        }
    }
}

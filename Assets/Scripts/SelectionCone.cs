using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SelectionCone : MonoBehaviour
{

    private MeshCollider coll;
    private List<GameObject> collisions;

    // Start is called before the first frame update
    void Start()
    {
        collisions = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grabbable")
        {         
            collisions.Add(other.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grabbable")
        {
            collisions.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetCollisions()
    {
        return collisions;
    }
}

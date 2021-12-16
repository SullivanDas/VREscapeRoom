using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRing : MonoBehaviour
{
    [SerializeField] private float radius = 0.15f;

    private List<GameObject> heldObjects = new List<GameObject>();
    private List<GameObject> copies = new List<GameObject>();
    Dictionary<GameObject, GameObject> copyOriginals = new Dictionary<GameObject, GameObject>();

    public void PopulateRing(List<GameObject> objectsToInstantiate)
    {
        ClearHeldObjects();

        //heldObjects = objectsToInstantiate;
        int totalObjects = objectsToInstantiate.Count;
        for(int i = 0; i < totalObjects; i++)
        {
            float angleOfSpawn = (2 * Mathf.PI / totalObjects) * i;
            float x = Mathf.Cos(angleOfSpawn);
            float y = Mathf.Sin(angleOfSpawn);

            Vector3 spawnPos = new Vector3(x, y, 0) * radius;

            Vector3 newBasisSpawnPos = new Vector3(transform.right.x * spawnPos.x, transform.right.y * spawnPos.x, transform.right.z * spawnPos.x) + new Vector3(transform.up.x * spawnPos.y, transform.up.y * spawnPos.y, transform.up.z * spawnPos.y);
            newBasisSpawnPos += transform.position;
            //copies.Add(Instantiate(objectsToInstantiate[i], spawnPos, transform.localRotation, transform));
            GameObject next = Instantiate(objectsToInstantiate[i], newBasisSpawnPos, Quaternion.identity, transform);
            next.tag = "Clone";
            next.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            //copies.Add(next);
            copyOriginals.Add(next, objectsToInstantiate[i]);
        }
    }

    public void PopulateMiniture(List<GameObject> objectsToInstantiate)
    {
        ClearHeldObjects();

        int totalObjects = objectsToInstantiate.Count;
        for (int i = 0; i < totalObjects; i++)
        {

            GameObject next = Instantiate(objectsToInstantiate[i], transform, false);
            next.tag = "Clone";

            next.transform.position *= 0.25f;
            next.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            next.transform.position += (transform.position - transform.position * 0.1f);
            //copies.Add(next);
            copyOriginals.Add(next, objectsToInstantiate[i]);
        }
    }
    public GameObject Select(GameObject go)
    {
        GameObject selection = copyOriginals[go];
        ClearHeldObjects();
        return selection;
    }

    private void ClearHeldObjects()
    {
        foreach (KeyValuePair<GameObject, GameObject> g in copyOriginals)
        {
            Destroy(g.Key);
        }
        copyOriginals.Clear();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class FlashlightSelect : MonoBehaviour
{
    [SerializeField] private float maxSelectSize;
    [SerializeField] private float minSelectSize;
    [SerializeField] private float maxSelectRange;
    [SerializeField] private AnimationCurve scaleAcceleration;

    public InputActionProperty selectAction;

    public GameObject selectionObject;
    private MeshRenderer selectionMesh;
    private Vector3 controllerStartPos;
    private bool isSelecting;

    // Start is called before the first frame update
    void Start()
    {
        selectionObject.transform.localScale = new Vector3(minSelectSize, minSelectSize, maxSelectRange);
        selectionMesh = selectionObject.GetComponentInChildren<MeshRenderer>();
        selectionMesh.enabled = false;

        selectAction.action.performed += StartSelecting;
        selectAction.action.canceled += EndSelecting;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelecting)
        {
            Vector3 dist = transform.position - controllerStartPos;
            float t = dist.magnitude;
            t = Vector3.Dot(transform.forward, dist) >= 0 ? t : 0;
            selectionObject.transform.localScale = new Vector3(Max(scaleAcceleration.Evaluate(t) * maxSelectSize, minSelectSize), Max(scaleAcceleration.Evaluate(t) * maxSelectSize, minSelectSize), maxSelectRange);
        }
    }

    float Max(float a, float b)
    {
        return a > b ? a : b;
    }
    void StartSelecting(InputAction.CallbackContext context)
    {
        selectionMesh.enabled = true;
        controllerStartPos = transform.position;
        isSelecting = true;
    }

    void EndSelecting(InputAction.CallbackContext context)
    {
        selectionMesh.enabled = false;
        isSelecting = false;
    }
}

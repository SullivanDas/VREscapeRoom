using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine;

public class FlashlightSelect : Grabber
{
    [SerializeField] private float maxSelectSize;
    [SerializeField] private float minSelectSize;
    [SerializeField] private float maxSelectRange;
    [SerializeField] private float maxBubbleCursorRange = 0.3f;

    [SerializeField] private AnimationCurve scaleAcceleration;

    public bool HasObjectsSelected { get; set; }

    public InputActionProperty selectAction;
    public InputActionProperty grabAction;
    public InputActionProperty toggleAction;

    public DisplayRing ring;
    public GameObject selectionObject;
    public CanvasGroup alphaGroup;
    public TextMeshProUGUI text;

    private List<GameObject> selectedObjects;
    private MeshRenderer selectionMesh;
    private SelectionCone selectionColl;
    private Vector3 controllerStartPos;
    private Grabbable currentObject;
    private Grabbable grabbedObject;
    private SphereCollider bubbleColl;

    private bool isKinematicSettings;
    private bool hasGravitySettings;
    private bool isSelecting;
    private bool isQueuedToReset;
    private bool isFlashlightEnabled;

    private int selectionState = 0;

    // Start is called before the first frame update
    void Start()
    {
        selectedObjects = new List<GameObject>();
        selectionObject.transform.localScale = new Vector3(minSelectSize, minSelectSize, maxSelectRange);
        selectionMesh = selectionObject.GetComponentInChildren<MeshRenderer>();
        selectionColl = selectionObject.GetComponentInChildren<SelectionCone>();
        selectionMesh.enabled = false;

        bubbleColl = GetComponent<SphereCollider>();

        selectAction.action.performed += StartSelecting;
        selectAction.action.canceled += EndSelecting;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        toggleAction.action.performed += ToggleFlashlight;

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
        if (!HasObjectsSelected && isFlashlightEnabled)
        {
            selectionMesh.enabled = true;
            controllerStartPos = transform.position;
            isSelecting = true;
        }

    }

    void EndSelecting(InputAction.CallbackContext context)
    {
        if (!HasObjectsSelected && isFlashlightEnabled)
        {
            selectionMesh.enabled = false;
            isSelecting = false;
            selectedObjects = selectionColl.GetCollisions();

            if(selectedObjects.Count > 0)
            {
                if(selectionState == 1)
                {
                    ring.PopulateRing(selectedObjects);
                }
                else if(selectionState == 2)
                {
                    ring.PopulateMiniture(selectedObjects);
                }
 
                HasObjectsSelected = true;
            }

        }
        else if (isQueuedToReset)
        {
            isQueuedToReset = false;
            HasObjectsSelected = false;
        }

    }

    public override void Grab(InputAction.CallbackContext context)
    {
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }

            if(currentObject.tag == "Clone")
            {
                isQueuedToReset = true;
                grabbedObject = currentObject.GetComponentInParent<DisplayRing>().Select(currentObject.gameObject).GetComponent<Grabbable>();
                grabbedObject.SetCurrentGrabber(this);
            }
            else
            {
                grabbedObject = currentObject;
                grabbedObject.SetCurrentGrabber(this);

            }

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                isKinematicSettings = grabbedObject.GetComponent<Rigidbody>().isKinematic;
                hasGravitySettings = grabbedObject.GetComponent<Rigidbody>().useGravity;
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            grabbedObject.transform.parent = this.transform;
            if (currentObject.tag == "Clone")
            {
                currentObject.tag = "Grabbable";
                grabbedObject.transform.localPosition = Vector3.zero;
            }


        }
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = isKinematicSettings;
                grabbedObject.GetComponent<Rigidbody>().useGravity = hasGravitySettings;
            }

            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = null;
            grabbedObject = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentObject == null && other.GetComponent<Grabbable>())
        {
            currentObject = other.gameObject.GetComponent<Grabbable>();

        }
        else if (other.GetComponent<Grabbable>())
        {
            float newDist = (other.transform.position - transform.position).magnitude;
            if (newDist < (currentObject.transform.position - transform.position).magnitude)
            {
                currentObject.DeHighlight();
                currentObject = other.gameObject.GetComponent<Grabbable>();
                currentObject.Highlight();
                bubbleColl.radius = newDist;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObject)
        {
            if (other.GetComponent<Grabbable>() && currentObject.GetInstanceID() == other.GetComponent<Grabbable>().GetInstanceID())
            {
                currentObject = null;
                bubbleColl.radius = maxBubbleCursorRange;
            }
        }
    }

    void ToggleFlashlight(InputAction.CallbackContext context)
    {
        selectionState = (selectionState + 1) % 3;
        switch (selectionState)
        {
            case 0:
                isFlashlightEnabled = false;
                alphaGroup.alpha = 0;

                break;

            case 1:
                text.text = "Flashlight Ring";
                isFlashlightEnabled = true;
                alphaGroup.alpha = 1;
                break;
            case 2:
                text.text = "Flashlight MiniWorld";
                isFlashlightEnabled = true;
                alphaGroup.alpha = 1;
                break;
        }

    }
}

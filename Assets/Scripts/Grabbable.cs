using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    private Grabber currentGrabber;

    private bool isHighlighted; 

    // Start is called before the first frame update
    void Start()
    {
        currentGrabber = null;

        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().Sleep();
        }
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }

    public void Highlight()
    {
        if(tag == "Clone" && !isHighlighted)
        {
            transform.localScale *= 2;
            isHighlighted = true;
        }
    }

    public void DeHighlight()
    {
        if(tag == "Clone" && isHighlighted)
        {
            
            transform.localScale /= 2;
            isHighlighted = false;
        }

    }
}

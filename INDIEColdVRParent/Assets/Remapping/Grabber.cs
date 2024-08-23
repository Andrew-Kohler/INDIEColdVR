using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public Collider grabberCollider;
    [Tooltip("Whether something is being held")] public bool locked = false;
    public bool shakesWithParent = false;

    public GameObject currentParent;
    [Tooltip("The grabber that the object will parent to in order to shake")] public GameObject heldParent;
    public GameObject held;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !locked)
        {
            Debug.Log("grabbing");
            Grabbable[] grabs = FindObjectsOfType<Grabbable>();
            Debug.Log(grabs.Length);
            foreach (Grabbable g in grabs)
            {
                if (g.gameObject.GetComponent<Collider>())
                {

                    if (g.gameObject.GetComponent<Collider>().bounds.Intersects(grabberCollider.bounds))
                    {
                        Debug.Log("intersection");
                        held = g.gameObject;
                        currentParent = (held.transform.parent == null)?null:held.transform.parent.gameObject;
                        if (GameManager.Instance.IsHeldObjectShaking)
                        {
                            held.transform.SetParent(heldParent.transform);
                        }
                        else
                        {
                            held.transform.SetParent(gameObject.transform);
                        }
                        //held.transform.SetParent(heldParent.transform); //gameObject.transform

                        locked = true;
                        return;
                    }
                    else
                    {
                        Debug.Log("no intersection");
                    }

                }


            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) & locked)
        {           
            held.transform.SetParent((currentParent==null)?null:currentParent.transform);
            locked = false;
            currentParent = null;
            held = null;
        }
    }
}

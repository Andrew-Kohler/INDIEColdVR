using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggleAxis()
    {
        GameManager.Instance.IsAxisJitter = !GameManager.Instance.IsAxisJitter;
        
    }

    public void ToggleShake()
    {
        GameManager.Instance.IsShakeJitter = !GameManager.Instance.IsShakeJitter;
    }

    public void ToggleProx()
    {
        GameManager.Instance.IsProxJitter = !GameManager.Instance.IsProxJitter;
    }

    public void ToggleHeldShake()
    {
        GameManager.Instance.IsHeldObjectShaking = !GameManager.Instance.IsHeldObjectShaking;
    }
}

    

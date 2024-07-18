using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public bool IsAxisJitter;
    public bool IsShakeJitter;
    public bool IsProxJitter;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject managerHolder = new GameObject("[Game Manager]");
                managerHolder.AddComponent<GameManager>();
                DontDestroyOnLoad(managerHolder);
                //_instance = managerHolder.GetComponent<GameManager>();          
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

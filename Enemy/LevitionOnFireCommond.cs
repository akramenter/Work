using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitionOnFireCommond : MonoBehaviour
{
    public LevitationController[] controllers;
    //public LevitationCannon[] cannons;
    // Start is called before the first frame update
    void Awake()
    {
        controllers = GetComponentsInChildren<LevitationController>();

    }

    // Update is called once per frame
    void Update()
    {
        //controllers[0].cannon.cannonType = controllers[2].cannon.cannonType = 0;
        //controllers[1].cannon.cannonType = controllers[3].cannon.cannonType = 1;
        if (controllers == null) return;
        if (controllers[1].cannon.cannonType != 1)
        {
            controllers[1].cannon.cannonType =  1;
            return;
        }

    }
}

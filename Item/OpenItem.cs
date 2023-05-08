using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenItem : MonoBehaviour
{
    public ActorManager am;
    public ActorManager player;
    int count = 0;
    void Start()
    {
        am = GetComponent<ActorManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ActorManager>();
    }


    void Update()
    {
        if (player.sm.doorOpen)
        {
            if (!am.ac.CheckState("opened"))
            {
                count = am.gm.count;
                am.ac.SetBool("lock", true);
                if (am.ac.CheckState("lock"))
                    am.ac.SetBool("lock", false);
            }
        }
    }
}

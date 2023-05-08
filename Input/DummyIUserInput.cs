using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyIUserInput : IUserInput
{
    public ActorManager am;
    IEnumerator Start()
    {
        //am = GetComponent<ActorManager>();
        while (true)
        {
            // if (am.sm.HP == am.sm.HPMax)
            //  rb = false;
            rb = true;
            yield return new WaitForSeconds(1.0f);
            //am.ac.anim.SetTrigger("trust");
            //yield return new WaitForSeconds(3.0f);
            //rb = false;
            //yield return new WaitForSeconds(3.0f);
        }


    }

    void Update()
    {


    }


}


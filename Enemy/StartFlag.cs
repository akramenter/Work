using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFlag : MonoBehaviour
{
    public bool startFlag;
    public ActorManager playerAm;
    public Collider coll;
    public Renderer rend;
    public bool IsInThePlace()
    {
        Collider col = this.GetComponent<Collider>();
        if (col.isTrigger)
        {
            // gameObject.SetActive(false);

        }
        else
        {
            // gameObject.SetActive(true);
        }
        return false;
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    //Debug.Log(gameObject.name + " - OnTriggerEnter - " + other.gameObject.name);
    //    StateManager sm = other.GetComponent<StateManager>();
    //    if (other.gameObject.name == "PlayerHandle")
    //    {
    //        sm.startFlag = true;

    //    }
    //}
    private void OnTriggerExit(Collider other)
    {
        StateManager sm = other.GetComponent<StateManager>();
        if (other.gameObject.name == "PlayerHandle")
        {
            // gameObject.SetActive(false);
            rend.enabled = false;
            coll.isTrigger = false;
            sm.startFlag = true;
        }

    }



    //// Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        playerAm = GameObject.FindGameObjectWithTag("Player").GetComponent<ActorManager>();
    }

    //// Update is called once per frame
    void Update()
    {
        // IsInThePlace();
        if (playerAm != null)
        {
            if (!playerAm.sm.startFlag)
            {
                rend.enabled = true;
                coll.isTrigger = true;
            }
        }

    }
}

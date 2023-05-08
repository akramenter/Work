using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    //public doorMtr mtr;
    public MeshCollider col;
    public ActorManager am;
    public MeshRenderer render;
    public Transform trans;
    public int curTime;
    bool flag = false;
    public float t = 10f;
    void Start()
    {
        //mtr = GetComponent<doorMtr>();
        col = GetComponent<MeshCollider>();
        render = GetComponent<MeshRenderer>();
        //mtr.enabled = false;
        am = GameObject.FindGameObjectWithTag("Player").GetComponent<ActorManager>();
        trans = GetComponentInParent<Transform>();
        // render.enabled = true ;
    }


    void Update()
    {
        Open();
        if (curTime > 0 && am.gm.count >= curTime + 3)
        {
            //mtr.enabled = true;
            flag = false;
            //col.enabled = false;
            //render.enabled = false;
            //trans.position = new Vector3(7.6f, -0.1f, -24.8f);
            // trans.rotation = new Quaternion(0, -0.9f, 0, 0.3f);
            trans.position = Vector3.Slerp(trans.position, new Vector3(7.6f, -0.1f, -24.8f), t);
            trans.rotation = Quaternion.Slerp(trans.rotation, new Quaternion(0, -0.9f, 0, 0.3f), t);
        }


    }
    public void Open()
    {
        if (am.sm.doorOpen && !flag)
        {
            if (am.gm.count > 0) //第一回
                curTime = am.gm.count;
            else//切换场景回来之后，若是死亡重来则不关门/通关则重置
            {
                // col.enabled = false;
                // render.enabled = false;
                trans.position = new Vector3(7.6f, -0.1f, -24.8f);
                trans.rotation = new Quaternion(0, -0.9f, 0, 0.3f);
                //{ "position":{ "x":7.628973960876465,"y":-0.15999984741210938,"z":-24.885244369506837},"rotation":{ "x":0.0,"y":-0.9215953350067139,"z":0.0,"w":0.38815221190452578},"scale":{ "x":7.0,"y":7.0,"z":7.0} }
            }
            flag = true;
        }
    }
}

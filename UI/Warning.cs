using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    public ActorManager am;
    public Image image;
    public Text text;
    int current;
    bool flag = false;
    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            am = obj.GetComponent<ActorManager>();
        }
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        image.enabled = false;
        text.enabled = false;
    }


    void Update()
    {
        if (am.sm.warning && !flag)
        {
            image.enabled = true;
            text.enabled = true;
            current = am.gm.count;
            flag = true;
        }

        if (current > 0 && am.gm.count >= current + 3)
        {
            image.enabled = false;
            text.enabled = false;
            am.sm.warning = false;
            flag = false;
            current = 0;
        }

    }
}

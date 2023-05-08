using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossHp : MonoBehaviour
{
    public Image HPbk;
    public Image HpCurrent;
    // public GameObject target;
    public float height = 2.5f;
    public float HpPercent;
    public StateManager sm;
    public BossAI boss;
    // Start is called before the first frame update
    void Start()
    {
        //target=GetComponentInParent<GameObject>();
        sm = GetComponentInParent<StateManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (sm.HP <= 0)
        {
            HPbk.enabled = false;
            HpCurrent.enabled = false;
            return;
        }
        //else
        //{
        //    HPbk.enabled = true;
        //    HpCurrent.enabled = true;
        //}

        Amount();


    }
    public void Amount()
    {

        HpPercent = sm.HP / sm.HPMax;
        HpCurrent.fillAmount = HpPercent;


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hp : MonoBehaviour
{
    public Image HPbk;
    public Image HpCurrent;
    // public GameObject target;
    public float height = 2.5f;
    public float HpPercent;
    public StateManager sm;
    // public EnemyStateInput pi;

    // Start is called before the first frame update
    void Start()
    {
        //target=GetComponentInParent<GameObject>();
        sm = GetComponentInParent<StateManager>();
        // pi = GetComponentInParent<EnemyStateInput>();

    }

    // Update is called once per frame
    void Update()
    {
        if (sm.HP <= 0)
        {
            Destroy(HPbk);
            Destroy(HpCurrent);
            return;
        }
        Vector3 tgtP = sm.transform.position;
        tgtP.y += height;
        Vector2 tarPos = Camera.main.WorldToScreenPoint(tgtP);
        HPbk.GetComponent<Image>().transform.position = tarPos;
        HpCurrent.GetComponent<Image>().transform.position = tarPos;
        Amount();


    }
    public void Amount()
    {
        HpPercent = sm.HP / sm.HPMax;
        HpCurrent.fillAmount = HpPercent;
    }

}

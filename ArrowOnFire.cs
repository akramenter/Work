using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowOnFire : MonoBehaviour
{
    public Transform arrowPrefab;
    public BossAI boss;
    public Transform arrowStartPos;
    public Battery battery;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossAI>();
        battery = GetComponentInChildren<Battery>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (boss.onFire)
        //{
        //    Instantiate(arrowPrefab, arrowStartPos.transform.position, arrowStartPos.transform.rotation);
        //    boss.onFire = false;
        //}
    }
    public void OnFire()
    {
        // boss.onFire = true;
        //if (boss.am.sm.HP >= boss.am.sm.HPMax * 0.5f || boss.am.sm.HP <= boss.am.sm.HPMax * 0.1f)
        Instantiate(arrowPrefab, arrowStartPos.transform.position, arrowStartPos.transform.rotation);
        // boss.onFire = false;
    }
}

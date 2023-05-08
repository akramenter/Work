using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBattle : MonoBehaviour
{
    public BoxCollider defCol; //defense collider 
    public GameObject cannonHandle;
    public Animator anim;
    public float Hp = 20;
    void Start()
    {
        defCol = GetComponent<BoxCollider>();
        //defCol.center = Vector3.up * 1.0f;
        defCol.isTrigger = true;
    }
    void Update()
    {

    }
    void OnTriggerEnter(Collider col)
    {
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        if (targetWc == null) { return; }
        GameObject target = targetWc.gameObject.GetComponentInParent<ActorManager>().gameObject;
        //GameObject attacker = targetWc.wm.am.ac.model.gameObject;
        //GameObject receiver = model.gameObject;

        if (col.tag == "Weapon" && target.tag == "Player")
        {
            Hp -= targetWc.GetATK();
            //print(Hp);
            if (Hp <= 0)
            {
                if (targetWc.wm.am.ac.camcon.lockTarget != null)
                    targetWc.wm.am.ac.camcon.LockProcessA(null, false, false, true);
                if (anim != null)
                    anim.SetTrigger("die");
                Destroy(cannonHandle);
            }
        }
        else
        {
            return;
        }
    }
    public static bool CheckAnglePlayer(GameObject player, GameObject target, float playerAngleLimit)
    {
        Vector3 counterDir = target.transform.position - player.transform.position;
        float counterAngle1 = Vector3.Angle(player.transform.forward, counterDir);
        float counterAngle2 = Vector3.Angle(target.transform.forward, player.transform.forward); //180

        bool counterValid = (counterAngle1 < playerAngleLimit && Mathf.Abs(counterAngle2 - 180) < playerAngleLimit);
        return counterValid;
    }
    public static bool CheckAngleTarget(GameObject player, GameObject target, float targetAngleLimit)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position; //敌人攻击的角度
        float attackingAngle1 = Vector3.Angle(target.transform.forward, attackingDir);
        bool attackValid = (attackingAngle1 < targetAngleLimit);
        return attackValid;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class BattleManager : IActorManagerInterface
{
    public CapsuleCollider defCol; //defense collider 
    public bool canDefance = false;
    void Start()
    {
        defCol = GetComponent<CapsuleCollider>();
        defCol.center = Vector3.up * 1.0f;
        defCol.height = 2.0f;
        defCol.radius = 0.25f;
        defCol.isTrigger = true;
    }
    void OnTriggerEnter(Collider col)
    {
        WeaponController targetWc = col.GetComponentInParent<WeaponController>();
        if (targetWc == null) { return; }
        GameObject attacker = targetWc.wm.am.ac.model.gameObject;
        GameObject receiver = am.ac.model.gameObject;

        if (col.tag == "Weapon")
        {
            if (CheckLookAt(receiver, attacker)) canDefance = true;
            else canDefance = false;
            am.TryDoDamage(targetWc, CheckAngleTarget(receiver, attacker, 70), CheckAnglePlayer(receiver, attacker, 45));
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
    public static bool CheckLookAt(GameObject player, GameObject target)
    {
        Vector3 attackingDir = player.transform.position - target.transform.position;
        float dot = Vector3.Dot(player.transform.forward, attackingDir.normalized);
        if (dot >= 0) return false;
        return true;
    }
}

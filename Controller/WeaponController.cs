using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponManager wm;
    public WeaponData wdata;
    void Awake()
    {
        wdata = GetComponentInChildren<WeaponData>();
    }
    public float GetATK()
    {
        return wdata.ATK + wm.am.sm.ATK;
    }
    public float GetDEF()
    {
        return wdata.DEF + wm.am.sm.DEF;
    }
    public string GetName()
    {
        return wdata.Name;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChange : MonoBehaviour
{
    public WeaponManager wm;
    public int inHandNo;
    public string[] names =
    {
        "Sword",
        "Falchion",
        "Mace",
        "Hand"
    };
    void Start()
    {
        wm = GetComponentInChildren<WeaponManager>();
        inHandNo = 3;
        wm.am.ac.leftIsShield = false;
        //wd = GetComponentsInChildren<WeaponData>();
    }
    //void Update()
    //{
    //    // NoLimit();


    //}
    public void ChangeWeapon(WeaponFactory weaponFact)
    {
        NoLimit();
        switch (inHandNo)
        {
            case 0:
                wm.ChangeDualHands(false);
                ChangeDual(weaponFact);
                wm.UpdateWeaponCollider(false, weaponFact.CreatWeapon("Shild", false, wm));
                wm.am.ac.leftIsShield = true;
                break;
            case 1:
                wm.ChangeDualHands(true, "sword");
                ChangeDual(weaponFact);
                wm.UpdateWeaponCollider(false, weaponFact.CreatWeapon("Hand", false, wm));

                break;
            case 2:
                wm.ChangeDualHands(false);
                ChangeDual(weaponFact);
                wm.UpdateWeaponCollider(false, weaponFact.CreatWeapon("Shild", false, wm));
                wm.am.ac.leftIsShield = true;
                break;
            case 3:
                wm.ChangeDualHands(true, "hand");
                ChangeDual(weaponFact);
                wm.UpdateWeaponCollider(false, weaponFact.CreatWeapon("Hand", false, wm));
                wm.am.ac.leftIsShield = false;
                break;

        }

    }
    public void NoLimit()
    {
        if (inHandNo < 0)
        {
            inHandNo = names.Length - 1;
        }
        if (inHandNo > names.Length - 1)
        {
            inHandNo = 0;
        }

    }
    //public void WeaponChange(bool isRight, WeaponFactory wf, WeaponManager wm, string weaponName, bool dualHand)
    //{
    //    wm.UpdateWeaponCollider(isRight, wf.CreatWeapon(weaponName, isRight, wm));
    //}
    public void ChangeDual(WeaponFactory weaponFact)
    {
        wm.UnloadWeapon(true); //right
        wm.UnloadWeapon(false);//left
        wm.UpdateWeaponCollider(true, weaponFact.CreatWeapon(names[inHandNo], true, wm));
    }
}

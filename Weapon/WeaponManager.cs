using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : IActorManagerInterface
{
    [SerializeField]
    public Collider weaponColL;
    [SerializeField]
    public Collider weaponColR;

    public GameObject whL;//ç∂éË
    public GameObject whR;//âEéË

    public WeaponController wcL;
    public WeaponController wcR;

    public GameObject swordSlash;
    public GameObject lanceStrightEffect;
    public GameObject lanceSlashEffect;
    public GameObject powerEffect;
    AnimatorStateInfo info;

    void Awake()
    {
        //left hand
        try
        {
            whL = transform.DeepFind("weaponHandleL").gameObject;
            wcL = BindWeaponController(whL);
            weaponColL = whL.GetComponentInChildren<Collider>();
            weaponColL.enabled = false;
        }
        catch
        {
            //
            // if there is no weaponHandleL or related objects
            //
        }
        //right hand
        try
        {
            whR = transform.DeepFind("weaponHandleR").gameObject;
            wcR = BindWeaponController(whR);
            weaponColR = whR.GetComponentInChildren<Collider>();
            weaponColR.enabled = false;
        }
        catch
        {
            //
            // if there is no weaponHandleR or related objects
            //
        }


    }
    public void UpdateWeaponCollider(bool isRight, Collider col)
    {
        if (!isRight)
        {
            weaponColL = col;
            //foreach(WeaponData weapon in weaponList)
            // {
            //     if(weapon.Name!= weaponList[])
            // }
        }
        else if (isRight)
        {
            weaponColR = col;
        }

    }
    public void UnloadWeapon(bool isRight)
    {
        if (!isRight)
        {
            foreach (Transform trans in whL.transform)
            {
                weaponColL = null;
                wcL.wdata = null;
                Destroy(trans.gameObject);
            }
        }
        else if (isRight)
        {
            foreach (Transform trans in whR.transform)
            {
                weaponColR = null;
                wcR.wdata = null;
                Destroy(trans.gameObject);
            }
        }
    }
    public WeaponController BindWeaponController(GameObject targetObj)
    {
        WeaponController tempWc;
        tempWc = targetObj.GetComponent<WeaponController>();
        if (tempWc == null)
        {
            tempWc = targetObj.AddComponent<WeaponController>();
        }
        tempWc.wm = this;
        return tempWc;
    }
    public void WeaponEnable()
    {

        //if (weaponColL == null || weaponColR == null) return;
        if (am.ac.CheckStateTag("attackL"))
        {
            weaponColL.enabled = true;
        }
        else
        {
            if (am.weaponChange != null)
            {
                if (am.weaponChange.inHandNo == 3)
                    weaponColL.enabled = true;
                weaponColR.enabled = true;
            }
            weaponColR.enabled = true;
        }


    }
    public void WeaponDisable()
    {
        // if (weaponColL == null || weaponColR == null) return;
        weaponColL.enabled = false;
        weaponColR.enabled = false;
        if (swordSlash != null)
        {
            // Instantiate(swordSlash, whR.transform.position, whR.transform.rotation);
            swordSlash.SetActive(false);
        }
        if (lanceStrightEffect != null)
        {
            lanceStrightEffect.SetActive(false);
        }
        if (lanceSlashEffect != null)
        {
            lanceSlashEffect.SetActive(false);
        }
    }
    public void SwordSlashEffectEnable()
    {
        if (swordSlash != null)
        {
            // Instantiate(swordSlash, whR.transform.position, whR.transform.rotation);
            swordSlash.SetActive(true);
        }
    }
    public void LanceStrightEffectEnable()
    {
        lanceStrightEffect.SetActive(true);

    }
    public void PowerEffectEnable()
    {
        powerEffect.SetActive(true);

    }
    public void PowerEffectDisable()
    {
        powerEffect.SetActive(false);

    }
    public void LanceSlashEffectEnable()
    {
        lanceSlashEffect.SetActive(true);

    }
    public void CounterBackEnable()
    {
        am.SetIsCounterBack(true);
    }
    public void CounterBackDisable()
    {
        am.SetIsCounterBack(false);
    }
    public void ChangeDualHands(bool dualOn, string name = null)
    {
        am.ChangeDualHands(dualOn, name);
    }

    void Update()
    {
        info = am.ac.anim.GetCurrentAnimatorStateInfo(0);

    }
}

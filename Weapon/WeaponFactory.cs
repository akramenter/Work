using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory
{
    private DataBase weaponDB;

    public WeaponFactory(DataBase _weaponDB)
    {
        weaponDB = _weaponDB;
    }
    public GameObject CreatWeapon(string weaponName, Vector3 pos, Quaternion rot)
    {
        GameObject prefab = Resources.Load(weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab, pos, rot);

        WeaponData wdata = obj.AddComponent<WeaponData>();
        wdata.ATK = weaponDB.weaponDatabase[weaponName]["ATK"].floatValue;
        return obj;
    }
    public Collider CreatWeapon(string weaponName, bool isRight, WeaponManager wm)
    {
        WeaponController wc;
        if (!isRight)
        {
            wc = wm.wcL;
        }
        else if (isRight)
        {
            wc = wm.wcR;
        }
        else
        {
            return null;
        }
        GameObject prefab = Resources.Load("Weapon/" + weaponName) as GameObject;
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = wc.transform;
        //if (!isRight)
        //{
        //    obj.transform.parent = wc.transform;
        //    obj.transform.parent.position = new Vector3(weaponDB.weaponDatabase[weaponName]["L"]["position"]["x"].floatValue, weaponDB.weaponDatabase[weaponName]["L"]["position"]["y"].floatValue, weaponDB.weaponDatabase[weaponName]["L"]["position"]["z"].floatValue);
        //}
        //else
        //{
        //    obj.transform.parent = wc.transform;
        //}
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        WeaponData _wdata = obj.AddComponent<WeaponData>();
        _wdata.ATK = weaponDB.weaponDatabase[weaponName]["ATK"].floatValue;
        _wdata.Name = weaponDB.weaponDatabase[weaponName]["Name"].stringValue;
        wc.wdata = _wdata;
        return obj.GetComponent<Collider>();
    }

}

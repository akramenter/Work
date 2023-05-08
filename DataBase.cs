using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
public class DataBase
{
    private string weaponDatabaseFileName = "weaponData";
    public readonly JSONObject weaponDatabase;
    public DataBase() //ctor+两次tab
    {
        TextAsset weaponContent = Resources.Load(weaponDatabaseFileName) as TextAsset;
        weaponDatabase = new JSONObject(weaponContent.text);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceCheck : MonoBehaviour
{
    public static InstanceCheck instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);


    }
    private void CheckGameObject()
    {
        if (tag == "Player")
        {
            return;
        }
        Destroy(this);
    }
    private void CheckSingle()//gmê•óBàÍìI
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
    private void Update()
    {

    }

}

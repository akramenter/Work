using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyArea : MonoBehaviour
{
    //public Collider area;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            // print("out:" + other.gameObject.name);
            Destroy(other.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //area = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

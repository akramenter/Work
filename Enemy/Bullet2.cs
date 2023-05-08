using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float originSpeed = 10f;

    //public WeaponData bulletData;
    // public ActorManager am;
    [SerializeField]
    private float speed = 0;
    // [SerializeField]
    // private Vector3 targetPos;
    public GameObject hit;

    private void OnTriggerEnter(Collider other)
    {
        GameObject clone = null;
        if (other.gameObject.tag != "Player")
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            ActorManager am = other.gameObject.GetComponent<ActorManager>();

            if (!am.ac.pi.jump && !am.sm.isJump && !am.sm.isJab && !am.sm.isRoll)
            {
                am.sm.AddHP(-1 * 2.0f);
                if (am.sm.HP <= 0)
                {
                    am.ac.IssueTrigger("die");
                }
                else
                {
                    if (!am.sm.isDown)
                    {
                        am.ac.IssueTrigger("hit");
                        //if (hit != null)
                        clone = Instantiate(hit, other.transform);
                    }
                }
            }
            else
            {
                return;
            }
            //hit.SetActive(false);
            //Destroy(hit);
            if (clone != null)
                Destroy(clone, 3);
            Destroy(gameObject);
        }

    }
    void OnBecameInvisible()  //看不见时消除
    {
        if (enabled)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        //bulletData = GetComponent<WeaponData>();
        //am = GameObject.FindGameObjectWithTag("Player").GetComponent<ActorManager>();

        // targetPos = am.gameObject.transform.position;

    }

    void Update()
    {
        speed = originSpeed * Time.deltaTime;
        //Vector3 pos = (targetPos - transform.position).normalized;
        transform.Translate(new Vector3(0, 0, speed));
        //if(transform.position.)
        // transform.position += new Vector3(pos.x, pos.y, pos.z * speed);
    }

}

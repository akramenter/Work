using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //抛物线
    //public float speed = 1f; //前方向（蓝轴）匀速
    public const float g = 9.8f;
    public GameObject target;
    public float speed = 1;
    public GameObject effect;
    public GameObject hit;

    private float verticalSpeed;
    private Vector3 moveDirection;
    private float angleSpeed;
    private float angle;

    //public WeaponData bulletData;
    private void OnTriggerEnter(Collider other)
    {
        GameObject clone = null;
        if (other.gameObject.tag != "Player")
        {
            //if (other.gameObject.tag == "Enemy")
            //{
            //    return;
            //}
            //else
            //{
            //    Destroy(gameObject);
            //    return;
            //}
            return;
        }
        else
        {
            effect.SetActive(false);
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
                    if (!am.sm.isHit)
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
            // Destroy(hit);
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
        //float tmepDistance = Vector3.Distance(transform.position, target.transform.position);
        //float tempTime = tmepDistance / speed;
        //float riseTime, downTime;
        //riseTime = downTime = tempTime / 2;
        verticalSpeed = g * 0.5f;

        //transform.LookAt(target.transform.position);
        float tempTan = verticalSpeed / speed;
        double hu = Mathf.Atan(tempTan);
        angle = (float)(180 / Mathf.PI * hu);
        transform.eulerAngles = new Vector3(-angle, transform.eulerAngles.y, transform.eulerAngles.z);
        angleSpeed = angle / 0.5f;

        //moveDirection = target.transform.position - transform.position;
        moveDirection = transform.forward;
    }
    private float time;
    void Update()
    {
        //transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        //if (transform.position.y < target.transform.position.y)
        //{
        //    //finish  
        //    return;
        //}


        time += Time.deltaTime;
        float test = verticalSpeed - g * time;
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * test * Time.deltaTime, Space.World);
        float testAngle = -angle + angleSpeed * time;
        transform.eulerAngles = new Vector3(testAngle, transform.eulerAngles.y, transform.eulerAngles.z);

    }


}



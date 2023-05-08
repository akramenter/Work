using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackArrow : MonoBehaviour
{
    public GameObject hitEffect;
    public Transform target; //瞄准的目标
    Vector3 speed = new Vector3(0, 0, 5); //炮弹本地坐标速度
    Vector3 lastSpeed; //存储转向前炮弹的本地坐标速度
    int rotateSpeed = 90; //旋转的速度，单位 度/秒
    Vector3 finalForward; //目标到自身连线的向量，最终朝向
    float angleOffset;  //自己的forward朝向和mFinalForward之间的夹角
    RaycastHit hit;
    public WeaponData arrowData;
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

            if (!am.ac.pi.jump && !am.sm.isJump && !am.sm.isJab && !am.sm.isRoll && !am.sm.isDefense)
            {
                am.sm.AddHP(-1 * arrowData.ATK);
                if (am.sm.HP <= 0)
                {
                    am.ac.IssueTrigger("die");
                }
                else
                {
                    if (!am.sm.isDown)
                    {
                        am.ac.IssueTrigger("down");
                        if (hitEffect != null)
                            clone = Instantiate(hitEffect, other.transform);
                    }
                }
            }
            else if (am.sm.isDefense)
            {
                am.ac.IssueTrigger("blocked");
                if (hitEffect != null)
                    clone = Instantiate(hitEffect, other.transform);
            }
            else
            {
                return;
            }
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
        //将炮弹的本地坐标速度转换为世界坐标
        speed = transform.TransformDirection(speed);

        arrowData = GetComponent<WeaponData>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        CheckHint();
        UpdateRotation();
        UpdatePosition();
    }

    //射线检测，如果击中目标点则销毁炮弹
    private void CheckHint()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.transform == target && hit.distance < 1)
            {
                Destroy(gameObject);
            }
        }
    }
    //更新位置
    private void UpdatePosition()
    {
        transform.position = transform.position + speed * Time.deltaTime;
    }

    //旋转，使其朝向目标点，要改变速度的方向
    private void UpdateRotation()
    {
        //先将速度转为本地坐标，旋转之后再变为世界坐标
        lastSpeed = transform.InverseTransformDirection(speed);

        ChangeForward(rotateSpeed * Time.deltaTime);

        speed = transform.TransformDirection(lastSpeed);
    }

    private void ChangeForward(float speed)
    {
        //获得目标点到自身的朝向
        finalForward = (target.position - transform.position).normalized;
        if (finalForward != transform.forward)
        {
            angleOffset = Vector3.Angle(transform.forward, finalForward);
            if (angleOffset > rotateSpeed)
            {
                angleOffset = rotateSpeed;
            }
            //将自身forward朝向慢慢转向最终朝向
            transform.forward = Vector3.Lerp(transform.forward, finalForward, speed / angleOffset);
        }
    }

}

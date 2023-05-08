using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateInput : IUserInput
{
    public enum EnemyAttackState
    {
        Idle, //没发现敌人时待机
        Pursuid,//发现时追迹
        Attack,//攻击范围内攻击
        Back,//远离后回到原位


        Null
    }
    EnemyAttackState enemyState;
    public GameManager gm;
    public GameObject targetPlayer;
    public ActorManager targetAm;
    public ActorManager am;
    public Transform[] points;
    public Hp hpBar;
    private int destPoint = 0;
    NavMeshAgent nma; /// NavMeshAgent
    [SerializeField] private Mesh cylinderMesh;
    [SerializeField] private float searchRange;
    [SerializeField] private float attackRange = 1.5f;
    void Start()
    {
        targetAm = targetPlayer.GetComponent<ActorManager>();
        am = GetComponent<ActorManager>();
        nma = GetComponent<NavMeshAgent>();
        enemyState = EnemyAttackState.Idle;
        // 禁用自动制动将允许点之间的
        // 连续移动（即，代理在接近目标点时不会减速）。
        if (nma != null)
        {
            nma.autoBraking = false;
        }
        //StartCoroutine(Timer());
        enemyState = EnemyAttackState.Idle;
        hpBar = GetComponentInChildren<Hp>();
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }
    public EnemyAttackState GetenemyState()
    {
        return enemyState;
    }
    public void EnemyIdle()
    {

        rb = false;
        if (!SearchPlayer())
        {
            if (nma == null) return;
            Vector3 vd = nma.destination - transform.position;
            Vector3 normal = vd.normalized;
            DirectionUp = -normal.z;
            DirectionRight = -normal.x;
            if (!nma.pathPending && nma.remainingDistance < 0.5f)
            {
                ////随机取一个方向
                //Vector3 dir = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f)).normalized;
                ////在这个方向的基础上取一个目标点
                ////Vector3 targetPos = transform.position + transform.rotation * dir * Random.Range(0, 10f);/*(minDis, maxDis);*/
                //Vector3 targetPos = transform.position + transform.rotation * dir * Random.Range(0, 10f);
                //SetTarget(targetPos);

                GotoNextPoint();
            }

        }
        else
        {
            //声音

            //靠近
            enemyState = EnemyAttackState.Pursuid;
        }

    }
    public void EnemyPursuid()
    {
        //if (lockon == false)
        //    lockon = true; //发现后锁定
        FaceToTarget();
        rb = false;
        if (nma == null) return;
        if (!SearchPlayer())
        {
            enemyState = EnemyAttackState.Back;
        }
        if (IsInAttackRange())
        {
            enemyState = EnemyAttackState.Attack;
        }
        MoveToTarget();
    }
    public void EnemyAttack()
    {
        FaceToTarget();
        if (!IsInAttackRange())
        {
            am.ac.anim.ResetTrigger("Attack");
            enemyState = EnemyAttackState.Pursuid;

        }
        if (rb && !am.sm.isAttack)
        {
            am.ac.anim.SetTrigger("Attack");
        }
        targetDup = 0;
        targetDright = 0;
        if (gm.count % 3 == 0)
            rb = false;
        else rb = true;

    }




    public void EnemyBack()
    {
        if (nma == null) return;
        rb = false;
        am.sm.HP = am.sm.HPMax;
        Vector3 vd = nma.destination - transform.position;
        Vector3 normal = vd.normalized;
        DirectionUp = -normal.z;
        DirectionRight = -normal.x;
        nma.SetDestination(points[destPoint].position);

        transform.SetPositionAndRotation(transform.position, Quaternion.Lerp(transform.rotation, new Quaternion(0, -180, 0, 0), 0.5f)); //转向恢复默认
        if (!nma.pathPending && nma.remainingDistance < 0.5f)
        {
            DirectionUp = 0;
            DirectionRight = 0;
            //am.ac.anim.SetFloat("Right", 0f);
            enemyState = EnemyAttackState.Idle;

        }

    }
    void Update()
    {
        //ui
        UiHp();
        //move
        if (!inputEnabled)
        {
            targetDup = 0;
            targetDright = 0;
        }
        DirectionUp = Mathf.SmoothDamp(DirectionUp, targetDup, ref velocityDup, 0.1f);
        DirectionRight = Mathf.SmoothDamp(DirectionRight, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(DirectionUp, DirectionRight));
        float DirectionUp2 = tempDAxis.x;
        float DirectionRight2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(DirectionUp2 * DirectionUp2 + DirectionRight2 * DirectionRight2);
        Dvec = DirectionRight2 * transform.right + DirectionUp2 * transform.forward;

        switch (enemyState)
        {
            case EnemyAttackState.Idle:
                EnemyIdle();
                break;
            case EnemyAttackState.Pursuid:
                EnemyPursuid();
                // StartCoroutine(EnemyPursuid());
                break;
            case EnemyAttackState.Attack:
                EnemyAttack();

                //StartCoroutine(EnemyAttack());
                break;
            case EnemyAttackState.Back:
                EnemyBack();
                break;

        }


    }
    public bool SearchPlayer()
    {
        if (targetPlayer == null) return false;
        Vector3 vec = targetPlayer.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, vec);

        if (vec.sqrMagnitude < searchRange * searchRange && angle < 45f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void MoveToTarget()
    {
        if (targetPlayer == null)
        {
            return;
        }
        Vector3 targetPos = targetPlayer.transform.position;
        Vector3 vec = transform.position - targetPos;
        vec.y = 0;
        targetPos += vec.normalized * attackRange * 0.9f;
        targetDup = 1;
        targetDright = 0;

        SetTarget(targetPos);

    }
    public void FaceToTarget()
    {
        if (targetPlayer != null)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)), 50 * Time.deltaTime);
    }

    public void SetFollowNavMeshAgent(bool follow)
    {
        if (!follow)
        {
            nma.ResetPath();
        }
        nma.enabled = follow;
    }
    public void SetTarget(Vector3 tarPos)
    {
        nma.destination = tarPos;
    }
    public bool IsNavMeshPathInvalid()
    {
        if (nma.pathStatus == NavMeshPathStatus.PathPartial ||
            nma.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            return true;
        }
        return false;
    }
    public void SetForward(Vector3 forward)
    {
        Quaternion targetRotation = Quaternion.LookRotation(forward);
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            nma.angularSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }
    void GotoNextPoint()
    {
        // 如果未设置任何点，则返回
        if (points.Length == 0)
            return;

        //将代理设置为前往当前选定的目标。
        nma.destination = points[destPoint].position;

        //选择数组中的下一个点作为目标，
        // 如有必要，循环到开始。
        destPoint = (destPoint + 1) % points.Length;


    }
    public bool IsInAttackRange()
    {
        if (targetPlayer == null)
        {
            return false;
        }
        Vector3 toTarget = targetPlayer.transform.position - transform.position;
        return toTarget.sqrMagnitude < attackRange * attackRange;
    }
    public void UiHp()
    {
        Vector3 vec = transform.position - targetPlayer.transform.position; //player到敌人的向量（player是敌人的target！）
        float dot = Vector3.Dot(targetAm.transform.forward, vec);
        Vector3 toTarget = targetPlayer.transform.position - transform.position;
        if (enemyState != EnemyAttackState.Idle && dot > 0)
        {
            hpBar.gameObject.SetActive(true);
        }
        else
        {
            if (targetAm.ac.camcon.lockState /*&& dot > 0*/ && toTarget.sqrMagnitude < 100)
            {
                hpBar.gameObject.SetActive(true);
            }
            else
            {
                hpBar.gameObject.SetActive(false);
            }

        }



    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireMesh(cylinderMesh, transform.position, Quaternion.identity,
            new Vector3(searchRange * 2f, 0.5f, searchRange * 2f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(cylinderMesh, transform.position, Quaternion.identity,
          new Vector3(attackRange * 2f, 0.5f, attackRange * 2f));
        // Gizmos.color = Color.green;
        //Gizmos.DrawWireMesh(cylinderMesh, transform.position, Quaternion.identity,
        // new Vector3(baseRange * 2f, 0.5f, baseRange * 2f));

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationCannon : MonoBehaviour
{
    public GameObject firPoint;
    public GameObject[] bullets;
    public BossAI boss;
    //public bool onFire; 读取boss的
    public int cannonType; //0：上方 ，1：下方
    //private float speed;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.am.sm.isDie)
        {
            //动画等

            //
            gameObject.SetActive(false);
        }
        Onfire();
    }

    public void Onfire()
    {
        //boss 发出onfire指令后，根据随机的种类选择协程组合，type分辨上下
        if (boss.onFire)
        {
            if (cannonType == 0)
                //StartCoroutine(FirShotgun());
                //StartCoroutine(FirRound(3, transform.position));
                //StartCoroutine(FireTurbine());
                StartCoroutine(FirRoundGroup());
            else if (cannonType == 1)
            {
                //StartCoroutine(FirShotgun());
                StartCoroutine(FireTurbine());
            }

        }

        //if (/*boss.onFire*/Input.GetKeyDown(KeyCode.X))
        //{
        //    if (cannonType == 0)
        //        //StartCoroutine(FirShotgun());
        //        //StartCoroutine(FirRound(3, transform.position));
        //        //StartCoroutine(FireTurbine());
        //        StartCoroutine(FirRoundGroup());
        //    else if (cannonType == 1)
        //    {
        //        //StartCoroutine(FirShotgun());
        //        StartCoroutine(FireTurbine());
        //    }

        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    StartCoroutine(FirShotgun());
        //    //StartCoroutine(FirRound(3, transform.position));
        //    //StartCoroutine(FireTurbine());
        //    //StartCoroutine(FirRoundGroup());

        //}

    }
    IEnumerator FirShotgun()
    {
        Vector3 bulletDir = firPoint.transform.forward; //前方向
        Quaternion leftRota = Quaternion.AngleAxis(-30, Vector3.up);
        Quaternion RightRota = Quaternion.AngleAxis(30, Vector3.up); //使用四元数制造2个旋转，分别是绕Z轴朝左右旋转30度（看好模型的坐标系！！！）
        for (int i = 0; i < 4; i++)     //散弹发射次数
        {
            for (int j = 0; j < 3; j++) //一次发射3颗子弹
            {
                switch (j)
                {
                    case 0:
                        CreatBullet(1, bulletDir, firPoint.transform.position);  //发射第一颗子弹，方向不需要进行旋转。参数为子弹运动方向与生成位置，函数实现未列出。
                        break;
                    case 1:
                        bulletDir = RightRota * bulletDir;//第一个方向子弹发射完毕，旋转方向到下一个发射方向
                        CreatBullet(1, bulletDir, firPoint.transform.position);//调用生成子弹函数，参数为发射方向与生成位置。
                        break;
                    case 2:
                        bulletDir = leftRota * (leftRota * bulletDir); //右边方向发射完毕，得向左边旋转2次相同的角度才能到达下一个发射方向
                        CreatBullet(1, bulletDir, firPoint.transform.position);
                        bulletDir = RightRota * bulletDir; //一轮发射完毕，重新向右边旋转回去，方便下一波使用
                        break;
                }
            }
            yield return new WaitForSeconds(0.5f); //协程延时0.5秒进行下一波发射
        }

    }
    IEnumerator FirRound(int number, Vector3 creatPoint)//参数为发射波数与子弹生成点
    {
        Vector3 bulletDir = firPoint.transform.forward;//发射方向
        int n = 30;//写到函数条件中
        Quaternion rotateQuate = Quaternion.AngleAxis(n, Vector3.up);//使用四元数制造绕Z轴旋转n度的旋转
        for (int i = 0; i < number; i++)    //发射波数
        {
            for (int j = 0; j < 360 / n; j++)
            {
                CreatBullet(0, bulletDir, creatPoint);   //生成子弹
                bulletDir = rotateQuate * bulletDir; //让发射方向旋转n度，到达下一个发射方向
            }
            yield return new WaitForSeconds(1f); //协程延时，1秒进行下一波发射
        }
        yield return null;
    }
    IEnumerator FirRoundGroup()
    {
        Vector3 bulletDir = firPoint.transform.forward;
        Quaternion rotateQuate = Quaternion.AngleAxis(45, Vector3.right);//使用四元数制造绕Z轴旋转45度的旋转
        List<Bullet> bullets = new List<Bullet>();       //装入开始生成的8个弹幕
        for (int i = 0; i < 8; i++)
        {
            var tempBullet = CreatBullet(0, bulletDir, firPoint.transform.position);
            bulletDir = rotateQuate * bulletDir; //生成新的子弹后，发射方向旋转45度，到达下一个发射方向
            bullets.Add(tempBullet);
        }
        yield return new WaitForSeconds(1.0f);   //1秒后在生成多波弹幕
        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i] != null)
                //bullets[i].speed = 0; //弹幕停止移动
                StartCoroutine(FirRound(1, bullets[i].transform.position));//通过之前弹幕的位置，生成多波多方向的圆形弹幕。调用上面圆形弹幕函数
        }
    }
    IEnumerator FireTurbine()
    {
        Vector3 bulletDir = firPoint.transform.forward;      //发射方向
        Quaternion rotateQuate = Quaternion.AngleAxis(20, Vector3.up);//使用四元数制造绕Z轴旋转20度的旋转
        float radius = 0.6f;        //生成半径
        float distance = 0.2f;      //每生成一次增加的距离
        for (int i = 0; i < 360 / 20; i++)
        {
            Vector3 firePoint = firPoint.transform.position + bulletDir * radius;   //使用向量计算生成位置
            //StartCoroutine(FirRound(1, firePoint));     //在算好的位置生成一波圆形弹幕
            CreatBullet(1, bulletDir, firePoint);
            yield return new WaitForSeconds(0.05f);     //延时较小的时间（为了表现效果），计算下一步
            bulletDir = rotateQuate * bulletDir;        //发射方向改变
            radius += distance;     //生成半径增加
        }
    }
    public Bullet CreatBullet(int bulletNo, Vector3 dir, Vector3 position)
    {
        GameObject m = Instantiate(bullets[bulletNo]);
        m.transform.position = position;
        m.transform.forward = dir;
        return m.GetComponent<Bullet>();

    }

}

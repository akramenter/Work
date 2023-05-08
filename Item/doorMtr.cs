using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorMtr : MonoBehaviour
{
    public Material mat;
    public MeshRenderer rend;
    public Texture texture;
    private int timer;
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        texture = rend.material.mainTexture;
        mat = Resources.Load("dissolve") as Material;
        if (mat == null)
        {
            Debug.Log("Don't Find Material");
            return;
        }
        //rend.enabled = true;
        rend.sharedMaterial = mat;//代表这个对象的共享材质资源（这个是替换材质球）
                                  //MeshRenderer继承自Renderer所以上面定义成MeshRenderer也可以,

        // GetComponent<MeshRenderer>().materials[0] = mat;//这个表示找到对应的材质但是不能替换材质球，
        GetComponent<MeshRenderer>().material.mainTexture = texture;//和上面的一样，可以替换材质的texture

    }


    void Update()
    {
        if (mat == Resources.Load("dissolve") as Material)
        {
            float j = timer++ * 0.002f;
            if (j < 1)
            {
                rend.material.EnableKeyword("_DISSOLVEEFFECT_ON"); //控制选项卡中的勾选
                rend.material.SetFloat("_Clip", j);
            }
        }

    }
}

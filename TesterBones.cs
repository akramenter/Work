using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterBones : MonoBehaviour
{
    public SkinnedMeshRenderer srcMeshRenderer; //source
    public SkinnedMeshRenderer tgtMeshRenderer;//target

    void Start()
    {
        tgtMeshRenderer.bones = srcMeshRenderer.bones;
    }


    void Update()
    {

    }
}

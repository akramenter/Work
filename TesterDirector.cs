using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TesterDirector : MonoBehaviour
{
    public PlayableDirector pd;
    public Animator attacker;
    public Animator victim;
    void Start()
    {

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var track in pd.playableAsset.outputs)
            {
                switch (track.streamName)
                {
                    case "Attacker Animation":
                        pd.SetGenericBinding(track.sourceObject, attacker);
                        break;
                    case "Victim Animation":
                        pd.SetGenericBinding(track.sourceObject, victim);
                        break;
                }

            }
            //pd.time = 0; //时间清零
            //pd.Stop();//停止播放
            //pd.Evaluate();//重新计算位置
            pd.Play();
        }
    }
}

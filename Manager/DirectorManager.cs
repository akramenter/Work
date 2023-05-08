using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManagerInterface
{

    public PlayableDirector pd;
    [Header("=== Timeline Assets ===")]
    public TimelineAsset frontStab;
    public TimelineAsset openBox;
    public TimelineAsset leverUp;
    [Header("=== Assets Settings ===")]
    public ActorManager attacker;
    public ActorManager victim;

    void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;
        if (attacker != null)
        {
            if (attacker.ac.camcon.isAI)
                victim = GameObject.FindGameObjectWithTag("Player").GetComponent<ActorManager>();
        }

    }

    void Update()
    {

    }
    public bool IsPlaying()
    {
        if (pd.state == PlayState.Playing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void PlayFrontStab(string timelineName, ActorManager attacker, ActorManager victim)
    {
        //正在播放中，再次按键不会从头播放
        //if (pd.time != 0)
        //{
        //    return;
        //}

        switch (timelineName)
        {
            case "frontStab":
                pd.playableAsset = Instantiate(frontStab);
                TimelineAsset timeline = (TimelineAsset)pd.playableAsset;
                foreach (var track in timeline.GetOutputTracks())
                {
                    switch (track.name)
                    {
                        case "Attacker Animation":
                            pd.SetGenericBinding(track, attacker.ac.anim);
                            break;
                        case "Victim Animation":
                            pd.SetGenericBinding(track, victim.ac.anim);
                            break;
                        case "Attacker Script":
                            pd.SetGenericBinding(track, attacker);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString(); //初始化exposedName
                                pd.SetReferenceValue(myClip.am.exposedName, attacker);
                            }
                            break;
                        case "Victim Script":
                            pd.SetGenericBinding(track, victim);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString();
                                pd.SetReferenceValue(myClip.am.exposedName, victim);
                            }
                            break;
                    }
                }
                pd.Evaluate();
                pd.Play();
                am.sm.canBreak = false;
                victim.sm.AddHP(-2.0f * am.wm.wcR.GetATK()); //敌人被斩杀掉血（后改成变化数值）
                if (victim.sm.HP <= 0) victim.sm.HP = 1;
                break;

            case "openBox":
                pd.playableAsset = Instantiate(openBox);
                timeline = (TimelineAsset)pd.playableAsset;
                foreach (var track in timeline.GetOutputTracks())
                {
                    switch (track.name)
                    {
                        case "Player Animation":
                            pd.SetGenericBinding(track, attacker.ac.anim);
                            break;
                        case "Box Animation":
                            pd.SetGenericBinding(track, victim.ac.anim);
                            break;
                        case "Player Script":
                            pd.SetGenericBinding(track, attacker);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString(); //初始化exposedName
                                pd.SetReferenceValue(myClip.am.exposedName, attacker);
                            }
                            break;

                        case "Box Script":
                            pd.SetGenericBinding(track, victim);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString();
                                pd.SetReferenceValue(myClip.am.exposedName, victim);
                            }
                            break;
                    }
                }
                pd.Evaluate();
                pd.Play();

                break;

            case "leverUp":
                pd.playableAsset = Instantiate(leverUp);
                timeline = (TimelineAsset)pd.playableAsset;
                foreach (var track in timeline.GetOutputTracks())
                {
                    switch (track.name)
                    {
                        case "Player Animation":
                            pd.SetGenericBinding(track, attacker.ac.anim);
                            break;
                        case "Lever Animation":
                            pd.SetGenericBinding(track, victim.ac.anim);
                            break;
                        case "Player Script":
                            pd.SetGenericBinding(track, attacker);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString(); //初始化exposedName
                                pd.SetReferenceValue(myClip.am.exposedName, attacker);
                            }
                            break;

                        case "Lever Script":
                            pd.SetGenericBinding(track, victim);
                            foreach (var clip in track.GetClips())
                            {
                                MyPlayableClip myClip = (MyPlayableClip)clip.asset;
                                MyPlayableBehaviour myBehav = myClip.template;
                                myClip.am.exposedName = System.Guid.NewGuid().ToString();
                                pd.SetReferenceValue(myClip.am.exposedName, victim);
                            }
                            break;
                    }
                }
                pd.Evaluate();
                pd.Play();
                break;


        }

    }
}

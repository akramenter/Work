using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : IActorManagerInterface
{
    private CapsuleCollider interCol;
    public List<EventCasterManager> overLapEcastms = new List<EventCasterManager>();
    void Start()
    {
        interCol = GetComponent<CapsuleCollider>();
    }


    void OnTriggerEnter(Collider col)
    {
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();

        foreach (var ecastm in ecastms)
        {
            if (!overLapEcastms.Contains(ecastm))
            {
                overLapEcastms.Add(ecastm);
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        EventCasterManager[] ecastms = col.GetComponents<EventCasterManager>();
        foreach (var ecastm in ecastms)
        {
            if (overLapEcastms.Contains(ecastm))
            {
                overLapEcastms.Remove(ecastm);
            }
        }
    }

}

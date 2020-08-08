using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    [SerializeField] 
    public MobStats mobStats;
    
    [SerializeField] 
    public BehaviourFunction behaviour;

    public MobSceneData sceneData;

    void Start()
    {
        sceneData = new MobSceneData();
        Supervisor.self.initMobSceneData(this, sceneData);
    }

    void Update()
    {
        behaviour.doActions(Time.deltaTime, this.gameObject, mobStats, sceneData);
    }
}

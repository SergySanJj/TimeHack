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

    private float currentHealth;

    void Start()
    {
        currentHealth = mobStats.maxHealth;

        sceneData = new MobSceneData();
        Supervisor.self.initMobSceneData(this, sceneData);
    }

    void Update()
    {
        if (!isAlive())
        {
            die();
            return;
        }


        behaviour.doActions(Time.deltaTime, this.gameObject, mobStats, sceneData);
    }

    public void receiveDamage(float damageValue)
    {
        currentHealth -= damageFunction(damageValue);
    }

    private bool isAlive()
    {
        return currentHealth > 0;
    }

    public void updateGameData()
    {
        Supervisor.self.initMobSceneData(this, sceneData);
    }

    private void die()
    {
        Supervisor.self.kill(this);
        Destroy(this.gameObject);
        Debug.Log("Mob death");
    }

    private float damageFunction(float damageValue)
    {
        return Random.Range(damageValue * 0.2f, damageValue);
    }

}

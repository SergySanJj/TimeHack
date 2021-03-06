﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public float lastTimeAttack = 0.0f;
    public float lastChangeTarget = 0.0f;
    public float changeEvery = 5f;

    [SerializeField] 
    public MobStats mobStats;
    
    [SerializeField] 
    public BehaviourFunction behaviour;

    public MobSceneData sceneData;

    private float currentHealth;

    public GameObject target = null;

    void Start()
    {
        currentHealth = mobStats.maxHealth;
        changeEvery = Random.Range(2f, 5f);

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

        if (Supervisor.playtime - lastChangeTarget > changeEvery)
        {
            Supervisor.chooseTarget(this, mobStats, sceneData);
            lastChangeTarget = Supervisor.playtime;
            changeEvery = Random.Range(3f, 5f);
        }


        behaviour.doActions(Time.deltaTime, this.gameObject, mobStats, sceneData);
    }

    public void receiveDamage(float damageValue)
    {
        currentHealth -= damageFunction(damageValue);

        if (currentHealth < mobStats.maxHealth / 2)
        {
            target = Supervisor.randomFrom(Supervisor.self.pointsOfInterest);
            changeEvery = Random.Range(4f, 7f);
        } 
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


    public bool canAttack(MobStats selfStats)
    {
        return Supervisor.playtime - lastTimeAttack > selfStats.attackPause;
    }


    public void attack(GameObject self, MobStats selfStats)
    {
        lastTimeAttack = Supervisor.playtime;
        Supervisor.self.attackSignal(self, selfStats);
        playAttackAnimation(self);
    }

    private void playAttackAnimation(GameObject gameObject)
    {
        float jumpHeight = 1f;
        float jumpTime = 0.1f;
        LeanTween.move(gameObject, gameObject.transform.position + new Vector3(0, jumpHeight, 0), jumpTime).setEase(LeanTweenType.easeInQuad);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supervisor : MonoBehaviour
{
    public Joystick joystick;

    public static float playtime = 0f;
    private static float lastTime = 0f;

    private static int rollbackSeconds = 5;

    public static Supervisor self;

    public static SceneState currentSceneState = new SceneState();

    public static Queue<SceneState> history = new Queue<SceneState>();

    [SerializeField]
    public Spawner playerSpawner;

    [SerializeField]
    public Spawner redTeamSpawner;

    [SerializeField]
    public Spawner blueTeamSpawner;

    [SerializeField]
    public int redTeamsSize = 5;

    [SerializeField]
    public int blueTeamsSize = 5;

    public class SceneState
    {
        public List<Player> players = new List<Player>();
        public List<Mob> redTeam = new List<Mob>();
        public List<Mob> blueTeam = new List<Mob>();
    }

    private void Awake()
    {
        self = this;
        spawnAll();
    }

    void Start()
    {
    }

    void Update()
    {
        playtime += Time.deltaTime;
        if (playtime - lastTime > 0.5f)
        {
            lastTime = playtime;
        }
        
    }

    void updateInfo()
    {
        foreach (Mob mob in currentSceneState.redTeam)
        {
            mob.updateGameData();
        }
        foreach (Mob mob in currentSceneState.blueTeam)
        {
            mob.updateGameData();
        }
    }

    public void spawnAll()
    {
        currentSceneState.players.Add(playerSpawner.spawn().GetComponent<Player>());
        currentSceneState.players[0].joystick = joystick;

        for (int i = 0; i < blueTeamsSize; i++)
        {
            currentSceneState.blueTeam.Add(blueTeamSpawner.spawn(i, blueTeamsSize).GetComponent<Mob>());
        }

        for (int i = 0; i < redTeamsSize; i++)
        {
            currentSceneState.redTeam.Add(redTeamSpawner.spawn(i, redTeamsSize).GetComponent<Mob>());
        }
    }


    public void initMobSceneData(Mob caller, MobSceneData sceneData)
    {
        sceneData.players = currentSceneState.players;
        if (caller.mobStats.team == 1)
        {
            sceneData.allies = currentSceneState.redTeam;
            sceneData.enemies = currentSceneState.blueTeam;
        } else
        {
            sceneData.allies = currentSceneState.blueTeam;
            sceneData.enemies = currentSceneState.redTeam;
        }
    }

    public void kill(Mob mob)
    {
        if (mob.mobStats.team == 1)
        {
            currentSceneState.redTeam.Remove(mob);
        } else
        {
            currentSceneState.blueTeam.Remove(mob);
        }
    }

    public void kill(Player player)
    {
        Vector3 lastPosition = player.transform.position;
        currentSceneState.players.Remove(player);
        Debug.Log("Player is dead");
    }

    public void attackSignal(GameObject from, MobStats stats)
    {
        Vector3 pos = from.transform.position;
        if (stats.team == 0)
        {
            foreach (Mob enemy in currentSceneState.blueTeam)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }
            foreach (Mob enemy in currentSceneState.redTeam)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }
        }
        else if (stats.team == 1)
        {
            foreach (Player enemy in currentSceneState.players)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }

            foreach ( Mob enemy in currentSceneState.blueTeam)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }
        }
        else
        {
            foreach (Player enemy in currentSceneState.players)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }

            foreach (Mob enemy in currentSceneState.redTeam)
            {
                float dist = Vector3.Distance(enemy.transform.position, from.transform.position);
                if (dist < stats.attackRadius)
                    enemy.receiveDamage(stats.damage);
            }
        }
    }
}

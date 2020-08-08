using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supervisor : MonoBehaviour
{
    public Joystick joystick;

    private static float playtime = 0f;
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
    int teamsSize = 5;

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
            updateInfo();
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

        for (int i = 0; i < teamsSize; i++)
        {
            currentSceneState.blueTeam.Add(blueTeamSpawner.spawn(i, teamsSize).GetComponent<Mob>());
        }

        for (int i = 0; i < teamsSize; i++)
        {
            currentSceneState.redTeam.Add(redTeamSpawner.spawn(i, teamsSize).GetComponent<Mob>());
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supervisor : MonoBehaviour
{
    [SerializeField]
    public GameObject limitTopLeft;
    [SerializeField]
    public GameObject limitBotRight;

    [SerializeField]
    public Color playerColor;
    [SerializeField]
    public Color ghostColor;

    [SerializeField]
    public HealthBar playerHealthBar;

    public Joystick joystick;

    public static float playtime = 0f;
    private static float lastTime = 0f;
    private static int rollbackSeconds = 5;
    private static float historyFrequency = 0.05f;
    private static int maxHistoryElements = (int)(rollbackSeconds / historyFrequency);

    private History history = new History();
    private History.PlayerHistory currentPlayerHistory = new History.PlayerHistory();

    public static Supervisor self;

    public static SceneState currentSceneState = new SceneState();


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

    private static System.Random rnd = new System.Random();


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
        if (playtime - lastTime > historyFrequency)
        {
            if (currentSceneState.players.Count > 0)
            {

                foreach (Player player in currentSceneState.players)
                {
                    if (player.isRespawned)
                    {
                        if (player.hasHistoryItems())
                            player.takeNextTarget();
                        else
                        {
                            player.die();
                            return;
                        }
                    }
                }

                if (currentSceneState.players.Count > 0)
                {
                    currentPlayerHistory.addHistoryPoint(currentSceneState.players[currentSceneState.players.Count - 1], maxHistoryElements);
                } else
                {
                    return;
                }
                lastTime = playtime;
            }

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
        currentSceneState.players[0].setLightColor(playerColor);
        currentSceneState.players[0].joystick = joystick;
        currentSceneState.players[0].setBorders(limitTopLeft, limitBotRight);
        playerHealthBar.changeSubscription(currentSceneState.players[0]);

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
        if (player.isRespawned)
        {
            currentSceneState.players.Remove(player);
            Debug.Log("Player copy is dead");
            return;
        }
        History.PlayerHistory.PlayerHistoryElement rollBackPlayer = currentPlayerHistory.getPlayback();

        if (rollBackPlayer == null)
        {
            Debug.Log("You lose");
            return;
        }

        // Ghost
        var ghost = Instantiate(player);
        rollBackPlayer.position.y = 1.5f;
        ghost.transform.position = rollBackPlayer.position;
        ghost.currentHealth = rollBackPlayer.health;
        Debug.Log("Respawn with " + rollBackPlayer.health);
        ghost.respawnWith(currentPlayerHistory);
        ghost.setLightColor(ghostColor);
        ghost.setBorders(limitTopLeft, limitBotRight);
        currentSceneState.players.Add(ghost);


        // Playable player char
        var respawned = Instantiate(player);
        rollBackPlayer.position.y = 3.0f;
        respawned.transform.position = rollBackPlayer.position;
        respawned.currentHealth = rollBackPlayer.health;
        respawned.setBorders(limitTopLeft, limitBotRight);
        playerHealthBar.changeSubscription(respawned);
        currentSceneState.players.Add(respawned);

        history.needToBePlayed.Add(currentPlayerHistory);
        currentPlayerHistory = new History.PlayerHistory();

        currentSceneState.players.Remove(player);
        Debug.Log("Player is dead");
    }

    public void attackSignal(GameObject from, MobStats stats)
    {
        Vector3 pos = from.transform.position;
        if (stats.team == 0)
        {
            attackIfCan(currentSceneState.redTeam, stats, pos);
            attackIfCan(currentSceneState.blueTeam, stats, pos);
        }
        else if (stats.team == 1)
        {
   
            attackIfCan(currentSceneState.players, stats, pos);
            attackIfCan(currentSceneState.blueTeam, stats, pos);
        }
        else
        {
            attackIfCan(currentSceneState.players, stats, pos);
            attackIfCan(currentSceneState.redTeam, stats, pos);
        }
    }

    private void attackIfCan(List<Mob> mobs, MobStats stats, Vector3 attackPosition)
    {
        foreach (Mob enemy in mobs)
        {
            float dist = Vector3.Distance(enemy.transform.position, attackPosition);
            if (dist < stats.attackRadius)
                enemy.receiveDamage(stats.damage);
        }
    }

    private void attackIfCan(List<Player> mobs, MobStats stats, Vector3 attackPosition)
    {
        foreach (Player enemy in mobs)
        {
            float dist = Vector3.Distance(enemy.transform.position, attackPosition);
            if (dist < stats.attackRadius)
                enemy.receiveDamage(stats.damage);
        }
    }

    private void OnDestroy()
    {
        currentSceneState = new SceneState();
        currentPlayerHistory = new History.PlayerHistory();

    }

    public static void chooseTarget(Mob self, MobStats selfStats, MobSceneData sceneData)
    {
        if (chance(0.15f))
        {
            // select some player
            self.target = randomFrom(Supervisor.currentSceneState.players);
        }
        else
        {
            // select enemy
            if (selfStats.team == 1)
            {
                self.target = randomFrom(Supervisor.currentSceneState.blueTeam);
            }
            else
            {
                self.target = randomFrom(Supervisor.currentSceneState.redTeam);
            }
        }
    }



    private static GameObject randomFrom(List<Mob> lst)
    {
        if (lst.Count > 0)
            return lst[rnd.Next(lst.Count)].gameObject;
        else return null;
    }

    private static GameObject randomFrom(List<Player> lst)
    {
        if (lst.Count > 0)
        {
            int ind = rnd.Next(lst.Count);
            if (lst[ind] != null)
                return lst[ind].gameObject;
            else
                return null;
        }
        else return null;
    }

    private static bool chance(float possibility)
    {
        return UnityEngine.Random.Range(0f, 1f) < possibility;
    }
}

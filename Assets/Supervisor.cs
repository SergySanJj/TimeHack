using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supervisor : MonoBehaviour
{

    [SerializeField]
    public Player player;

    private static float playtime = 0f;

    private static int rollbackSeconds = 5;

    public static Supervisor self;

    public static SceneState currentSceneState = new SceneState();

    public static Queue<SceneState> history = new Queue<SceneState>();

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
    }

    public void spawnAll()
    {
        currentSceneState.players.Add(player);
    }

    public void initMobSceneData(Mob caller, MobSceneData sceneData)
    {
        sceneData.players = new List<Player>(currentSceneState.players);
    }
}

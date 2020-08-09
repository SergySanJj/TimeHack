using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject limitTopLeft;
    public GameObject limitBotRight;

    public void setBorders(GameObject tl, GameObject br)
    {
        limitTopLeft = tl;
        limitBotRight = br;
    }

    public bool isRespawned = false;

    [SerializeField]
    public MobStats stats;

    public float currentHealth;
    public void setHealth(float value)
    {
        if (value > 0)
            currentHealth = value;
        else 
            currentHealth = 0;
        onChangePlayerHealth?.Invoke(value / stats.maxHealth);
    }

    private float lastTimeAttack = 0.0f;

    Vector3 forward, right;

    public Joystick joystick;


    public event Action<float> onChangePlayerHealth;
    public void ChangeHealth(float val)
    {
        onChangePlayerHealth?.Invoke(val);
    }

    private void OnEnable()
    {
        setHealth(stats.maxHealth);
    }

    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        GameEvents.current.onPlayerAttackButton += tryAttack;
    }

    void Update()
    {
        if (!isAlive())
        {
            die();
            return;
        }

        if (isRespawned)
        {
            followHistoryCheckpoints(Time.deltaTime);
        } else
        {
            if (Input.anyKey || Input.touchCount > 0)
                Move();
        }
    }

    private History.PlayerHistory playerHistory;
    private Vector3 targetPos = new Vector3(0, 0, 0);

    public void respawnWith(History.PlayerHistory history)
    {
        this.playerHistory = history;
        isRespawned = true;
    }

    public void takeNextTarget()
    {
        if (hasHistoryItems())
        {
            History.PlayerHistory.PlayerHistoryElement el = playerHistory.history.Dequeue();
            targetPos = el.position;
        } else {
            die();
        }

    }

    public bool hasHistoryItems()
    {
        return (playerHistory.history.Count > 0);
    }

    private void followHistoryCheckpoints(float deltaTime)
    {
        if (targetPos == null)
            return;

        float dist = Vector3.Distance(targetPos, transform.position);

        if (dist < 0.1)
            return;

        Vector3 dir = targetPos - transform.position;
        dir = Vector3.Normalize(dir);
        transform.position += dir * stats.moveSpeed * deltaTime;

        Vector3 heading = Vector3.Normalize(dir);
        heading.y = 0;

        if (heading.sqrMagnitude > 0.01)
            transform.forward = heading;

        transform.position = limitMovement(transform.position);
    }

    private void Move()
    {
        Vector3 rightMovement = right * stats.moveSpeed * Time.deltaTime * joystickPositionScaler(joystick.Horizontal);
        Vector3 upMovement = forward * stats.moveSpeed * Time.deltaTime * joystickPositionScaler(joystick.Vertical);

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        if (heading.sqrMagnitude > 0.001 )
            transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;

        transform.position = limitMovement(transform.position);
    }

    private float joystickPositionScaler(float position)
    {
        float mult;
        if (position > 0) mult = 1f; else mult = -1f;

        if (Math.Abs(position) < 0.2)
            return 0f;
        if (Math.Abs(position) < 0.5)
            return 0.5f * mult;
        else
            return 1.0f * mult;
    }

    public bool canAttack()
    {
        return Supervisor.playtime - lastTimeAttack > stats.attackPause;
    }

    public void attack()
    {
        lastTimeAttack = Supervisor.playtime;
        Supervisor.self.attackSignal(this.gameObject, stats);
        Debug.Log("Player Attacks");
        playAttackAnimation(this.gameObject);
    }

    private void playAttackAnimation(GameObject gameObject)
    {
        float jumpHeight = 0.5f;
        float jumpTime = 0.1f;
        float rotationTime = 0.4f;
        LeanTween.move(gameObject, limitMovement(gameObject.transform.position + new Vector3(0, jumpHeight, 0)), jumpTime).setEase(LeanTweenType.easeInQuad);
        LeanTween.rotateAround(gameObject, new Vector3(0, 1f, 0), 300, rotationTime);
    }

    public void tryAttack()
    {
        if (canAttack())
            attack();
    }

    public void receiveDamage(float damageValue)
    {
        setHealth(currentHealth - damageValue);
    }

    private void OnDestroy()
    {
        GameEvents.current.onPlayerAttackButton -= tryAttack;
    }

    private bool isAlive()
    {
        return currentHealth > 0;
    }

    public void die()
    {
        Supervisor.self.kill(this);
        Destroy(this.gameObject);
        Debug.Log("Player death");
    }

    public void setLightColor(Color color)
    {
        this.gameObject.GetComponentInChildren<Light>().color = color;
    }

    private Vector3 limitMovement(Vector3 vec)
    {
        if (vec.x < limitTopLeft.gameObject.transform.position.x)
            vec.x = limitTopLeft.transform.position.x;
        if (vec.z > limitTopLeft.transform.position.z)
            vec.z = limitTopLeft.transform.position.z;

        if (vec.x > limitBotRight.transform.position.x)
            vec.x = limitBotRight.transform.position.x;
        if (vec.z < limitBotRight.transform.position.z)
            vec.z = limitBotRight.transform.position.z;

        return vec;
    }
}

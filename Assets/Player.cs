using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public MobStats stats;

    public float currentHealth;
    private float lastTimeAttack = 0.0f;

    Vector3 forward, right;

    public Joystick joystick;

    void Start()
    {
        currentHealth = stats.maxHealth;

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

        if (Input.anyKey || Input.touchCount > 0)
            Move();
    }

    private void Move()
    {
        Vector3 rightMovement = right * stats.moveSpeed * Time.deltaTime * joystick.Horizontal;
        Vector3 upMovement = forward * stats.moveSpeed * Time.deltaTime * joystick.Vertical;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        if (heading.sqrMagnitude > 0.001 )
            transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
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
        LeanTween.move(gameObject, gameObject.transform.position + new Vector3(0, jumpHeight, 0), jumpTime).setEase(LeanTweenType.easeInQuad);
        LeanTween.rotateAround(gameObject, new Vector3(0, 1f, 0), 300, rotationTime);
    }

    public void tryAttack()
    {
        if (canAttack())
            attack();
    }

    public void receiveDamage(float damageValue)
    {
        currentHealth -= damageValue;
    }

    private void OnDestroy()
    {
        GameEvents.current.onPlayerAttackButton -= tryAttack;
    }

    private bool isAlive()
    {
        return currentHealth > 0;
    }

    private void die()
    {
        Supervisor.self.kill(this);
        Destroy(this.gameObject);
        Debug.Log("Player death");
    }
}

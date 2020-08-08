using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public MobStats stats;

    Vector3 forward, right;

    public Joystick joystick;

    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void Update()
    {
        if (Input.anyKey || Input.touchCount > 0)
            Move();
    }

    private void Move()
    {
        Vector3 rightMovement = right * stats.moveSpeed * Time.deltaTime * joystick.Horizontal;
        Vector3 upMovement = forward * stats.moveSpeed * Time.deltaTime * joystick.Vertical;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
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
}

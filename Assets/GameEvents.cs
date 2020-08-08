using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }


    public event Action onPlayerAttackButton;
    public void PlayerAttackButton()
    {
        onPlayerAttackButton?.Invoke();
    }
}

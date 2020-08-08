using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mob Stats", menuName = "Game Charachters/Mob Stats")]
public class MobStats : ScriptableObject
{
    public int team = 1; // 0 - player, 1 - red team, 2 - blue team

    public float maxHealth = 100f;
    public float moveSpeed = 4f;
    public float damage = 10f; 
}

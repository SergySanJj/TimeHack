using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mob Stats", menuName = "Game Charachters/Mob Stats")]
public class MobStats : ScriptableObject
{
    public int playSide = 1; // 0 - player, 1 - red team, 2 - blue team

    public float health = 100f;
    public float moveSpeed = 4f;
    public float damage = 10f; 
}

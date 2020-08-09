using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSide : MonoBehaviour
{
    [SerializeField]
    public int team;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            chooseSide();
        }
   
    }

    public void chooseSide()
    {
        Debug.Log("Side " + team);
        PlayerPrefs.SetInt("Team", team);
        GameLoader.instance.LoadScene(2);
    }
}

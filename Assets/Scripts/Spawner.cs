using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    public float distanceBetween = 2f;

    [SerializeField]
    public GameObject spawnable;

    public Joystick joystick;

    public GameObject spawn(int index, int total)
    {

        Vector3 spawnerPos = this.transform.position;
        float basisOffset = index*distanceBetween - (total / 2) * distanceBetween;
        spawnerPos.x += basisOffset;

        GameObject o = Instantiate(spawnable, spawnerPos, Quaternion.identity);
        return o;
    }

    public GameObject spawn()
    {
        return spawn(0,1);
    }
}

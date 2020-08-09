using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCar : MonoBehaviour
{
    public float magnitude = .2f;
    public float timing = 8.0f;
    private Vector3 center;
    private bool up = false;

    void Start()
    {
        center = this.gameObject.transform.position;
        floating();
    }

    private void floating()
    {
        if (up)
        {
            up = false ;
            LeanTween.move(this.gameObject, center + new Vector3(0, magnitude, 0), timing).setEaseInSine().setOnComplete(floating);

        } else
        {
            up = true;
            LeanTween.move(this.gameObject, center - new Vector3(0, magnitude, 0), timing).setEaseOutSine().setOnComplete(floating);
        }

    }
}

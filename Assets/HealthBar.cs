using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image fillImage = null;

    private Player subscribedTo = null;

    public void changeSubscription(Player player)
    {
        if (subscribedTo != null)
        {
            subscribedTo.onChangePlayerHealth -= setTo;
        }

        subscribedTo = player;
        player.onChangePlayerHealth += setTo;
        setTo(subscribedTo.currentHealth / subscribedTo.stats.maxHealth);
    }

    public void setTo(float val)
    {
        // fillImage.fillAmount = val;
        LeanTween.value(fillImage.fillAmount, val, 0.1f).setOnUpdate((float v)=>
        {
            fillImage.fillAmount = v;
        });
    }

    private void OnDestroy()
    {
        if (subscribedTo != null)
        {
            subscribedTo.onChangePlayerHealth -= setTo;
        }
    }
}

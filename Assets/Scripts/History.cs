using UnityEngine;
using System.Collections.Generic;

public class History 
{
    public List<PlayerHistory> needToBePlayed = new List<PlayerHistory>();


    public class PlayerHistory
    {
        public Queue<PlayerHistoryElement> history = new Queue<PlayerHistoryElement>();

        public void addHistoryPoint(Player player, int maxHistoryPoints)
        {
            PlayerHistoryElement playerHistoryElement = new PlayerHistoryElement();
            playerHistoryElement.position = player.transform.position;
            playerHistoryElement.health = player.currentHealth;
            history.Enqueue(playerHistoryElement);

            while(history.Count > maxHistoryPoints)
            {
                history.Dequeue();
            }
        }

        public PlayerHistoryElement getPlayback()
        {
            return history.Dequeue();
        }

        public class PlayerHistoryElement
        {
            public Vector3 position;
            public float health;
        }
    }    
}

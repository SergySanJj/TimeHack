using UnityEngine;
using System.Collections.Generic;

public class History 
{
    public List<PlayerHistory> needToBePlayed = new List<PlayerHistory>();


    public class PlayerHistory
    {
        public Queue<PlayerHistoryElement> history = new Queue<PlayerHistoryElement>();

        public void addHistoryPoint(Player player, bool wasAttacking, int maxHistoryPoints)
        {
            if (player == null)
                return;

            PlayerHistoryElement playerHistoryElement = new PlayerHistoryElement();
            playerHistoryElement.position = player.transform.position;
            playerHistoryElement.health = player.currentHealth;
            playerHistoryElement.wasAttacking = wasAttacking;
            history.Enqueue(playerHistoryElement);

            while(history.Count > maxHistoryPoints)
            {
                history.Dequeue();
            }
        }

        public PlayerHistoryElement getPlayback()
        {
            if (history.Count > 0)
                return history.Dequeue();
            else
                return null;
        }

        public class PlayerHistoryElement
        {
            public Vector3 position;
            public float health;
            public bool wasAttacking;
        }
    }    
}

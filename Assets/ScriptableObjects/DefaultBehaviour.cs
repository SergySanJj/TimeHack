using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default Behaviour Action", menuName = "Behaviours/Default")]
public class DefaultBehaviour : BehaviourFunction
{

    public float lastTimeAttack = 0.0f;
    

    public override void doActions(float deltaTime, GameObject self, MobStats selfStats, MobSceneData sceneData)
    {
        try
        {
            Vector3 playerPos = sceneData.players[0].transform.position;

            float dist = Vector3.Distance(playerPos, self.transform.position);

            if (dist > 0.9f * selfStats.attackRadius)
            {
                Vector3 dir = playerPos - self.transform.position;
                dir = Vector3.Normalize(dir);
                self.transform.position += dir * selfStats.moveSpeed * deltaTime;
            }
            else
            {
                //if (canAttack(selfStats))
                attack(self, selfStats);
            }
        } catch (MissingReferenceException)
        {
            return;
        }
    }

    public bool canAttack(MobStats selfStats)
    {
        return Supervisor.playtime - lastTimeAttack > selfStats.attackPause;
    }


    public void attack(GameObject self, MobStats selfStats)
    {
        lastTimeAttack = Supervisor.playtime;
        Debug.Log("Mob Attack");
        Supervisor.self.attackSignal(self, selfStats);
    }

}

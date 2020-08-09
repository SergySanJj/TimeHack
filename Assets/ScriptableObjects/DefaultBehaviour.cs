using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default Behaviour Action", menuName = "Behaviours/Default")]
public class DefaultBehaviour : BehaviourFunction
{


    public override void doActions(float deltaTime, GameObject self, MobStats selfStats, MobSceneData sceneData)
    {
        Mob mob = self.GetComponent<Mob>();

        if ( mob.target == null)
        {
            Supervisor.chooseTarget(mob, selfStats, sceneData);
        }

        targetApproachAndAttack(deltaTime, mob, selfStats, sceneData);
    }

    public void targetApproachAndAttack(float deltaTime, Mob self, MobStats selfStats, MobSceneData sceneData)
    {
        if (self.target == null)
            return;

        Vector3 targetPos = self.target.transform.position;
        float dist = Vector3.Distance(targetPos, self.transform.position);

        if (dist > 0.9f * selfStats.attackRadius)
        {
            Vector3 dir = targetPos - self.transform.position;
            dir = Vector3.Normalize(dir);
            self.transform.position += dir * selfStats.moveSpeed * deltaTime;
        }
        else
        {

            if (self.canAttack(selfStats))
                self.attack(self.gameObject, selfStats);
        }
    }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default Behaviour Action", menuName = "Behaviours/Default")]
public class DefaultBehaviour : BehaviourFunction
{

    private static System.Random rnd = new System.Random();

    public override void doActions(float deltaTime, GameObject self, MobStats selfStats, MobSceneData sceneData)
    {
        Mob mob = self.GetComponent<Mob>();

        if ( mob.target == null)
        {
            chooseTarget(mob, selfStats, sceneData);
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


    private void chooseTarget(Mob self, MobStats selfStats, MobSceneData sceneData)
    {
        if (chance(0.05f))
        {
            // select some player
            self.target = randomFrom(Supervisor.currentSceneState.players);
        } else
        {
            // select enemy
            if (selfStats.team == 1)
            {
                self.target = randomFrom(Supervisor.currentSceneState.blueTeam);
            } else
            {
                self.target = randomFrom(Supervisor.currentSceneState.redTeam);
            }
        }
    }

    private static GameObject randomFrom(List<Mob> lst)
    {
        if (lst.Count > 0)
            return lst[rnd.Next(lst.Count)].gameObject;
        else return null;
    }

    private static GameObject randomFrom(List<Player> lst)
    {
        if (lst.Count > 0)
            return lst[rnd.Next(lst.Count)].gameObject;
        else return null;
    }

    private static bool chance(float possibility)
    {
        return Random.Range(0f, 1f) < possibility;
    }
}

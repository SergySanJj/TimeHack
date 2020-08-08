using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Default Behaviour Action", menuName = "Behaviours/Default")]
public class DefaultBehaviour : BehaviourFunction
{


    public override void doActions(float deltaTime, GameObject self, MobStats selfStats, MobSceneData sceneData)
    {
        Vector3 playerPos = sceneData.players[0].transform.position;
        Vector3 dir = playerPos - self.transform.position;
        dir = Vector3.Normalize(dir);
        self.transform.position += dir*selfStats.moveSpeed*deltaTime;
    }

}

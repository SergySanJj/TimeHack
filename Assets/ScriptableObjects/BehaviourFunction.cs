using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourFunction: ScriptableObject 
{ 
    public abstract void doActions(float deltaTime, GameObject self, MobStats selfStats, MobSceneData sceneData);
}

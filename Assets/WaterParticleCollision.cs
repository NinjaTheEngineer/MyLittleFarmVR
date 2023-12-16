using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticleCollision : NinjaMonoBehaviour
{
    public ParticleSystem particleSystem;
    public List<ParticleCollisionEvent> collisionEvents;
    public int minCollisionsToWet = 30;

    private void Start() {
        //particleSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other) {
        var logId = "OnParticleCollision";
        var farmingBlock = other.GetComponent<FarmingBlock>();
        if(farmingBlock==null || !farmingBlock.IsPlanted) {
            return;
        }
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        if(numCollisionEvents > minCollisionsToWet) {
            farmingBlock.ProgressMeter.Increment();
        }
        logd(logId, "Collided with " + other.logf() + ", " + numCollisionEvents + " times");
    }
}

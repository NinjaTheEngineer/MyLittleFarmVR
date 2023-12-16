using NinjaTools;
using UnityEngine;

public class Free : BaseState<SeedState> {
    Seed _seed;
    float collisionTime;
    GameObject lastCollidedObj;
    bool isSticking = false;
    public Free(SeedState key, Seed seed) : base(key) {
        var logId = "Free_ctor";
        _seed = seed;
        Utils.logd(logId, "Initialized!");
    }

    public override void EnterState() {
        var logId = "EnterState";
        isSticking = false;
        collisionTime = Time.realtimeSinceStartup;
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        if (Time.realtimeSinceStartup - collisionTime > _seed.DisappearDelay) {
            return SeedState.Disabled;
        }
        if (isSticking) {
            return SeedState.Sticking;
        }
        return SeedState.Free;
    }
    public override void UpdateState() { }

    public override void OnCollisionEnter(Collision collision) {
        var logId = "OnTriggerEnter";
        collisionTime = Time.realtimeSinceStartup;
        if (lastCollidedObj == collision.gameObject) {
            Utils.logd(logId, "Collided with same object=" + lastCollidedObj.logf() + " => returning");
            return;
        }
        lastCollidedObj = collision.gameObject;
        Utils.logd(logId, "Collision with " + lastCollidedObj.name);
        var farmingBlock = lastCollidedObj.GetComponent<FarmingBlock>() ?? lastCollidedObj.GetComponentInParent<FarmingBlock>();
        if (farmingBlock == null || farmingBlock.IsPlanted) {
            Utils.logd(logId, "FarmingBlock=" + farmingBlock.logf() + " => returning");
            return;
        }
        _seed.SetFarmingBlock(farmingBlock);
        isSticking = true;
    }

    public override void OnCollisionExit(Collision collision) { }

    public override void OnCollisionStay(Collision collision) { }

    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }
}

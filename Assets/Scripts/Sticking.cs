using NinjaTools;
using UnityEngine;

public class Sticking : BaseState<SeedState> {
    Seed _seed;
    float stickingStartTime;
    bool isSticking;
    public Sticking(SeedState key, Seed seed) : base(key) {
        var logId = "Sticking_ctor";
        _seed = seed;
        Utils.logd(logId, "Initialized!");
    }

    public override void EnterState() {
        var logId = "EnterState";
        isSticking = true;
        stickingStartTime = Time.realtimeSinceStartup;
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        if (!isSticking) {
            return SeedState.Free;
        }
        if (Time.realtimeSinceStartup - stickingStartTime > _seed.StickingDelay) {
            return SeedState.Sticked;
        }
        return SeedState.Sticking;
    }


    public override void UpdateState() { }

    public override void OnCollisionEnter(Collision collision) {
        var logId = "OnCollisionEnter";
        stickingStartTime = Time.realtimeSinceStartup;
        var collidedObj = collision.gameObject;
        Utils.logd(logId, "Collision with " + collidedObj.name);
        var farmingBlock = collidedObj.GetComponent<FarmingBlock>() ?? collidedObj.GetComponentInParent<FarmingBlock>();
        if (farmingBlock == null || farmingBlock.IsPlanted) {
            Utils.logd(logId, "FarmingBlock=" + farmingBlock.logf() + " => returning");
            isSticking = false;
        }
    }

    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
    public override void OnTriggerEnter(Collider collision) { }
    public override void OnTriggerExit(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
}

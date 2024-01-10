using NinjaTools;
using UnityEngine;

public class Disabled : BaseState<SeedState> {
    Seed _seed;
    public Disabled(SeedState key, Seed seed) : base(key) {
        var logId = "Disabled_ctor";
        _seed = seed;
        Utils.logd(logId, "Initialized!");
    }

    public override void EnterState() {
        var logId = "EnterState";
        _seed.DestroySelf();
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        return SeedState.Disabled;
    }

    public override void OnTriggerEnter(Collider other) {
    }

    public override void OnTriggerExit(Collider other) {
    }

    public override void OnTriggerStay(Collider other) {
    }

    public override void UpdateState() {
    }

    public override void OnCollisionEnter(Collision collision) {
    }

    public override void OnCollisionStay(Collision collision) {
    }

    public override void OnCollisionExit(Collision collision) {
    }
}
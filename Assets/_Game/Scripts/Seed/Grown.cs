 using NinjaTools;
using UnityEngine;

public class Grown : BaseState<SeedState> {
    Seed _seed;
    public Grown(SeedState key, Seed seed) : base(key) {
        var logId = "Grown_ctor";
        _seed = seed;
    }
    public override void EnterState() {
        AudioManager.Instance.PlaySFX(SoundType.CropReady, _seed.transform.position);
        _seed.VRGrabbable.SetupColliders();
        var logId = "EnterState";
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        return SeedState.Grown;
    }


    public override void UpdateState() { }

    public override void OnCollisionEnter(Collision collision) {
        var logId = "OnCollisionEnter";
    }

    public override void OnCollisionStay(Collision collision) { }
    public override void OnCollisionExit(Collision collision) { }
    public override void OnTriggerEnter(Collider collision) { }
    public override void OnTriggerExit(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
}


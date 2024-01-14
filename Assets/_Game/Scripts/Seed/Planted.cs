using NinjaTools;
using UnityEngine;

public class Planted : BaseState<SeedState> {
    Seed _seed;
    public Planted(SeedState key, Seed seed) : base(key) {
        var logId = "Planted_ctor";
        _seed = seed;
    }
    GameObject cropObj;
    float stateInitTime;
    float growDelay;
    public override void EnterState() {
        var logId = "EnterState";
        growDelay = _seed.GrowDelay;
        stateInitTime = Time.realtimeSinceStartup;
        cropObj = GameObject.Instantiate(_seed.SeedConfig.cropPrefab, _seed.transform.position, Quaternion.identity);
        cropObj.transform.parent = _seed.transform;
        _seed.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        if(Time.realtimeSinceStartup - stateInitTime > growDelay) {
            return SeedState.Growing;
        }
        return SeedState.Planted;
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

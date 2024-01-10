using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NinjaTools;
using UnityEngine;
using UnityEngine.XR.OpenXR;

public class Sticking : BaseState<SeedState> {
    Seed _seed;
    float stickingStartTime;
    bool isSticking;
    public Sticking(SeedState key, Seed seed) : base(key) {
        var logId = "Sticking_ctor";
        _seed = seed;
    }
    TweenerCore<Vector3, Vector3, VectorOptions> moveDownDo;
    public override void EnterState() {
        var logId = "EnterState";
        _seed.SetKinematic(true);
        isSticking = true;
        moveDownDo = _seed.transform.DOMoveY(_seed.transform.position.y - _seed.SeedConfig.initialYOffset, _seed.StickingDelay);
        stickingStartTime = Time.realtimeSinceStartup;
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        if (!isSticking) {
            moveDownDo.Kill();
            _seed.SetKinematic(false);
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
        
        if (collidedObj.tag==Constants.TerrainTag) {
            return;
        }
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


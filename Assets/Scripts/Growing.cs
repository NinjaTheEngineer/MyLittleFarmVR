using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NinjaTools;
using UnityEngine;

public class Growing : BaseState<SeedState> {
    Seed _seed;
    bool grown = false;
    public Growing(SeedState key, Seed seed) : base(key) {
        var logId = "Growing_ctor";
        _seed = seed;
    }
    TweenerCore<Vector3, Vector3, VectorOptions> scaleUpDo;
    public override void EnterState() {
        var logId = "EnterState";
        scaleUpDo = _seed.transform.DOScale(1, _seed.SeedConfig.growthTime)
            .SetEase(_seed.SeedConfig.growScaleEase)
            .OnComplete(() => grown = true);
    }

    public override void ExitState() {
        var logId = "ExitState";
    }

    public override SeedState GetNextState() {
        var logId = "GetNextState";
        if (grown) {
            return SeedState.Grown;
        }
        return SeedState.Growing;
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


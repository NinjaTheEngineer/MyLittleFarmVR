using NinjaTools;
using System;
using System.Threading.Tasks;
using UnityEngine;

[SelectionBase]
public class Seed : NinjaMonoBehaviour {
    [SerializeField] Rigidbody rb;
    public int stickingDelayInMs = 2000;
    public int disappearDelayInMs = 2000;
    [SerializeField] FarmingBlock farmingBlock;
    private async void OnCollisionEnter(Collision collision) {
        var logId = "OnCollisionEnter";
        var collisionObj = collision.gameObject;
        logd(logId, "Collision with "+collisionObj.name);
        var farmingBlock = collisionObj.GetComponent<FarmingBlock>() ?? collisionObj.GetComponentInParent<FarmingBlock>();
        if(farmingBlock==null || farmingBlock.IsPlanted) {
            logw(logId, "FarmingBlock="+farmingBlock.logf()+" => returning");
            await Disappear();
            return;
        }
        //if can't add seed return or await 'x' to go back to objectPool
        farmingBlock.ProgressMeter.Increment();
        await StickingToTheGround();
    }
    async Task Disappear() {
        await Task.Delay(disappearDelayInMs);
        Destroy(gameObject);
    }
    async Task StickingToTheGround() {
        var logId = "StickingToTheGround";
        await Task.Delay(stickingDelayInMs);
        rb.isKinematic = true;
    }
}
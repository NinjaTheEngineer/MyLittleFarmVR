using HurricaneVR.Framework.Core.Grabbers;
using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellBox : NinjaMonoBehaviour {
    public Transform sellStation;
    public List<HVRSocket> sockets;
    public float sellDelay = 3f;
    private void Start() {
        StartCoroutine(HandleBoxSalesRoutine());
    }
    //make a coroutine method that runs every 1 second and checks if the box is near the sellStation
    //if it is, it should get all sockets and see if they are filled with a seed
    //if they are, it should get the seed and add it to the sellStation
    //then it should remove the seed from the socket
    //then it should add the money to the player
    public IEnumerator HandleBoxSalesRoutine() {
        var logId = "HandleBoxSalesRoutine";
        bool soldStuff = false;
        var waitForSeconds = new WaitForSeconds(sellDelay);
        while (true) {
            yield return waitForSeconds;
            var distance = Vector3.Distance(transform.position, sellStation.position);
            soldStuff = false;
            if (distance < 1f) {
                foreach (var socket in sockets) {
                    var socketItem = socket.LinkedGrabbable;
                    if(socketItem==null) {
                        continue;
                    }
                    var seed = socketItem.GetComponent<Seed>();
                    if (seed == null) {
                        logd(logId, "No seed found in socket="+socketItem.logf());
                        continue;
                    }
                    GoldSystem.Instance.AddGold(seed.SeedConfig.price);
                    soldStuff = true;
                    Destroy(socketItem);
                }
                if(soldStuff) {
                    AudioManager.Instance.PlaySFX(SoundType.PurchaseItem, sellStation.position);
                }
            }
        }
    }
}

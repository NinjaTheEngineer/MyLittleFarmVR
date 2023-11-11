using NinjaTools;
using HurricaneVR.Framework.Core.Grabbers;

public class HVRMediator : NinjaMonoBehaviour {

    public HVRHandGrabber leftHandGrabber;
    public HVRHandGrabber rightHandGrabber;

    public static HVRMediator Instance;
    public void OnAwake() {
        if(Instance==null) {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    void OnEnable() {
        BlockPlacer.OnPlacementStart += DisableForceGrabbers;
        BlockPlacer.OnBlockPlaced += EnableForceGrabbers;
    }
    void OnDisable() {
        BlockPlacer.OnPlacementStart -= DisableForceGrabbers;
        BlockPlacer.OnBlockPlaced -= EnableForceGrabbers;
    }
    public void EnableForceGrabbers() {
        var logId = "EnableForceGrabbers";
        logd(logId, "Enabling Force Grabbers");
        leftHandGrabber.EnableForceGrabber();
        rightHandGrabber.EnableForceGrabber();
    }
    public void DisableForceGrabbers() {
        var logId = "DisableForceGrabbers";
        logd(logId, "Disabling Force Grabbers");
        leftHandGrabber.DisableForceGrabber();
        rightHandGrabber.DisableForceGrabber();

    }
}
using NinjaTools;
using System.Threading.Tasks;
using UnityEngine;

public enum SeedState {
    Free, Disabled, Sticking, Sticked, Planted, Growing, Grown
}

[SelectionBase]
public class Seed : StateManager<SeedState> {
    [SerializeField] Rigidbody rb;
    [field: SerializeField] public float StickingDelay { get; private set; }
    [field: SerializeField] public float DisappearDelay { get; private set; }
    [field: SerializeField] public float GrowDelay { get; private set; }
    [field: SerializeField] public SeedConfig SeedConfig { get; private set; }
    
    [SerializeField] FarmingBlock farmingBlock;
    [SerializeField] SeedState currentState;
    [SerializeField] GameObject visu;
    
    private void Awake() {
        States.Add(SeedState.Free, new Free(SeedState.Free, this));
        States.Add(SeedState.Disabled, new Disabled(SeedState.Disabled, this));
        States.Add(SeedState.Sticking, new Sticking(SeedState.Sticking, this));
        States.Add(SeedState.Sticked, new Sticked(SeedState.Sticked, this));
        States.Add(SeedState.Planted, new Planted(SeedState.Planted, this));
        States.Add(SeedState.Growing, new Growing(SeedState.Growing, this));
        States.Add(SeedState.Grown, new Grown(SeedState.Grown, this));
        currentState = SeedState.Free;
        CurrentState = States[currentState];
    }
    private void Start() {
        var logId = "Start";
        GrowDelay = Random.Range(1, SeedConfig.maxGrowDelay);
        logd(logId, "GrowDelay="+GrowDelay);
    }
    public void DestroySelf() {
        Destroy(gameObject);
    }
    public void SetFarmingBlock(FarmingBlock farmingBlock) {
        this.farmingBlock = farmingBlock;
    }
    public void StickToGround() {
        rb.isKinematic = true;
        farmingBlock.OnPlanted += FarmBlockPlanted;
        farmingBlock.ProgressMeter.Increment();
    }

    void FarmBlockPlanted() {
        farmingBlock.OnPlanted -= FarmBlockPlanted;
        HideSeed();
        TransitionToState(SeedState.Planted);
    }
    
    public override void OnStateChange(SeedState state) {
        var logId = "OnStateChange";
        currentState = state;
    }
    void HideSeed() {
        visu.SetActive(false);
    }

    public void SetKinematic(bool isKinematic) => rb.isKinematic = isKinematic;
}
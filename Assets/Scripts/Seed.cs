using NinjaTools;
using System;
using System.Threading.Tasks;
using UnityEngine;

public enum SeedState {
    Free, Disabled, Sticking, Sticked, Planted, Growing, Grown
}
[CreateAssetMenu(fileName ="SeedConfig", menuName ="ScriptableObjects/Create New Seed Config")]
public class SeedConfig : ScriptableObject {
    public GameObject cropPrefab;
    public float initialY = -0.5f;
    public float maxY = 1f;
    public float growthTime = 90f;
}

[SelectionBase]
public class Seed : StateManager<SeedState> {
    [SerializeField] Rigidbody rb;
    [field: SerializeField] public float StickingDelay { get; private set; }
    [field: SerializeField] public float DisappearDelay { get; private set; }
    [field: SerializeField] public SeedConfig SeedConfig { get; private set; }
    
    [SerializeField] FarmingBlock farmingBlock;
    [SerializeField] SeedState _CurrentState;
    
    private void Awake() {
        States.Add(SeedState.Free, new Free(SeedState.Free, this));
        States.Add(SeedState.Disabled, new Disabled(SeedState.Disabled, this));
        States.Add(SeedState.Sticking, new Sticking(SeedState.Sticking, this));
        States.Add(SeedState.Sticked, new Sticked(SeedState.Sticked, this));
        States.Add(SeedState.Planted, new Planted(SeedState.Planted, this));
        _CurrentState = SeedState.Free;
        CurrentState = States[_CurrentState];
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
        TransitionToState(SeedState.Planted);
    }
    
    public override void OnStateChange(SeedState state) {
        var logId = "OnStateChange";
        _CurrentState = state;
    }
}
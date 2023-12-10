using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FarmingBlockState {
    Placed, Worked, Wet, Planted, PlantedWet, ReadyToHarvest
}
[SelectionBase]
public class FarmingBlock : NinjaMonoBehaviour {
    public FarmingBlockState initialState;
    public FarmingBlockState CurrentState { get; private set; }
    public bool IsPlanted => CurrentState == FarmingBlockState.Planted || CurrentState == FarmingBlockState.PlantedWet;
    public bool IsWorked => CurrentState == FarmingBlockState.Worked;
    [field: SerializeField] public ProgressMeter ProgressMeter { get; private set; }
    public Mesh CurrentMesh { get; private set; }
    [SerializeField] Mesh workedGroundMesh;
    public float Width { get; private set; }
    public float Length { get; private set; }
    MeshFilter meshFilter;
    [SerializeField] GameObject visu;
    private void Awake() {
        meshFilter = visu.GetComponent<MeshFilter>();
        CurrentMesh = meshFilter.mesh;
        Width = visu.transform.localScale.x;
        Length = visu.transform.localScale.z;
    }
    private void Start() {
        CurrentState = initialState;
        if (IsWorked) {
            SetWorkedGround();
        }
    }
    void OnEnable() {
        ProgressMeter.OnComplete += SetWorkedGround;
    }
    public int maxSeeds = 10;
    public int numOfSeeds = 0;
    public void SetWorkedGround() {
        var logId = "SetWorkedGround";
        ChangeState(FarmingBlockState.Worked);
        ProgressMeter.OnComplete -= SetWorkedGround;
        logd(logId, "Setting Mesh to WorkedGround");
        ProgressMeter.SetProgressMeter(maxSeeds, numOfSeeds, 1);
        ProgressMeter.OnComplete += FullOfSeeds;
        meshFilter.mesh = workedGroundMesh;
        CurrentMesh = workedGroundMesh;
    }
    public void FullOfSeeds() {
        var logId = "FullOfSeeds";
        logd(logId, "Ground full of seeds!");
        ChangeState(FarmingBlockState.Planted);
    }
    public void ChangeState(FarmingBlockState newState) {
        var logId = "ChangeState";
        if (CurrentState == newState) {
            logw(logId, "Tried to set CurrentState to same state=" + newState, highlightGO: gameObject);
        }
        logd(logId, "Changing State from " + CurrentState + " to " + newState, highlightGO: gameObject);
        CurrentState = newState;
    }
    public override string ToString() => "FarmingBlock: CurrentState=" + CurrentState + " ProgressMeter=" + ProgressMeter.logf();
}

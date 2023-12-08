using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FarmingBlockState {
    Placed, Worked, Wet, Planted, PlantedWet, ReadyToHarvest
}
public class FarmingBlock : NinjaMonoBehaviour {
    public FarmingBlockState initialState;
    public FarmingBlockState CurrentState { get; private set; }
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
    }
    void OnEnable() {
        ProgressMeter.OnComplete += SetWorkedGround;
    }
    public void SetWorkedGround() {
        var logId = "SetWorkedGround";
        ChangeState(FarmingBlockState.Worked);
        ProgressMeter.OnComplete -= SetWorkedGround;
        logd(logId, "Setting Mesh to WorkedGround");
        meshFilter.mesh = workedGroundMesh;
        CurrentMesh = workedGroundMesh;
    }
    public bool IsWorked => CurrentState == FarmingBlockState.Worked;
    public void ChangeState(FarmingBlockState newState) {
        var logId = "ChangeState";
        if(CurrentState==newState) {
            logw(logId, "Tried to set CurrentState to same state=" + newState, highlightGO: gameObject);
        }
        logd(logId, "Changing State from " + CurrentState + " to " + newState, highlightGO: gameObject);
        CurrentState = newState;
    }
}

using NinjaTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FarmingBlockState {
    Placed, Worked, Planted, Wet, ReadyToHarvest
}
[SelectionBase]
public class FarmingBlock : NinjaMonoBehaviour {
    public FarmingBlockState initialState;
    [field: SerializeField] public FarmingBlockState CurrentState { get; private set; }
    public bool IsPlanted => CurrentState == FarmingBlockState.Planted;
    public bool IsWorked => CurrentState == FarmingBlockState.Worked;
    [field: SerializeField] public ProgressMeter ProgressMeter { get; private set; }
    public Mesh CurrentMesh { get; private set; }
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] Mesh workedGroundMesh;
    [SerializeField] Color wetColor;
    Color defaultColor;
    public float Width { get; private set; }
    public float Length { get; private set; }

    public Action OnPlanted;
    public Action OnWet;
    MeshFilter meshFilter;
    [SerializeField] GameObject visu;
    private void Awake() {
        meshFilter = visu.GetComponent<MeshFilter>();
        CurrentMesh = meshFilter.mesh;
        defaultColor = meshRenderer.material.color;
        Width = visu.transform.localScale.x;
        Length = visu.transform.localScale.z;
    }
    private void Start() {
        CurrentState = initialState;
        if (IsWorked) {
            SetWorkedGround();
        } 
        else if (CurrentState == FarmingBlockState.Placed) {
            ProgressMeter.OnComplete += SetWorkedGround;
        }
    }
    public int maxSeeds = 5;
    public void SetWorkedGround() {
        var logId = "SetWorkedGround";
        ChangeState(FarmingBlockState.Worked);

        meshRenderer.material.color = defaultColor;
        cropsHarvest = 0;
        ProgressMeter.OnComplete -= SetWorkedGround;
        ProgressMeter.SetProgressMeter(maxSeeds, 0, 1);
        ProgressMeter.OnComplete += SetPlantedGround;

        logd(logId, "Setting Mesh to WorkedGround");
        meshFilter.mesh = workedGroundMesh;
        meshCollider.sharedMesh = workedGroundMesh;
        CurrentMesh = workedGroundMesh;
    }
    [SerializeField] float groundWetAmount = 100;
    [SerializeField] float waterFillRate = 1;
    public void SetPlantedGround() {
        var logId = "SetPlantedGround";
        ChangeState(FarmingBlockState.Planted);

        ProgressMeter.OnComplete -= SetPlantedGround;
        ProgressMeter.SetProgressMeter(groundWetAmount, 0, waterFillRate);
        ProgressMeter.OnComplete += SetWetGround;
        OnPlanted?.Invoke();
    }
    public void SetWetGround() {
        ChangeState(FarmingBlockState.Wet);
        meshRenderer.material.color = wetColor;
        ProgressMeter.OnComplete -= SetWetGround;
        OnWet?.Invoke();
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
    int cropsHarvest = 0;
    void OnCropHarvest(Seed seed) {
        cropsHarvest++;
        seed.OnHarvest -= OnCropHarvest;
        if(cropsHarvest==maxSeeds) {
            SetWorkedGround();
        }
    }
    public void AddSeed(Seed seed) {
        ProgressMeter.Increment();
        seed.OnHarvest += OnCropHarvest;
    }
}

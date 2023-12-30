using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.XR;
using TMPro;
using System.Net;
using System.Threading.Tasks;

public class BlockPlacer : NinjaMonoBehaviour {
    public GameObject blockPrefab;
    public PreviewBlock previewPrefab;
    public LayerMask obstacleLayers;
    public LayerMask layersToHit;
    PreviewBlock previewBlock;
    bool _isPlacingBlock = false;

    public static Action OnPlacementStart;
    public static Action OnBlockPlaced;
    public bool IsPlacingBlock {
        get => _isPlacingBlock;
        private set {
            var logId = "IsPlacingBlock_set";
            logd(logId, "Setting IsPlacingBlock from " + _isPlacingBlock + " to " + value);
            _isPlacingBlock = value;
            if (_isPlacingBlock) {
                lineRenderer.positionCount = numOfCurvePoints;
                OnPlacementStart?.Invoke();
            } else {
                lineRenderer.positionCount = 0;
            }
            
            previewBlock.Show(_isPlacingBlock);
        }
    }
    public float handlePreviewStateRate = 0.05f;
    public Vector3 blockSize = new Vector3(2f, 1f, 2f);

    [Header("Gizmos")]
    public float rayDistance = 5f;
    public float nearbyRadius = 0.5f;
    public float xDifference = 2f;
    public float zDifference = 2f;
    public Vector3 lastRayHitPos;
    RaycastHit raycastHit;
    private void OnDrawGizmos() {
        if (!Application.isPlaying && !isActiveAndEnabled) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(RightHandOrigin.position, RightHandOrigin.forward * rayDistance);
        Gizmos.DrawWireSphere(lastRayHitPos, nearbyRadius);
        if (previewBlock) {
            Gizmos.DrawWireCube(previewBlock.transform.position, blockSize);
        }
    }
    private void Start() {
        previewBlock = Instantiate(previewPrefab);
        previewBlock.Show(false);
    }
    public LineRenderer lineRenderer;
    bool activeButtonLastFrame = false;
    public float pointOneMultiplier = 2f;
    public float pointTwoMultiplier = 2f;
    private void Update() {
        if (IsPlacingBlock && Physics.Raycast(RightHandOrigin.position, RightHandOrigin.forward * rayDistance, out raycastHit, 10, layersToHit)) {
            Debug.DrawRay(RightHandOrigin.position, RightHandOrigin.forward * rayLength, Color.red);
            GenerateCurve(RightHandOrigin.position, raycastHit.point);
            HandlePreviewPosition();
        }
        var buttonActive = HVRGlobalInputs.Instance.RightPrimaryButtonState.Active;
        if (!activeButtonLastFrame && buttonActive) {
            TogglePlacement();
        }
        activeButtonLastFrame = buttonActive;

    }
    public float curveHeight = 0.5f;
    void GenerateCurve(Vector3 startPos, Vector3 endPos) {
        lineRenderer.positionCount = numOfCurvePoints; // Adjust the number of points as needed

        Vector3[] positions = new Vector3[numOfCurvePoints];

        for (int i = 0; i < numOfCurvePoints; i++) {
            float t = i / (float)(numOfCurvePoints - 1);
            positions[i] = CalculateBezierPoint(t, startPos, endPos, curveHeight);
        }

        lineRenderer.SetPositions(positions);
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, float height) {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * p0
        p += 3 * uu * t * (p0 + Vector3.down * height); // 3 * (1-t)^2 * t * (p0 + height * down vector)
        p += 3 * u * tt * (p1 + Vector3.down * height); // 3 * (1-t) * t^2 * (p1 + height * down vector)
        p += ttt * p1; // t^3 * p1

        return p;
    }
    public void TogglePlacement() {
        var logId = "TogglePlacement";
        logd(logId, "TogglePlacement to "+_isPlacingBlock);
        IsPlacingBlock = !_isPlacingBlock;
    }
    public Transform RightHandOrigin;
    public float rayLength = 10f;
    public void PlaceBlock() {
        var logId = "PlaceBlock";
        if (IsPlacingBlock && previewBlock.InValidPosition) {
            IsPlacingBlock = false;
            Instantiate(blockPrefab, previewBlock.transform.position, Quaternion.identity);
            logd(logId, "Placing Block!");
            OnBlockPlaced?.Invoke();
            HVRControllerEvents.Instance.RightTriggerActivated.RemoveListener(PlaceBlock);
            HVRControllerEvents.Instance.LeftTriggerActivated.RemoveListener(PlaceBlock);
        } else {
            logw(logId, "Could not place block.");
        }

    }
    public async void StartPlacement(bool isLeftHand) {
        IsPlacingBlock = true;
        await Task.Delay(1000);
        if(isLeftHand) {
            HVRControllerEvents.Instance.RightTriggerActivated.AddListener(PlaceBlock);
        } else {
            HVRControllerEvents.Instance.LeftTriggerActivated.AddListener(PlaceBlock);
        }
    }

    private void HandlePreviewPosition() {
        var logId = "HandlePreviewPosition";
        var previewPosition = raycastHit.point;
        var hits = Physics.BoxCastAll(previewPosition, blockSize, Vector3.up);
        List<FarmingBlock> nearbyBlocks = new List<FarmingBlock>();

        foreach (var hit in hits) {
            var block = hit.collider.GetComponentInParent<FarmingBlock>();
            if (block) {
                nearbyBlocks.Add(block);
            }
        }

        if (nearbyBlocks.Count > 0) {
            var nearbyBlock = GetNearestBlock(previewPosition, nearbyBlocks);
            var direction = (nearbyBlock.transform.position - previewPosition).normalized;

            previewPosition = CalculateFinalPosition(nearbyBlock, direction);
            logd(logId, "RayCast=" + raycastHit.collider.name + " => Direction=" + direction, true, 1f);
        }
        previewBlock.transform.position = previewPosition;
        logd(logId, "RayCast=" + raycastHit.collider.name, true, 1f);
    }
    public int numOfCurvePoints = 50;
    private FarmingBlock GetNearestBlock(Vector3 raycastPoint, List<FarmingBlock> nearbyBlocks) {
        var logId = "GetNearestBlock";
        var nearbyBlocksCount = nearbyBlocks.Count;
        if (nearbyBlocksCount == 0) {
            logw(logId, "No blocks found => returning null", delay: 10f);
            return null;
        }
        FarmingBlock nearestBlock = nearbyBlocks[0];
        if (nearbyBlocksCount == 1) {
            return nearestBlock;
        }
        var distanceFromNearestBlock = Vector3.Distance(raycastPoint, nearestBlock.transform.position);
        for (int i = 1; i < nearbyBlocksCount; i++) {
            var currentBlock = nearbyBlocks[i];
            var distanceFromCurrentBlock = Vector3.Distance(raycastPoint, currentBlock.transform.position);
            if (distanceFromCurrentBlock < distanceFromNearestBlock) {
                distanceFromNearestBlock = distanceFromCurrentBlock;
                nearestBlock = currentBlock;
            }
        }
        return nearestBlock;
    }

    Vector3 CalculateFinalPosition(FarmingBlock nearbyBlock, Vector3 direction) {
        var logId = "GetFinalPosition";
        var dirX = direction.x;
        var dirZ = direction.z;
        var blockPos = nearbyBlock.transform.position;
        bool changedX = true;
        bool changedY = true;
        if (dirX > 0.85f) {
            blockPos.x -= xDifference;
        } else if (dirX < -0.85f) {
            blockPos.x += xDifference;
        } else if (dirZ < 0.8f || dirZ > -0.8f) {
            if (dirX > 0.6f) {
                blockPos.x -= xDifference;
            } else if (dirX < -0.6f) {
                blockPos.x += xDifference;
            }
        } else {
            changedX = false;
        }

        if (dirZ > 0.85f) {
            blockPos.z -= zDifference;
        } else if (dirZ < -0.85f) {
            blockPos.z += zDifference;
        } else if (dirX < 0.8f || dirX > -0.8f) {
            if (dirZ > 0.6f) {
                blockPos.z -= zDifference;
            } else if (dirZ < -0.6f) {
                blockPos.z += zDifference;
            }
        } else {
            changedY = false;
        }
        bool anyChange = changedX || changedY;
        return anyChange ? blockPos : raycastHit.point;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NinjaTools;
using System;
using System.Security.Cryptography;
using UnityEngine.UIElements;

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
            if (_isPlacingBlock == value) {
                return;
            }
            previewBlock.SetActivePreview(value);
            _isPlacingBlock = value;
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
        var cam = Camera.main.transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cam.position, cam.forward * rayDistance);
        Gizmos.DrawWireSphere(lastRayHitPos, nearbyRadius);

        if (previewBlock) {
            Gizmos.DrawWireCube(previewBlock.transform.position, blockSize);
        }
    }
    Camera MainCamera;
    private void Start() {
        previewBlock = Instantiate(previewPrefab);
        previewBlock.SetActivePreview(false);
        MainCamera = Camera.main;
        IsPlacingBlock = true;
    }
    public LineRenderer lineRenderer;
    private void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1) && !IsPlacingBlock) {
            IsPlacingBlock = true;
        }*/
        Debug.DrawRay(RightHandOrigin.position, RightHandOrigin.forward * rayLength, Color.red);
        if (IsPlacingBlock && Physics.Raycast(RightHandOrigin.position, RightHandOrigin.forward * rayDistance, out raycastHit, 10, layersToHit)) {
            lineRenderer.SetPositions(new Vector3[] { RightHandOrigin.position, raycastHit.point });
            HandlePreviewPosition();
        }
    }
    public void StartPlacingBlock() {
        if (!IsPlacingBlock) {
            IsPlacingBlock = true;
            OnPlacementStart?.Invoke();
        }
    }
    public Transform RightHandOrigin;
    public float rayLength = 10f;
    public void OnRightTriggerPressed() {
        if (IsPlacingBlock && previewBlock.CanBePlaced) {
            PlaceBlock();
        }
    }
    public void OnRightTriggerReleased() {
        if (IsPlacingBlock && previewBlock.CanBePlaced) {
            PlaceBlock();
        }
    }
    void PlaceBlock() {
        IsPlacingBlock = false;
        Instantiate(blockPrefab, previewBlock.transform.position, Quaternion.identity);
        OnBlockPlaced?.Invoke();
    }
    private void HandlePreviewPosition() {
        var logId = "HandlePreviewPosition";
        var previewPosition = raycastHit.point;
        var hits = Physics.BoxCastAll(previewPosition, blockSize, Vector3.up);
        List<GroundBlock> nearbyBlocks = new List<GroundBlock>();

        foreach (var hit in hits) {
            var block = hit.collider.GetComponentInParent<GroundBlock>();
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
        //previewBlock.CanBePlaced = IsPositionValid(previewPosition, blockSize);
        logd(logId, "RayCast=" + raycastHit.collider.name, true, 1f);
    }

    private GroundBlock GetNearestBlock(Vector3 raycastPoint, List<GroundBlock> nearbyBlocks) {
        var logId = "GetNearestBlock";
        var nearbyBlocksCount = nearbyBlocks.Count;
        if (nearbyBlocksCount == 0) {
            logw(logId, "No blocks found => returning null");
            return null;
        }
        GroundBlock nearestBlock = nearbyBlocks[0];
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

    Vector3 CalculateFinalPosition(GroundBlock nearbyBlock, Vector3 direction) {
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

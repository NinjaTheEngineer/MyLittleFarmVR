using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class HeightMapPainter : NinjaMonoBehaviour {
    [Range(0.05f, 3f)]
    [SerializeField] float paintDelay = 0.1f;
    [Range(0.1f, 5f)]
    [SerializeField] float paintIntensity = 2f;
    [Range(0.05f, 1f)]
    [SerializeField] float minY = 0.3f;
    [Range(0.05f, 1f)]
    [SerializeField] float loweringIntensity = 0.3f;
    [Range(0.05f, 1f)]
    [SerializeField] float raisingIntensity = 0.12f;
    [Range(0.1f, 5f)]
    [SerializeField] float raisingRadius = 0.15f;
    float lastPaintTime;
    private bool _isDrawing = false;
    public bool IsDrawing {
        get => _isDrawing;
        private set {
            var logId = "IsDrawing_set";
            logd(logId, "Setting _isDrawing from " + _isDrawing + " to " + value);
            _isDrawing = value;
            if (_isDrawing) {
                StartCoroutine(StartPaintHeightMapRoutine());
            }
        }
    }
    IEnumerator StartPaintHeightMapRoutine() {
        var logId = "StartPaintHeightMapRoutine";
        var delay = new WaitForSeconds(paintDelay);
        while (_isDrawing) {
            logd(logId, "PaintHeightMap!");
            PaintHeightMap();
            yield return new WaitForSeconds(paintDelay);
        }
    }
    private async void OnCollisionStay(Collision collision) {
        var logId = "OnCollisionStay";
        if (Time.realtimeSinceStartup - lastPaintTime < paintDelay) {
            return;
        }
        logd(logId, "Collision Stay with " + collision.gameObject.name + " at " + collision.contacts[0], true, highlightGO: collision.gameObject);
        await DeformMesh(collision);

    }
    async Task DeformMesh(Collision collision) {
        var logId = "DeformMesh";
        var farmingBlock = collision.gameObject.GetComponentInParent<FarmingBlock>();
        if (farmingBlock == null || farmingBlock.IsWorked) {
            logw(logId, "CollisionObj="+collision.gameObject.name+ " at " + collision.contacts[0]+" FarmingBlock=" + farmingBlock.logf() + " => no-op", true, highlightGO: collision.gameObject);
            return;
        }
        var collisionContacts = collision.contacts;
        var contactsCount = collisionContacts.Length;
        var mesh = farmingBlock.CurrentMesh; ;
        var progressMeter = farmingBlock.ProgressMeter;

        if (mesh == null || progressMeter == null) {
            logw(logId, "CurrentMesh=" + mesh.logf() + " ProgressMeter=" + progressMeter.logf() + " FarmingBlock=" + farmingBlock.logf() + " => no-op");
            return;
        }

        progressMeter.Increment();

        Vector3[] vertices = mesh.vertices;
        var verticesCount = vertices.Length;

        for (int i = 0; i < contactsCount; i++) {
            ContactPoint ccp = collisionContacts[i];

            // Convert world coordinates to local coordinates
            Vector3 localPointPosition = farmingBlock.transform.InverseTransformPoint(ccp.point);

            // Find the closest vertex and set its y-coordinate to 0
            float minDistance = float.MaxValue;
            int closestVertexIndex = -1;

            for (int j = 0; j < verticesCount; j++) {
                float distance = Vector3.Distance(vertices[j], localPointPosition);

                if (distance < minDistance) {
                    minDistance = distance;
                    closestVertexIndex = j;
                }
            }

            logd(logId, "LocalPointPosition=" + localPointPosition + " ClosestVertexIndex=" + closestVertexIndex);

            if (closestVertexIndex == -1) {
                logw(logId, "No vertex found => no-op", false, 10f);
                continue;
            }

            // Lower the closest vertex
            vertices[closestVertexIndex].y -= loweringIntensity * paintIntensity;
            var closestVertice = vertices[closestVertexIndex];
            // Raise the surrounding vertices
            for (int k = 0; k < verticesCount; k++) {
                var currentVertice = vertices[k];
                if (currentVertice == closestVertice) {
                    continue;
                }
                float distanceToClosest = Vector3.Distance(currentVertice, closestVertice);

                if (distanceToClosest <= raisingRadius) // You can adjust the radius as needed
                {
                    vertices[k].y += raisingIntensity * paintIntensity;
                }
            }

            await Task.Yield();
        }
        if (mesh.vertices == vertices) {
            logw(logId, "No vertices changed => no-op");
            return;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        Debug.Log("Changed mesh for " + contactsCount + " contact points");
        lastPaintTime = Time.realtimeSinceStartup;
    }
    void PaintHeightMap() {
        var logId = "PaintHeightMap";
        var hit = new RaycastHit();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)) {
            MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.mesh;
            logd(logId, "Hit object=" + hit.collider.gameObject.name + " CurrentMesh=" + mesh.name, true);

            // Get the vertices and UVs
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;

            // Convert world coordinates to local coordinates
            Vector3 localPointPosition = hit.transform.InverseTransformPoint(hit.point);

            // Find the closest vertex and set its y-coordinate to 0
            float minDistance = float.MaxValue;
            int closestVertexIndex = -1;


            for (int i = 0; i < vertices.Length; i++) {
                float distance = Vector3.Distance(vertices[i], localPointPosition);

                if (distance < minDistance) {
                    minDistance = distance;
                    closestVertexIndex = i;
                }
            }
            logd(logId, "LocalPointPosition=" + localPointPosition + " ClosestVertexIndex=" + closestVertexIndex);
            if (closestVertexIndex == -1) {
                logw(logId, "No vertex found => no-op");
                return;
            }

            // Update the mesh with the modified vertices
            var newY = vertices[closestVertexIndex].y - (paintIntensity * 0.05f);
            newY = newY >= minY ? newY : minY;
            vertices[closestVertexIndex].y = newY;
            mesh.vertices = vertices;
            mesh.RecalculateBounds();

        }
    }
}

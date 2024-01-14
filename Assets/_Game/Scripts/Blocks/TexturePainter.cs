using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TexturePainter : NinjaMonoBehaviour {
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
    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            IsDrawing = !_isDrawing;
        }
        if (_isDrawing) {
            //PaintHeightMap();
        }
    }
    public float paintDelay = 1f;
    public float paintIntensity = 1f;
    public float minY = 0.5f;
    IEnumerator StartPaintHeightMapRoutine() {
        var logId = "StartPaintHeightMapRoutine";
        var delay = new WaitForSeconds(paintDelay);
        while (_isDrawing) {
            logd(logId, "PaintHeightMap!");
            PaintHeightMap();
            yield return new WaitForSeconds(paintDelay);
        }
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

            // Use world coordinates for painting
            Vector2 uvCoord = hit.textureCoord;

            // Update the mesh with the modified vertices
            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            var renderer = hit.collider.GetComponent<Renderer>();
            logd(logId, "Starting PaintAllTexturesRoutine for UV=" + uvCoord);
            StartCoroutine(PaintAllTexturesRoutine(uvCoord, renderer));
        }
    }
    public bool turnOfPain;
    public Dictionary<Renderer, Material> rendererMaterial = new Dictionary<Renderer, Material>();
    IEnumerator PaintAllTexturesRoutine(Vector2 uvCoord, Renderer renderer) {
        var logId = "PaintAllTexturesRoutine";
        if (turnOfPain) yield break;
        Material material;
        if (!rendererMaterial.ContainsKey(renderer)) {
            material = new Material(renderer.material);
            rendererMaterial.Add(renderer, material);
        } else {
            material = rendererMaterial[renderer];
        }

        logd(logId, "Painting UV Coord=" + uvCoord + " for material=" + material);
        // Paint on the height map
        var hasHeightMap = material.HasProperty("_ParallaxMap");
        if (hasHeightMap) {
            Texture2D heightMap = GetTexture(material, "_ParallaxMap");
            yield return StartCoroutine(PaintTextureRoutine(uvCoord, heightMap, Color.black));
            ApplyTextureToMaterial("_ParallaxMap", material, heightMap);
        }

        // Paint on the base texture
        var hasMainTex = material.HasProperty("_MainTex");
        /*
        if (hasMainTex) {
            Texture2D baseTexture = material.GetTexture("_MainTex") as Texture2D;
            yield return PaintTexture(uvCoord, baseTexture, Color.black);
            ApplyTextureToMaterial("_MainTex", material, baseTexture);
        }
        */
        // Paint on the occlusion map
        var hasOcclusionMap = material.HasProperty("_OcclusionMap");
        if (hasOcclusionMap) {
            Texture2D occlusionMap = GetTexture(material, "_OcclusionMap");
            yield return StartCoroutine(PaintTextureRoutine(uvCoord, occlusionMap, Color.black));
            ApplyTextureToMaterial("_OcclusionMap", material, occlusionMap);
        }
        renderer.material = material;
        rendererMaterial[renderer] = material;
        logd(logId, "HasHeightMap=" + hasHeightMap + " HasMainTex=" + hasMainTex + " HasOcclusionMap=" + hasOcclusionMap);
    }
    public int paintRadius = 10;
    [Range(0, 1)]
    public float maxPaintIntensity = 0.5f;
    IEnumerator PaintTextureRoutine(Vector2 uvCoord, Texture2D texture, Color color) {
        var logId = "PaintTexture";
        // Normalize the UV coordinates to be within [0, 1]
        float normalizedX = Mathf.Repeat(uvCoord.x, 1.0f);
        float normalizedY = Mathf.Repeat(uvCoord.y, 1.0f);

        // Convert normalized UV coordinates to pixel coordinates
        int centerX = Mathf.RoundToInt((texture.width - 1) * normalizedX);
        int centerY = Mathf.RoundToInt((texture.height - 1) * normalizedY);

        // Get all pixels from the texture
        Color[] pixels = texture.GetPixels();
        logw(logId, "UvCoord=" + uvCoord + " NormalizedX=" + normalizedX + " NormalizedY=" + normalizedY + " CenterX=" + centerX + " CenterY=" + centerY + " PixelsLength=" + pixels.Length + " TextureWidth=" + texture.width + " TextureHeight=" + texture.height);
        // Iterate over a square region around the center
        for (int x = centerX - paintRadius; x <= centerX + paintRadius; x++) {
            for (int y = centerY - paintRadius; y <= centerY + paintRadius; y++) {
                // Ensure the coordinates are within valid range
                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height) {
                    // Calculate distance from the center
                    float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));

                    // Adjust intensity based on distance
                    float intensity = Mathf.Clamp01(maxPaintIntensity - distance / paintRadius);

                    // Modify the pixel color
                    int index = y * texture.width + x;
                    pixels[index] = Color.Lerp(pixels[index], color, intensity);
                }
            }
        }
        yield return null;

        // Apply the changes
        texture.SetPixels(pixels);
        texture.Apply();
    }
    void ApplyTextureToMaterial(string propertyName, Material material, Texture2D texture) {
        // Apply the modified texture to the material
        material.SetTexture(propertyName, texture);
    }
    public class MaterialMask {
        Material material;
        string maskName;
        public MaterialMask(Material material, string maskName) {
            this.material = material;
            this.maskName = maskName;
        }
        public override bool Equals(object obj) {
            return GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode() {
            return material.GetHashCode() + maskName.GetHashCode();
        }
    }
    Dictionary<MaterialMask, Texture2D> materialMasks = new Dictionary<MaterialMask, Texture2D>();
    Texture2D GetTexture(Material material, string propertyName) {

        Texture2D texture = material.GetTexture(propertyName) as Texture2D;
        MaterialMask matMask = new MaterialMask(material, propertyName);
        if (!materialMasks.ContainsKey(matMask)) {
            texture = new Texture2D(texture.width, texture.height);
            materialMasks[matMask] = texture;
        } else {
            texture = materialMasks[matMask];
        }

        if (texture == null) {
            material.SetTexture(propertyName, texture);
        }

        return texture;
    }
}

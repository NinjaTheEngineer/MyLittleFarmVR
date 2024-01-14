using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ParticleSystemItem {
    public ParticleType particleType;
    public ParticleSystem particleSystemPrefab;
}

public enum ParticleType {
    PlaceBlock,
    MerchantBuyCrop,
    // ... add other particle system types here
}

public class ParticleSystemManager : NinjaMonoBehaviour {
    public static ParticleSystemManager Instance;

    [SerializeField]
    private List<ParticleSystemItem> particleSystemItems;

    private Dictionary<ParticleType, ParticleSystem> particleSystemDictionary = new Dictionary<ParticleType, ParticleSystem>();
    private List<ParticleSystem> particleSystemPool = new List<ParticleSystem>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeParticleSystemDictionary();
        } else {
            Destroy(gameObject);
        }
    }

    private void InitializeParticleSystemDictionary() {
        foreach (var item in particleSystemItems) {
            particleSystemDictionary[item.particleType] = item.particleSystemPrefab;
        }
    }

    public void PlayParticleSystem(ParticleType type, Vector3 position, bool loop = false) {
        if (particleSystemDictionary.TryGetValue(type, out ParticleSystem prefab)) {
            ParticleSystem ps = GetOrCreateParticleSystem(prefab);
            ps.transform.position = position;
            var main = ps.main;
            main.loop = loop;
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }

    private ParticleSystem GetOrCreateParticleSystem(ParticleSystem prefab) {
        foreach (var ps in particleSystemPool) {
            if ((!ps.gameObject.activeInHierarchy || !ps.isPlaying) && ps.name == prefab.name) {
                return ps;
            }
        }

        ParticleSystem newPs = Instantiate(prefab, transform);
        newPs.gameObject.SetActive(false);
        particleSystemPool.Add(newPs);
        return newPs;
    }

    private IEnumerator DisableParticleSystem(ParticleSystem ps, float delay) {
        yield return new WaitForSeconds(delay);
        ps.gameObject.SetActive(false);
    }
}
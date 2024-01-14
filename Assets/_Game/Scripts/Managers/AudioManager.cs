using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SoundItem {
    public SoundType soundType;
    public AudioClip audioClip;
}
public enum SoundType {
    ButtonHover,
    ButtonClick,
    PlaceBlock,
    GroundWork_1,
    GroundWork_2,
    Steps_1,
    Steps_2,
    CropReady,
    Harvest,
    ProgressComplete,
    CloseMenu,
    Restock,
    StoreItem,
    PurchaseItem,
    PurchaseItemFail,

    // ... add other sound types here
}
public class AudioManager : NinjaMonoBehaviour {
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource backgroundMusicSource;

    [SerializeField]
    private AudioSource sfxSourcePrefab;

    [SerializeField]
    private List<SoundItem> soundItems;

    [SerializeField]
    AudioClip backgroundMusicClip;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Dictionary<SoundType, AudioClip> soundDictionary = new Dictionary<SoundType, AudioClip>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundDictionary();
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayBackgroundMusic();
    }
    private void InitializeSoundDictionary() {
        foreach (var item in soundItems) {
            soundDictionary[item.soundType] = item.audioClip;
        }
    }
    public void BackgroundMusicVolumeUpdated() {
        backgroundMusicSource.Stop();
        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic(float volume = 1.0f, bool loop = true) {
        backgroundMusicSource.clip = backgroundMusicClip;
        backgroundMusicSource.volume = volume;
        backgroundMusicSource.loop = loop;
        backgroundMusicSource.Play();
    }

    public void PlayRandom(List<SoundType> sounds, Vector3 position, float volume = 1.0f, float spatialBlend = 1.0f) {
        //get of random sound from sounds list
        int randomIndex = Random.Range(0, sounds.Count);
        PlaySFX(sounds[randomIndex], position, volume, spatialBlend);
    }
    public void PlaySFX(SoundType type, Vector3 position, float volume = 1.0f, float spatialBlend = 1.0f) {
        if (soundDictionary.TryGetValue(type, out AudioClip sfxClip)) {
            AudioSource sfxSource = GetOrCreateSFXSource();
            sfxSource.transform.position = position;
            sfxSource.clip = sfxClip;
            sfxSource.volume = volume;
            sfxSource.spatialBlend = spatialBlend;
            sfxSource.gameObject.SetActive(true);
            sfxSource.Play();

            StartCoroutine(DisableSFXSource(sfxSource, sfxClip.length));
        }
    }

    private AudioSource GetOrCreateSFXSource() {
        foreach (var source in sfxPool) {
            if (!source.gameObject.activeInHierarchy) {
                return source;
            }
        }

        AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
        sfxPool.Add(newSource);
        return newSource;
    }

    private IEnumerator DisableSFXSource(AudioSource source, float delay) {
        yield return new WaitForSeconds(delay);
        source.gameObject.SetActive(false);
    }
}
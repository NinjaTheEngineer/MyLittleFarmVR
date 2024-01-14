using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Merchant : NinjaMonoBehaviour {
    [SerializeField] List<Transform> itemHolders = new List<Transform>();
    [SerializeField] List<SeedConfig> seedConfigs = new List<SeedConfig>();
    [SerializeField] PurchasableSeedBag seedBagPrefab;

    [SerializeField] int restockDelayMs = 10000;
    [SerializeField] Transform BuyingArea;

    int maxItems;
    private void Start() {
        maxItems = itemHolders.Count;
        for (int i = 0; i < maxItems; i++) {
            var seedBag = Instantiate(seedBagPrefab, itemHolders[i]);
            seedBag.SetupBag(seedConfigs[i]);
        }
        PurchasableSeedBag.OnBagPurchased += PopulateEmptyHolder;
        StartCoroutine(BuyCropsRoutine());
    }
    public float buyingRadius = 0.5f;
    public float buyingDelay = 1f;
    IEnumerator BuyCropsRoutine() {
        var logId = "BuyCropsRoutine";
        var waitForSeconds = new WaitForSeconds(buyingDelay);
        while (true) {
            yield return waitForSeconds;
            var colliders = Physics.OverlapSphere(BuyingArea.position, buyingRadius);
            foreach (var collider in colliders) {
                yield return null;
                var seed = collider.GetComponent<Seed>() ?? collider.GetComponentInParent<Seed>();
                if (seed == null) {
                    logw(logId, "Seed not found on collider=" + collider.name + " => continuing");
                    continue;
                }
                GoldSystem.Instance.AddGold(seed.SeedConfig.sellPrice);
                AudioManager.Instance.PlaySFX(SoundType.PurchaseItem, seed.transform.position, 0.8f);
                ParticleSystemManager.Instance.PlayParticleSystem(ParticleType.MerchantBuyCrop, seed.transform.position);
                seed.DestroySelf();
                break;
            }
        }
    }

    async void PopulateEmptyHolder() {
        var logId = "PopulateEmptyHolder";
        logd(logId, "Populating empty holder");
        await Task.Delay(restockDelayMs);
        bool populated = false;
        for (int i = 0; i < maxItems; i++) {
            var currentHolder = itemHolders[i];
            if (currentHolder.childCount == 0) {
                var seedBag = Instantiate(seedBagPrefab, currentHolder);
                var randomSeedConfig = seedConfigs[Random.Range(0, seedConfigs.Count)];
                seedBag.SetupBag(randomSeedConfig);
                logd(logId, "Populated empty holder");
                populated = true;
            }
        }
        if (populated) {
            AudioManager.Instance.PlaySFX(SoundType.Restock, transform.position);
        }
    }

    void RandomlyPopulateHolders() {
        for (int i = 0; i < maxItems; i++) {
            var seedBag = Instantiate(seedBagPrefab, itemHolders[i]);
            var randomSeedConfig = seedConfigs[Random.Range(0, seedConfigs.Count)];
            seedBag.SetupBag(randomSeedConfig);
        }
    }

    private void OnDestroy() {
        PurchasableSeedBag.OnBagPurchased -= PopulateEmptyHolder;
    }
}

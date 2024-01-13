using NinjaTools;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Merchant : NinjaMonoBehaviour
{
    [SerializeField] List<Transform> itemHolders = new List<Transform>();
    [SerializeField] List<SeedConfig> seedConfigs = new List<SeedConfig>();
    [SerializeField] PurchasableSeedBag seedBagPrefab;

    [SerializeField] int restockDelayMs = 10000;

    int maxItems;
    private void Start() {
        maxItems = itemHolders.Count;
        for (int i = 0; i < maxItems; i++) {
            var seedBag = Instantiate(seedBagPrefab, itemHolders[i]);
            seedBag.SetupBag(seedConfigs[i]);
        }
        PurchasableSeedBag.OnBagPurchased += PopulateEmptyHolder;
    }

    async void PopulateEmptyHolder() {
        var logId = "PopulateEmptyHolder";
        logd(logId, "Populating empty holder");
        await Task.Delay(restockDelayMs);
        bool populated = false;
        for (int i = 0; i < maxItems; i++) {
            var currentHolder = itemHolders[i];
            if(currentHolder.childCount == 0) {
                var seedBag = Instantiate(seedBagPrefab, currentHolder);
                var randomSeedConfig = seedConfigs[Random.Range(0, seedConfigs.Count)];
                seedBag.SetupBag(randomSeedConfig);
                logd(logId, "Populated empty holder");
                populated = true;
            }
        }
        if(populated) {
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

using NinjaTools;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PurchasableSeedBag : NinjaMonoBehaviour {
    public List<Transform> cropHolders;
    public SeedConfig defaultSeedConfig;
    public TextMeshProUGUI priceText;
    public GameObject canvas;
    public MeshRenderer meshRenderer;
    Vector3 initPosition;
    Quaternion initRotation;
    public float purchaseDistance = 1f;
    int price;
    SeedConfig _seedConfig;

    public static Action OnBagPurchased;

    private void Awake() {
        initPosition = transform.position;
        initRotation = transform.rotation;
    }
    public void SetupBag(SeedConfig seedConfig = null) {
        _seedConfig = seedConfig ?? defaultSeedConfig;
        price = _seedConfig.price;
        priceText.text = price.ToString()+" GOLD";
        var cropHoldersCount = cropHolders.Count;
        for (int i = 0; i < cropHoldersCount; i++)
        {
            Instantiate(_seedConfig.purchasableBagCrop, cropHolders[i]);
        }
        meshRenderer.material.color = _seedConfig.bagColor;
    }
    private void Update() {
        var distance = Vector3.Distance(initPosition, transform.position);
        if(distance > purchaseDistance) {
            TryToPurchase();
        }
        
    }
    public void TryToPurchase() {
        if(GoldSystem.SpendGold(price)) {
            OnPurchase();
            return;
        }
        transform.position = initPosition;    
        transform.rotation = initRotation;
    }
    public void OnPurchase() {
        var logId = "OnPurchase";
        var seedBag = gameObject.AddComponent<SeedBag>();
        seedBag.SetSeedConfiguration(_seedConfig);
        var pos = transform.position;
        transform.parent = null;
        transform.localPosition = pos;
        OnBagPurchased?.Invoke();
        logd(logId, "Purchased");
        Destroy(canvas);
        Destroy(this);
    }
}

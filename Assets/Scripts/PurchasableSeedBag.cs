using NinjaTools;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PurchasableSeedBag : NinjaMonoBehaviour {
    public List<Transform> cropHolders;
    public SeedConfig seedConfig;
    public TextMeshProUGUI priceText;
    public GameObject canvas;
    public MeshRenderer meshRenderer;
    Vector3 initPosition;
    Quaternion initRotation;
    public float purchaseDistance = 1f;
    int price;
    private void Awake() {
        initPosition = transform.position;
        initRotation = transform.rotation;
    }
    private void Start() {
        price = seedConfig.price;
        priceText.text = price.ToString()+" GOLD";
        var cropHoldersCount = cropHolders.Count;
        for (int i = 0; i < cropHoldersCount; i++)
        {
            Instantiate(seedConfig.purchasableBagCrop, cropHolders[i]);
        }
        meshRenderer.material.color = seedConfig.bagColor;
    }
    private void Update() {
        //check if current position and initPosition are in purchaseDistance, if so, try to purchase
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
        var seedBag = gameObject.AddComponent<SeedBag>();
        seedBag.SetSeedConfiguration(seedConfig);
        Destroy(canvas);
        Destroy(this);
    }
}

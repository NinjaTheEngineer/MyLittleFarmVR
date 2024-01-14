using DG.Tweening;
using NinjaTools;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasableSeedBag : NinjaMonoBehaviour {
    public List<Transform> cropHolders;
    public SeedConfig defaultSeedConfig;
    public TextMeshProUGUI priceText;
    public GameObject canvas;
    public MeshRenderer meshRenderer;
    public Rigidbody rigidbody;
    Vector3 initPosition;
    Quaternion initRotation;
    public float purchaseDistance = 1f;
    int price;
    SeedConfig _seedConfig;

    public static Action OnBagPurchased;

    private void Awake() {
        initPosition = transform.position;
        initRotation = transform.rotation;
        errorBg.gameObject.SetActive(false);
        rigidbody = GetComponent<Rigidbody>();
    }
    public void SetupBag(SeedConfig seedConfig = null) {
        _seedConfig = seedConfig ?? defaultSeedConfig;
        price = _seedConfig.price;
        priceText.text = price.ToString() + " GOLD";
        var cropHoldersCount = cropHolders.Count;
        for (int i = 0; i < cropHoldersCount; i++) {
            Instantiate(_seedConfig.purchasableBagCrop, cropHolders[i]);
        }
        meshRenderer.material.color = _seedConfig.bagColor;
    }
    private void Update() {
        var distance = Vector3.Distance(initPosition, transform.position);
        if (distance > purchaseDistance) {
            TryToPurchase();
        }

    }
    public void TryToPurchase() {
        if (GoldSystem.Instance.SpendGold(price)) {
            AudioManager.Instance.PlaySFX(SoundType.PurchaseItem, transform.position);
            OnPurchase();
            return;
        }
        OnPurchaseFailed();
        transform.position = initPosition;
        transform.rotation = initRotation;
        rigidbody.velocity = Vector3.zero;
    }
    [SerializeField] TMPro.TextMeshProUGUI errorText;
    [SerializeField] Image errorBg;
    [SerializeField] float errorBgFinalY = 0.225f;
    [SerializeField] float tweenErrorDuration = 0.25f;
    [SerializeField] Ease tweenErrorEase = Ease.OutBounce;
    [SerializeField] Ease tweenTextErrorEase = Ease.OutBounce;
    public void OnPurchaseFailed() {
        var logId = "OnPurchaseFailed";
        logd(logId, "Failed");

        if (errorTween != null && errorTween.IsPlaying()) {
            logw(logId, "Error tween is already playing Tween="+errorTween.logf());
            return;
        }
        AudioManager.Instance.PlaySFX(SoundType.PurchaseItemFail, transform.position);
        errorBg.transform.localPosition = Vector3.zero;
        errorBg.gameObject.SetActive(true);
        //use dotween to fade the alpha color of the error text from 0 to 1
        errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 0);
        errorText.DOFade(1, tweenErrorDuration).SetEase(tweenTextErrorEase);
        errorTween = errorBg.transform.DOLocalMoveY(errorBgFinalY, tweenErrorDuration).SetEase(tweenErrorEase).OnComplete(() => DisableError());

    }
    void DisableError() {
        errorText.DOFade(0, tweenErrorDuration).SetEase(tweenTextErrorEase);
        errorTween = errorBg.transform.DOLocalMoveY(0, tweenErrorDuration).SetEase(tweenErrorEase).OnComplete(() => errorBg.gameObject.SetActive(false));
    }
    Tween errorTween;
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

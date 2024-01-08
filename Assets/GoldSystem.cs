using NinjaTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSystem : NinjaMonoBehaviour
{
    public static Action<int> OnGoldChanged;
    [SerializeField] int startingGold = 10;
    static int _currentGold;
    public static int CurrentGold { 
        get => _currentGold;
        private set {
            var logId = "CurrentGold_set";
            value = value > 0 ? value : 0;
            _currentGold = value;
            OnGoldChanged?.Invoke(_currentGold);
        }
    }

    private void Start() {
        CurrentGold = startingGold;
    }

    public bool SpendGold(int amount) {
        var logId = "SpendGold";
        if (CurrentGold < amount) {
            logd(logId, "Not enough gold");
            return false;
        }
        CurrentGold -= amount;
        return true;
    }
    public void AddGold(int amount) {
        var logId = "AddGold";
        CurrentGold += amount;
    }
}

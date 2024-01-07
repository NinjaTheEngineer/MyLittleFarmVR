using NinjaTools;
using System;

public class GameManager : NinjaMonoBehaviour {
    public static GameManager Instance;
    public Action OnGameStart;
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartGame() {
        var logId = "StartGame";
        OnGameStart?.Invoke();
    }
}

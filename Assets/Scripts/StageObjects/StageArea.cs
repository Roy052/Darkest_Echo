using UnityEngine;
using UnityEngine.SceneManagement;

public class StageArea : StageObject
{
    public int areaNum;

    private void Awake()
    {
        funcEnterPlayer = OnEnterPlayer;
        funcExitPlayer = OnExitPlayer;
    }

    void OnEnterPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Debug.Log($"Area {areaNum} Enter");
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "StageGenerator")
            return;
#endif
        if (other.CompareTag("Player") == false || Singleton.stageSM.areaFunc.Count <= areaNum) return;
        Singleton.stageSM.areaFunc[areaNum]?.Invoke(true);
    }

    void OnExitPlayer(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Debug.Log($"Area {areaNum} Exit");
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "StageGenerator")
            return;
#endif
        if (other.CompareTag("Player") == false || Singleton.stageSM.areaFunc.Count <= areaNum) return;
        Singleton.stageSM.areaFunc[areaNum]?.Invoke(false);
    }
}
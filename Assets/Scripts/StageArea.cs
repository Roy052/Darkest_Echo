using UnityEngine;
using UnityEngine.SceneManagement;

public class StageArea : StageObject
{
    public int areaNum;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Debug.Log($"Area {areaNum} Enter");
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "StageGenerator")
            return;
#endif
        if (collision.CompareTag("Player") == false || Singleton.stageSM.areaFunc.Count <= areaNum) return;
        Singleton.stageSM.areaFunc[areaNum]?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Debug.Log($"Area {areaNum} Exit");
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "StageGenerator")
            return;
#endif
        if (collision.CompareTag("Player") == false || Singleton.stageSM.areaFunc.Count <= areaNum) return;
        Singleton.stageSM.areaFunc[areaNum]?.Invoke(false);
    }
}
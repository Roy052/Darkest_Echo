using UnityEngine;
using UnityEngine.Events;

public enum StageObjectType
{
    AlertDevice = 0,
    StageArea = 1,
    Trap = 2,
    Water = 3,
    MovingObject = 4,
    UnlockZone = 5,
}

public class StageObject : MonoBehaviour
{
    public StageObjectType type;
    public UnityAction<Collider2D> funcEnterPlayer;
    public UnityAction<Collider2D> funcEnterSoundWave;
    public UnityAction<Collider2D> funcExitPlayer;
    public UnityAction<Collider2D> funcExitSoundWave;

    AudioSource audioSrc;

    private void Awake()
    {
        TryGetComponent(out audioSrc);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Str.TagPlayer))
        {
            funcEnterPlayer?.Invoke(other);
            if (audioSrc != null)
                audioSrc.Play();
        }
        else if (other.CompareTag(Str.TagSoundWave))
            funcEnterSoundWave?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Str.TagPlayer))
        {
            funcExitPlayer?.Invoke(other);
            if (audioSrc != null)
                audioSrc.Play();
        }
        else if (other.CompareTag(Str.TagSoundWave))
            funcExitSoundWave?.Invoke(other);
    }
}

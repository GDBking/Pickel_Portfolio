using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public void PlayBgm(AudioClip bgm)
    {
        AudioManager.Instance.PlayBgm(bgm);
    }

    public void PlaySfx(AudioClip sfx)
    {
        AudioManager.Instance.PlaySfx(sfx);
    }
}

using UnityEngine;
// using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("#BGM")]
    public float bgmVolume;
    public AudioSource bgmPlayer;

    [Header("#SFX")]
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

   // public AudioMixerGroup mixerGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        bgmVolume = PlayerPrefs.GetInt("BgmSound", 30) / 100f;
        sfxVolume = PlayerPrefs.GetInt("SfxSound", 100) / 100f;

        Init();
    }

    private void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new("bgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        // bgmPlayer.outputAudioMixerGroup = mixerGroup;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            // sfxPlayers[i].outputAudioMixerGroup = mixerGroup;
            sfxPlayers[i].playOnAwake = false;
        }
    }

    public void PlayBgm(AudioClip bgmClip)
    {
        bgmPlayer.clip = bgmClip;
        bgmPlayer.Play();
    }

    public void PlaySfx(AudioClip clip, float vol = 1f)
    {
        for (int i = 0; i < channels; i++)
        {
            int loopIndex = (i + channelIndex) % channels;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[channelIndex].clip = clip;
            sfxPlayers[channelIndex].volume = sfxVolume * vol;
            sfxPlayers[channelIndex].Play();
            break;
        }
    }
}
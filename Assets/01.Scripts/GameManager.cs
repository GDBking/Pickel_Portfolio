using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] AudioClip villageBgm;
    [SerializeField] AudioClip mineBgm;
    public AudioClip npcInteractionSound;

    [HideInInspector] public Camera cam;

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
            return;
        }
        cam = Camera.main;
        //SaveManager.Load();
    }

    float playTime;
    private void Update()
    {
        playTime += Time.unscaledDeltaTime;
    }

    public void SavePlayTime()
    {
        SaveManager.saveData.endingLog.playTime += (int)playTime;
        playTime = 0f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cam = Camera.main;

        if (scene.name == "2.Village" && SaveManager.saveData.isInit)
            AudioManager.Instance.PlayBgm(villageBgm);
        else if (scene.name == "3.Mine")
            AudioManager.Instance.PlayBgm(mineBgm);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
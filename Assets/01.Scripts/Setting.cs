using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [Header("필터")]
    [SerializeField] bool is16v9;
    [SerializeField] bool hasHz;

    [Header("해상도 버튼")]
    [SerializeField] Button resLeftButton;
    [SerializeField] Button resRightButton;

    [Header("전체화면 모드 버튼")]
    [SerializeField] Button fsLeftButton;
    [SerializeField] Button fsRightButton;

    [Header("표시용 텍스트")]
    [SerializeField] TextMeshProUGUI resolutionText;      // ex: "1920 x 1080 60Hz"
    [SerializeField] TextMeshProUGUI fullscreenModeText;  // ex: "창모드 / 보더리스 / 전체화면"

    [Header("오디오")]
    public Slider bgmBar;
    public Slider sfxBar;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    List<Resolution> resolutions = new();

    const string KEY_RES_INDEX = "ResolutionIndex";
    const string KEY_FULLSCREEN_MODE_INDEX = "FullScreenModeIndex";
    const string KEY_BGM = "BgmSound";
    const string KEY_SFX = "SfxSound";

    public int FullScreenModeIndex
    {
        get => PlayerPrefs.GetInt(KEY_FULLSCREEN_MODE_INDEX, 1);
        set => PlayerPrefs.SetInt(KEY_FULLSCREEN_MODE_INDEX, value);
    }

    public int ResolutionIndex
    {
        get => PlayerPrefs.GetInt(KEY_RES_INDEX, 0);
        set => PlayerPrefs.SetInt(KEY_RES_INDEX, value);
    }

    private class LocaleTexts
    {
        public string noResolution;
        public string hzSuffix;
        public string windowed;
        public string borderless;
        public string fullscreen;
        public string unknown;
    }

    private Dictionary<string, LocaleTexts> _localeMap;

    private void Awake()
    {
        if (resLeftButton != null) resLeftButton.onClick.AddListener(PrevResolution);
        if (resRightButton != null) resRightButton.onClick.AddListener(NextResolution);
        if (fsLeftButton != null) fsLeftButton.onClick.AddListener(PrevFullscreenMode);
        if (fsRightButton != null) fsRightButton.onClick.AddListener(NextFullscreenMode);

        _localeMap = new Dictionary<string, LocaleTexts>()
        {
            { "ko", new LocaleTexts {
                noResolution = "해상도 없음",
                hzSuffix = "Hz",
                windowed = "창모드",
                borderless = "테두리 없는 창",
                fullscreen = "전체 화면",
                unknown = "알 수 없음"
            } },
            { "en", new LocaleTexts {
                noResolution = "No resolution",
                hzSuffix = "Hz",
                windowed = "Windowed",
                borderless = "Borderless",
                fullscreen = "Fullscreen",
                unknown = "Unknown"
            } },
            { "fr", new LocaleTexts {
                noResolution = "Aucune résolution",
                hzSuffix = "Hz",
                windowed = "Fenêtre",
                borderless = "Sans bordure",
                fullscreen = "Plein écran",
                unknown = "Inconnu"
            } },
            { "it", new LocaleTexts {
                noResolution = "Nessuna risoluzione",
                hzSuffix = "Hz",
                windowed = "Finestra",
                borderless = "Senza bordi",
                fullscreen = "Schermo intero",
                unknown = "Sconosciuto"
            } },
            { "de", new LocaleTexts {
                noResolution = "Keine Auflösung",
                hzSuffix = "Hz",
                windowed = "Fenstermodus",
                borderless = "Rahmenlos",
                fullscreen = "Vollbild",
                unknown = "Unbekannt"
            } },
            { "es", new LocaleTexts {
                noResolution = "Sin resolución",
                hzSuffix = "Hz",
                windowed = "Ventana",
                borderless = "Sin bordes",
                fullscreen = "Pantalla completa",
                unknown = "Desconocido"
            } },
            { "ja", new LocaleTexts {
                noResolution = "解像度なし",
                hzSuffix = "Hz",
                windowed = "ウィンドウ",
                borderless = "ボーダレス",
                fullscreen = "フルスクリーン",
                unknown = "不明"
            } },
            { "pt", new LocaleTexts {
                noResolution = "Sem resolução",
                hzSuffix = "Hz",
                windowed = "Janela",
                borderless = "Sem bordas",
                fullscreen = "Tela cheia",
                unknown = "Desconhecido"
            } },
            { "ru", new LocaleTexts {
                noResolution = "Нет разрешения",
                hzSuffix = "Hz",
                windowed = "Оконный режим",
                borderless = "Без границ",
                fullscreen = "Полноэкранный",
                unknown = "Неизвестно"
            } },
            { "zh", new LocaleTexts {
                noResolution = "无可用分辨率",
                hzSuffix = "Hz",
                windowed = "窗口模式",
                borderless = "无边框窗口",
                fullscreen = "全屏",
                unknown = "未知"
            } },
        };
    }

    private void Start()
    {
        // resolutions 준비를 위해 약간 지연
        Invoke(nameof(InitResolutions), 0.05f);
        // 저장된 전체화면 모드 적용
        Invoke(nameof(ApplySavedFullScreenMode), 0.07f);

        if (SceneManager.GetActiveScene().name == "2.Dungeon")
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        bgmBar.value = PlayerPrefs.GetInt(KEY_BGM, 70);
        sfxBar.value = PlayerPrefs.GetInt(KEY_SFX, 100);

        SetBgmText(bgmBar.value);
        SetSfxText(sfxBar.value);
    }
    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void InitResolutions()
    {
        resolutions = new List<Resolution>(Screen.resolutions);
        resolutions.Reverse();

        if (is16v9)
            resolutions = resolutions.FindAll(x => Mathf.Approximately((float)x.width / x.height, 16f / 9f));

        if (!hasHz && resolutions.Count > 0)
        {
            HashSet<string> seen = new();
            List<Resolution> temp = new();
            foreach (var r in resolutions)
            {
                string key = $"{r.width}x{r.height}";
                if (!seen.Contains(key))
                {
                    seen.Add(key);
                    temp.Add(r);
                }
            }
            resolutions = temp;
        }

        // 안전한 인덱스
        if (resolutions.Count == 0)
        {
            ResolutionIndex = 0;
        }
        else
        {
            ResolutionIndex = Mathf.Clamp(ResolutionIndex, 0, resolutions.Count - 1);
            ApplyResolution(ResolutionIndex);
        }

        UpdateDisplays();
    }

    void ApplyResolution(int index)
    {
        if (resolutions == null || resolutions.Count == 0) return;
        index = Mathf.Clamp(index, 0, resolutions.Count - 1);
        var r = resolutions[index];

        // Screen.fullScreenMode을 유지하면서 해상도 적용
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);

        ResolutionIndex = index;
        UpdateDisplays();
    }

    // 왼/오 버튼 - 해상도 (사이클)
    public void PrevResolution()
    {
        if (resolutions == null || resolutions.Count == 0) return;
        int idx = ResolutionIndex + 1;
        if (idx >= resolutions.Count) idx = 0;
        ApplyResolution(idx);
    }

    public void NextResolution()
    {
        if (resolutions == null || resolutions.Count == 0) return;
        int idx = ResolutionIndex - 1;
        if (idx < 0) idx = resolutions.Count - 1;
        ApplyResolution(idx);
    }

    // ---------- 전체화면 모드 3단 ----------
    void ApplySavedFullScreenMode()
    {
        ApplyFullScreenMode(FullScreenModeIndex, false);
    }

    void ApplyFullScreenMode(int modeIndex, bool setPlayerPrefs)
    {
        modeIndex = Mathf.Clamp(modeIndex, 0, 2);
        FullScreenMode unityMode = FullScreenMode.FullScreenWindow;
        switch (modeIndex)
        {
            case 0: unityMode = FullScreenMode.Windowed; break;
            case 1: unityMode = FullScreenMode.FullScreenWindow; break;
            case 2: unityMode = FullScreenMode.ExclusiveFullScreen; break;
        }

        Screen.fullScreenMode = unityMode;
        Screen.fullScreen = (unityMode != FullScreenMode.Windowed);

        if (setPlayerPrefs) FullScreenModeIndex = modeIndex;

        UpdateDisplays();
    }

    public void PrevFullscreenMode()
    {
        int idx = FullScreenModeIndex - 1;
        if (idx < 0) idx = 2;
        ApplyFullScreenMode(idx, true);
    }

    public void NextFullscreenMode()
    {
        int idx = FullScreenModeIndex + 1;
        if (idx > 2) idx = 0;
        ApplyFullScreenMode(idx, true);
    }
    private LocaleTexts GetLocaleTexts()
    {
        string localeCode = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "en";

        if (string.IsNullOrEmpty(localeCode)) localeCode = "en";

        // 정확 매칭
        if (_localeMap.TryGetValue(localeCode, out var exact))
            return exact;

        // 접두사 매칭 (pt-BR -> pt)
        int dash = localeCode.IndexOf('-');
        if (dash > 0)
        {
            string prefix = localeCode.Substring(0, dash).ToLower();
            if (_localeMap.TryGetValue(prefix, out var prefixVal))
                return prefixVal;
        }

        // zh 계열 매칭
        if (localeCode.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
        {
            if (_localeMap.TryGetValue("zh", out var zhVal))
                return zhVal;
        }

        // fallback en
        if (_localeMap.TryGetValue("en", out var enVal))
            return enVal;

        // 마지막 안전장치: 임의의 첫 항목
        foreach (var kv in _localeMap)
            return kv.Value;

        // 절대 도달하지 않음
        return new LocaleTexts
        {
            noResolution = "No resolution",
            hzSuffix = "Hz",
            windowed = "Windowed",
            borderless = "Borderless",
            fullscreen = "Fullscreen",
            unknown = "Unknown"
        };
    }

    void UpdateDisplays()
    {
        var texts = GetLocaleTexts();

        // 해상도 텍스트
        if (resolutionText != null)
        {
            if (resolutions != null && resolutions.Count > 0)
            {
                var r = resolutions[Mathf.Clamp(ResolutionIndex, 0, resolutions.Count - 1)];
                string hzPart = "";
                if (hasHz)
                {
                    // refreshRateRatio 사용 코드는 기존 코드 유지
                    hzPart = $" {Mathf.RoundToInt((float)r.refreshRateRatio.value)}{texts.hzSuffix}";
                }
                resolutionText.text = $"{r.width} x {r.height}{hzPart}";
            }
            else
            {
                resolutionText.text = texts.noResolution;
            }
        }

        // 전체화면 모드 텍스트
        if (fullscreenModeText != null)
        {
            fullscreenModeText.text = FullScreenModeIndex switch
            {
                0 => texts.windowed,
                1 => texts.borderless,
                2 => texts.fullscreen,
                _ => texts.unknown,
            };
        }
    }

    // ---------- 오디오 ----------
    public void SetBgmText(float value)
    {
        bgmText.text = ((int)value).ToString();
        AudioManager.Instance.bgmPlayer.volume = value / 100f;

        PlayerPrefs.SetInt(KEY_BGM, (int)value);
        PlayerPrefs.Save();
    }

    public void SetSfxText(float value)
    {
        sfxText.text = ((int)value).ToString();
        AudioManager.Instance.sfxVolume = value / 100f;

        PlayerPrefs.SetInt(KEY_SFX, (int)value);
        PlayerPrefs.Save();
    }

    // 저장 버튼이 있는 경우 호출
    public void SaveBtnClick()
    {
        PlayerPrefs.SetInt(KEY_RES_INDEX, ResolutionIndex);
        PlayerPrefs.SetInt(KEY_FULLSCREEN_MODE_INDEX, FullScreenModeIndex);
        PlayerPrefs.SetInt(KEY_BGM, (int)bgmBar.value);
        PlayerPrefs.SetInt(KEY_SFX, (int)sfxBar.value);
        PlayerPrefs.Save();
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale newLocale)
    {
        UpdateDisplays();
    }

    public void LobbyBtn()
    {
        if (TotalPanel.Instance != null)
            TotalPanel.Instance.SaveBagOrePiece();

        SaveManager.Save();
        SceneManager.LoadScene("1.Lobby");
    }

    public void VillageBtn()
    {
        SceneManager.LoadScene("2.Village");
    }
}
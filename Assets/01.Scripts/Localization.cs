using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LocalizationData
{
    public string languageCode; // "En", "Fr", "De", "Zh" 등 언어 코드
    public string name;
    [TextArea(10, 100)] public string description;
}

public class Localization : MonoBehaviour
{
    public static Localization instance;
    public int index;

    [System.Serializable]
    public class LocaleFontPair
    {
        public string localeCode; // ex: "en", "ko", "ja", "zh-Hans", "pt-BR", "ru"
        public TMP_FontAsset font;
    }

    [Header("Font Mapping (set in Inspector)")]
    public List<LocaleFontPair> localeFonts = new();

    [Header("Options")]
    public bool applyFontsToAllTMP = true; // true면 씬의 모든 TMP_Text에 적용
    public bool applyOnStart = true;       // Start 시 폰트 자동 적용 여부

    private bool isInitialized = false;     // 초기화 완료 플래그

    private void Awake()
    {
        // 기존 인스턴스 처리 (중복 방지)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        index = PlayerPrefs.GetInt("localeIndex", -1);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        // 1. Unity Localization 초기화 대기
        yield return LocalizationSettings.InitializationOperation;

        // 2. SteamManager 초기화 대기 (최대 120프레임)
        int waitFrame = 0;
        while (!SteamManager.Initialized && waitFrame < 120)
        {
            waitFrame++;
            yield return null;
        }

        // 3. 언어 설정 필요할 경우 (최초 실행 시 Steam 언어 감지)
        if (SteamManager.Initialized && index == -1)
        {
            string lang = SteamApps.GetCurrentGameLanguage().ToLower();
            Debug.Log("[Steam] Game Language: " + lang);

            if (lang == "korean" || lang == "koreana")
            {
                index = FindLocaleIndex("ko");
            }
            else if (lang == "english")
            {
                index = FindLocaleIndex("en");
            }
            else
            {
                // 지원되지 않는 언어 → 기본값 영어로 fallback
                index = FindLocaleIndex("en");
            }

            // 그래도 못 찾으면 ko → 그마저도 없으면 0번 인덱스
            if (index == -1)
            {
                index = FindLocaleIndex("ko");
                if (index == -1) index = 0;
            }

            PlayerPrefs.SetInt("localeIndex", index);
            PlayerPrefs.Save();
        }

        // 4. 이벤트 구독 (초기 설정 전에 구독하여 유실 방지)
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

        // 초기화 완료 선언
        isInitialized = true;

        // 5. 언어 적용
        if (LocalizationSettings.InitializationOperation.Status == AsyncOperationStatus.Succeeded)
        {
            int totalLocales = LocalizationSettings.AvailableLocales.Locales.Count;
            if (index < 0 || index >= totalLocales)
            {
                index = 0;
            }

            var selected = LocalizationSettings.AvailableLocales.Locales[index];
            if (LocalizationSettings.SelectedLocale != selected)
            {
                // 이 대입으로 인해 OnSelectedLocaleChanged가 자동 트리거됨
                LocalizationSettings.SelectedLocale = selected;
            }
            else if (applyOnStart)
            {
                // 이미 같은 Locale이 세팅되어 있다면 수동으로 폰트 적용
                ApplyFontsForLocale(selected);
            }
        }
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 초기화가 아직 끝나지 않았다면 씬 로드 시점의 폰트 변경을 스킵 (Start에서 해줄 것임)
        if (!isInitialized) return;

        if (LocalizationSettings.SelectedLocale != null)
        {
            ApplyFontsForLocale(LocalizationSettings.SelectedLocale);
        }
    }

    // --- 왼/오 버튼으로 전환할 수 있는 메서드들 ---
    public void ChangeLocaleByDelta(int delta)
    {
        if (!isInitialized) return;

        int count = LocalizationSettings.AvailableLocales.Locales.Count;
        if (count == 0) return;

        // index 무효 시 보정
        if (index < 0 || index >= count)
        {
            index = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            if (index < 0) index = 0;
        }

        index += delta;

        // 순환 처리
        if (index < 0) index = count - 1;
        if (index >= count) index = 0;

        // Locale 설정 (SelectedLocaleChanged 이벤트를 타고 폰트가 적용됨)
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        // 저장
        PlayerPrefs.SetInt("localeIndex", index);
        PlayerPrefs.Save();
    }

    public void OnClickNextLocale() => ChangeLocaleByDelta(1);
    public void OnClickPrevLocale() => ChangeLocaleByDelta(-1);

    private int FindLocaleIndex(string code)
    {
        return LocalizationSettings.AvailableLocales.Locales
            .FindIndex(locale => locale.Identifier.Code == code);
    }

    private void OnSelectedLocaleChanged(Locale newLocale)
    {
        if (newLocale == null) return;

        // 현재 인덱스 동기화
        int found = LocalizationSettings.AvailableLocales.Locales.IndexOf(newLocale);
        if (found >= 0) index = found;

        TMP_FontAsset targetFont = FindFontForLocale(newLocale.Identifier.Code);

        if (targetFont != null)
        {
            // TMP 전역 기본 폰트 교체 (이후 생성될 TMP 객체들을 위함)
            TMP_Settings.defaultFontAsset = targetFont;
        }

        ApplyFontsForLocale(newLocale);
    }

    private void ApplyFontsForLocale(Locale locale)
    {
        if (locale == null) return;

        TMP_FontAsset targetFont = FindFontForLocale(locale.Identifier.Code);

        if (targetFont == null)
        {
            Debug.LogWarning($"[Localization] No font mapped for locale '{locale.Identifier.Code}'. Skipping font apply.");
            return;
        }

        if (applyFontsToAllTMP)
        {
            // [최적화] Resources.FindObjectsOfTypeAll 대신 씬 내 활성화된 객체들 위주로 탐색
            // 비활성화된 오브젝트까지 100% 덮어써야 한다면 원래 코드가 맞으나, 렉을 유발하므로 
            // 씬이 바뀔 때 어차피 재적용되니 FindObjectsByType을 사용하는 것이 성능상 훨씬 이롭습니다.
            TMP_Text[] allTexts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var t in allTexts)
            {
                // 씬에 존재하는 컴포넌트인지 검증
                if (t != null && t.gameObject.scene.isLoaded)
                {
                    t.font = targetFont;
                }
            }
        }
    }

    private TMP_FontAsset FindFontForLocale(string localeCode)
    {
        if (string.IsNullOrEmpty(localeCode)) return null;

        // 1) 정확 매칭
        foreach (var p in localeFonts)
        {
            if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals(localeCode, System.StringComparison.OrdinalIgnoreCase))
            {
                if (p.font != null) return p.font;
            }
        }

        // 2) 접두사 매칭 (ex: "pt-BR" -> "pt")
        int dash = localeCode.IndexOf('-');
        if (dash > 0)
        {
            string prefix = localeCode[..dash];
            foreach (var p in localeFonts)
            {
                if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals(prefix, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (p.font != null) return p.font;
                }
            }
        }

        // 3) zh 계열 처리
        if (localeCode.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
        {
            foreach (var p in localeFonts)
            {
                if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.StartsWith("zh", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (p.font != null) return p.font;
                }
            }
        }

        // 4) fallback: "en"
        foreach (var p in localeFonts)
        {
            if (!string.IsNullOrEmpty(p.localeCode) && p.localeCode.Equals("en", System.StringComparison.OrdinalIgnoreCase))
            {
                if (p.font != null) return p.font;
            }
        }

        return null;
    }

    // [추가] 외부 스크립트에서 현재 언어 코드를 안전하게 읽어갈 수 있는 프로퍼티
    public string CurrentLanguageCode => LocalizationSettings.SelectedLocale != null
        ? LocalizationSettings.SelectedLocale.Identifier.Code
        : "en";

    public string GetName(List<LocalizationData> dataList, string defaultDecs)
    {
        // 1. 리스트가 비어있다면 당연히 기본 텍스트 반환
        if (dataList == null || dataList.Count == 0)
            return defaultDecs;

        // 2. 현재 유니티 로컬라이징 패키지에 세팅된 언어 코드를 가져옴 (ex: "ko", "en", "zh-Hans")
        string currentCode = CurrentLanguageCode;

        // 3. 정확히 일치하는 언어 코드가 있는지 먼저 검색 ("ko", "en", "fr" 등)
        var result = dataList.Find(x => x.languageCode.Equals(currentCode, System.StringComparison.OrdinalIgnoreCase));

        // 4. 만약 "zh-Hans"인데 리스트엔 "zh"만 적어놓은 경우를 대비한 접두사(Prefix) 매칭 처리
        if (string.IsNullOrEmpty(result.name) && currentCode.Contains("-"))
        { //
            string prefix = currentCode.Split('-')[0]; // "zh-Hans" -> "zh"
            result = dataList.Find(x => x.languageCode.Equals(prefix, System.StringComparison.OrdinalIgnoreCase));
        } //

        // 5. 번역을 찾았다면 번역본을, 없다면 기본 텍스트를 반환
        return string.IsNullOrEmpty(result.name) ? defaultDecs : result.name;
    }

    public string GetDescription(List<LocalizationData> dataList, string defaultDecs)
    {
        // 1. 리스트가 비어있다면 당연히 기본 텍스트 반환
        if (dataList == null || dataList.Count == 0)
            return defaultDecs;

        // 2. 현재 유니티 로컬라이징 패키지에 세팅된 언어 코드를 가져옴 (ex: "ko", "en", "zh-Hans")
        string currentCode = CurrentLanguageCode;

        // 3. 정확히 일치하는 언어 코드가 있는지 먼저 검색 ("ko", "en", "fr" 등)
        var result = dataList.Find(x => x.languageCode.Equals(currentCode, System.StringComparison.OrdinalIgnoreCase));

        // 4. 만약 "zh-Hans"인데 리스트엔 "zh"만 적어놓은 경우를 대비한 접두사(Prefix) 매칭 처리
        if (string.IsNullOrEmpty(result.description) && currentCode.Contains("-"))
        { //
            string prefix = currentCode.Split('-')[0]; // "zh-Hans" -> "zh"
            result = dataList.Find(x => x.languageCode.Equals(prefix, System.StringComparison.OrdinalIgnoreCase));
        } //

        // 5. 번역을 찾았다면 번역본을, 없다면 기본 텍스트를 반환
        return string.IsNullOrEmpty(result.description) ? defaultDecs : result.description;
    }
}
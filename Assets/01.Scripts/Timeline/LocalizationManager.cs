using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DialogueParser
{
    public string talker;
    public string contents;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    readonly Dictionary<int, DialogueParser> table = new();

    // UI / 시스템 텍스트
    readonly Dictionary<string, string> textTable = new();

    // ko, en, ja ...
    [SerializeField] string currentLanguage = "ko";

    void Awake()
    {
        Instance = this;

        // 현재 Unity Localization 언어 가져오기
        LoadCurrentLanguage();

        // CSV 로드
        LoadCSV("Localization/dialogue");
        LoadTextCSV("Localization/text");

        // 언어 변경 이벤트 구독
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    // =========================
    // Unity Localization 연동
    // =========================
    void LoadCurrentLanguage()
    {
        if (LocalizationSettings.SelectedLocale != null)
        {
            currentLanguage =
                LocalizationSettings
                .SelectedLocale
                .Identifier
                .Code
                .Trim();

            Debug.Log($"현재 언어: {currentLanguage}");
        }
    }

    void OnLocaleChanged(Locale locale)
    {
        if (locale == null)
            return;

        currentLanguage =
            locale
            .Identifier
            .Code
            .Trim();

        Debug.Log($"언어 변경됨: {currentLanguage}");

        LoadCSV("Localization/dialogue");
        LoadTextCSV("Localization/text");
    }

    // =========================
    // 수동 언어 변경
    // =========================
    public void SetLanguage(string lang)
    {
        currentLanguage = lang.Trim();

        LoadCSV("Localization/dialogue");
        LoadTextCSV("Localization/text");
    }

    // =========================
    // 대사용 CSV
    // =========================
    void LoadCSV(string path)
    {
        table.Clear();

        TextAsset csv = Resources.Load<TextAsset>(path);

        if (csv == null)
        {
            Debug.LogError("CSV 파일 못 찾음: " + path);
            return;
        }

        // \r 제거
        string[] lines = csv.text
            .Replace("\r", "")
            .Split('\n');

        if (lines.Length <= 0)
            return;

        // 헤더 파싱
        string[] header = SplitCSVLine(lines[0]);

        int langIndex = -1;

        for (int i = 0; i < header.Length; i++)
        {
            if (header[i].Trim().Equals(
                currentLanguage,
                StringComparison.OrdinalIgnoreCase))
            {
                langIndex = i;
                break;
            }
        }

        if (langIndex == -1)
        {
            Debug.LogError($"언어 컬럼 못 찾음: {currentLanguage}");
            return;
        }

        // 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] cols = SplitCSVLine(lines[i]);

            if (cols.Length <= langIndex)
                continue;

            int key = i + 1;

            DialogueParser value = new()
            {
                talker = cols[0].Trim(),
                contents = cols[langIndex].Trim()
            };

            table[key] = value;
        }
    }

    // =========================
    // UI / 시스템 텍스트 CSV
    // =========================
    void LoadTextCSV(string path)
    {
        textTable.Clear();

        TextAsset csv = Resources.Load<TextAsset>(path);

        if (csv == null)
        {
            Debug.LogError("텍스트 CSV 파일 못 찾음: " + path);
            return;
        }

        // \r 제거
        string[] lines = csv.text
            .Replace("\r", "")
            .Split('\n');

        if (lines.Length <= 0)
            return;

        // 헤더 파싱
        string[] header = SplitCSVLine(lines[0]);

        int langIndex = -1;

        for (int i = 0; i < header.Length; i++)
        {
            if (header[i].Trim().Equals(
                currentLanguage,
                StringComparison.OrdinalIgnoreCase))
            {
                langIndex = i;
                break;
            }
        }

        if (langIndex == -1)
        {
            Debug.LogError($"언어 컬럼 못 찾음: {currentLanguage}");

            for (int i = 0; i < header.Length; i++)
            {
                Debug.Log($"header[{i}] = [{header[i]}]");
            }

            return;
        }

        // 데이터 파싱
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] cols = SplitCSVLine(lines[i]);

            if (cols.Length <= langIndex)
                continue;

            string key = cols[0].Trim();
            string value = cols[langIndex].Trim();

            textTable[key] = value;
        }
    }

    // =========================
    // 대사 가져오기
    // =========================
    public DialogueParser Get(int key)
    {
        return table.TryGetValue(key, out var value)
            ? value
            : new DialogueParser
            {
                talker = "",
                contents = ""
            };
    }

    // =========================
    // UI 텍스트 가져오기
    // =========================
    public string GetText(string key)
    {
        if (textTable.TryGetValue(key, out string value))
            return value;

        Debug.LogWarning($"로컬라이징 키 없음: {key}");

        return key;
    }

    // =========================
    // CSV 파서
    // =========================
    string[] SplitCSVLine(string line)
    {
        List<string> result = new();

        bool inQuotes = false;

        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // "" 처리
                if (inQuotes &&
                    i + 1 < line.Length &&
                    line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current);

        return result.ToArray();
    }
}
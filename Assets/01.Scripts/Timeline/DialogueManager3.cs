using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayableDirector))]
public class DialogueManager3 : MonoBehaviour
{
    [SerializeField] AudioClip dialogueSound;

    [HideInInspector] public PlayableDirector director;

    [SerializeField] DialogueData[] datas;
    [SerializeField] Transform[] transforms;

    [SerializeField] GameObject[] explosionEffects;
    [SerializeField] AudioClip explosionSound;

    const string END_MARK = "**";

    readonly string[] oreKeys =
    {
        "ore_coal",
        "ore_tin",
        "ore_copper",
        "ore_iron",
        "ore_gold",
        "ore_sapphire",
        "ore_ruby",
        "ore_emerald",
        "ore_diamond",
        "ore_red_diamond"
    };

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    public void StartDialogue(int idx)
    {
        StartCoroutine(ShowDialogueSequence(datas[idx], idx));
    }

    public IEnumerator ShowDialogueSequence(DialogueData data, int idx)
    {
        director.Pause();

        int row = data.minRow;

        while (true)
        {
            DialogueParser text = LocalizationManager.Instance.Get(row);

            // 종료 마커
            if (text.contents.StartsWith(END_MARK))
                break;

            yield return null;

            // 위치 설정
            DialogueUI.Instance.SetTextPos2(
                text.talker == "p"
                    ? transforms[idx * 2].position
                    : transforms[idx * 2 + 1].position,
                text.talker,
                data.textColor);

            // 변수 파싱
            if (row == 632)
            {
                string parsedText = ParseVariables(text.contents);
                yield return StartCoroutine(TypeText(parsedText));
            }
            else
                yield return StartCoroutine(TypeText(text.contents));

            // 입력 대기
            yield return new WaitUntil(
                () => Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame);

            DialogueUI.Instance.Show("");

            row++;
        }

        DialogueUI.Instance.Hide();

        director.Play();
    }

    string ParseVariables(string text)
    {
        SaveManager.Save();

        SaveManager.EndingLog log =
            SaveManager.saveData.endingLog;

        StringBuilder builder = new();

        // 기본 로그
        builder.AppendLine(
            $"{L("ending_total_mine")}: {log.pickaxCnt}");

        builder.AppendLine(
            $"{L("ending_total_bomb")}: {log.bombCnt}");

        builder.AppendLine(
            $"{L("ending_total_potion")}: {log.potionCnt}");

        builder.AppendLine(
            $"{L("ending_total_enter")}: {log.mineCnt}");

        builder.AppendLine();

        // 광석 로그
        builder.AppendLine(
            $"{L("ending_total_ore")}");

        for (int i = 0; i < log.orePieces.Length; i++)
        {
            builder.AppendLine(
                $"{L(oreKeys[i])}: {log.orePieces[i]}");
        }

        builder.AppendLine();

        // 플레이 타임
        int totalSeconds = log.playTime;

        int hour = totalSeconds / 3600;
        int minute = totalSeconds % 3600 / 60;
        int second = totalSeconds % 60;

        string playTime = string.Format(
            L("ending_play_time_format"),
            hour,
            minute,
            second);

        builder.AppendLine(
            $"{L("ending_total_play_time")}: {playTime}");

        // 치환
        text = text.Replace(
            "{ENDING_LOG}",
            builder.ToString());

        return text;
    }

    string L(string key)
    {
        return LocalizationManager.Instance.GetText(key);
    }

    readonly WaitForSeconds wait = new(0.03f);

    IEnumerator TypeText(string text)
    {
        DialogueUI.Instance.Show("");

        for (int i = 0; i < text.Length; i++)
        {
            if (i % 5 == 0)
                AudioManager.Instance.PlaySfx(dialogueSound);

            DialogueUI.Instance.Show(text[..(i + 1)]);

            yield return wait;
        }
    }

    public void GenerateExplosion()
    {
        StartCoroutine(GenerateExplosionRoutine());
    }

    readonly WaitForSeconds wait2 = new(0.1f);

    readonly Vector2 min = new(-7.85f, -3.94f);
    readonly Vector2 max = new(7.95f, 3.96f);

    IEnumerator GenerateExplosionRoutine()
    {
        for (int i = 0; i < 20; i++)
        {
            AudioManager.Instance.PlaySfx(explosionSound, 0.3f);

            Instantiate(
                explosionEffects[
                    Random.Range(0, explosionEffects.Length)],

                new(
                    Random.Range(min.x, max.x),
                    Random.Range(min.y, max.y)),

                Quaternion.identity);

            yield return wait2;
        }
    }

    public void ChangeScene()
    {
        SaveManager.DeleteSave();

        SceneManager.LoadScene("1.Lobby");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerEnding : MonoBehaviour
{
    private static readonly int IsBondageHash = Animator.StringToHash("isBondage");
    [SerializeField] GameObject dungeon;
    [SerializeField] Transform dungeonEntrance;

    public void MoveToDungeonEntrance()
    {
        dungeon.SetActive(true);
        transform.position = dungeonEntrance.position;
    }

    public void EnterCutscene()
    {
        PlayerDig.Instance.enabled = false;
        TotalPanel.Instance.isChangeScene = false;
    }

    public void ExitCutscene() 
    { 
        if (PlayerDig.Instance != null)
            PlayerDig.Instance.enabled = true;
    }

    public void Bondage()
    {
        GetComponent<Animator>().SetBool(IsBondageHash, true);
        GetComponent<CapsuleCollider2D>().offset = new(-1.490116e-08f, -0.2f);
    }

    public void ChangeDestroyedVillage()
    {
        SceneManager.LoadScene("4.DestroyedVillage");
    }
}

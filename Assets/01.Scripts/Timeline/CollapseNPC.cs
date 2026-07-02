using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CollapseNPC : NPC
{
    public override void Interact()
    {
        if (TotalPanel.Instance.gameObject.activeSelf) return;

        base.Interact();

        anim.SetBool(IdleHash, true);
    }
}

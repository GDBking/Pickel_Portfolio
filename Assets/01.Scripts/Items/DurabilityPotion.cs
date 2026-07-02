using UnityEngine;

public class DurabilityPotion : Item
{
    [SerializeField] int incDurabillity;
    [SerializeField] ParticleSystem potionEffect;
    [SerializeField] ParticleSystem playerEffect;
    [SerializeField] AudioClip potionSound;

    public override void UseItem(Vector2 hitNormal)
    {
        base.UseItem(hitNormal);

        AudioManager.Instance.PlaySfx(potionSound);

        Instantiate(potionEffect, transform.position, Quaternion.identity);
        Instantiate(playerEffect, playerDig.transform);

        Pickax.Instance.SetDurability(incDurabillity);

        SaveManager.saveData.endingLog.potionCnt++;
    }
}

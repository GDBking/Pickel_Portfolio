using UnityEngine;

public class PowerPotion : Item
{
    [SerializeField] float duration;
    [SerializeField] ParticleSystem potionEffect;
    [SerializeField] ParticleSystem playerEffect;
    [SerializeField] AudioClip potionSound;

    public override void UseItem(Vector2 hitNormal)
    {
        base.UseItem(hitNormal);

        AudioManager.Instance.PlaySfx(potionSound);

        Instantiate(potionEffect, transform.position, Quaternion.identity);
        Instantiate(playerEffect, playerDig.transform);

        GaugeBar.Instance.SetGaugeBar(0, duration);

        playerDig.PowerPotion(duration);

        SaveManager.saveData.endingLog.potionCnt++;
    }
}

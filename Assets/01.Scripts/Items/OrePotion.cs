using UnityEngine;

public class OrePotion : Item
{
    [SerializeField] bool isMaxOrePotion;
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

        GaugeBar.Instance.SetGaugeBar(isMaxOrePotion ? 2 : 1, duration);

        playerDig.OrePotion(duration, isMaxOrePotion);

        SaveManager.saveData.endingLog.potionCnt++;
    }
}

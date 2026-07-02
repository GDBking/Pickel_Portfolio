using UnityEngine;
public class LocaleButton : MonoBehaviour
{
    public void NextLocale()
    {
        if (Localization.instance != null)
        {
            Localization.instance.OnClickNextLocale();
        }
    }

    public void PrevLocale()
    {
        if (Localization.instance != null)
        {
            Localization.instance.OnClickPrevLocale();
        }
    }
}
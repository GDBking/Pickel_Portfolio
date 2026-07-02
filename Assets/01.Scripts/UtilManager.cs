using UnityEngine;
using UnityEngine.Localization.Settings;

public static class UtilManager
{
    public static WaitForSeconds completedWait = new(1.9f);

    public static string Localization_Text(Key_Village key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("Village", key.ToString());
    }
}

public enum Key_Village
{
    TotalQuantity, CurQuantity,
    OreProb, OrePiece, Obstacle,
    Potion1, Potion2, Potion3, Potion4, Potion5, Potion6,
    Twinkle,
    Ore0, Ore1, Ore2, Ore3, Ore4, Ore5, Ore6, Ore7, Ore8, Ore9,
}
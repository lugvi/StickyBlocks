using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinObject", menuName = "linerider/SkinObject", order = 0)]
public class SkinObject : ScriptableObject
{
    public List<Gradient> gradients;
    public Sprite background;
    public Sprite ShopIcon;
    public int UnlockScore;

    public bool Unlocked
    {
        get
        {
            return PlayerPrefs.GetInt("unlocked_new_item" + this.name) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("unlocked_new_item" + name, value ? 1 : 0);
        }
    }

    public bool CanUnlock()
    {
        return PlayerPrefs.GetInt("Highscore") >= UnlockScore;
    }

}
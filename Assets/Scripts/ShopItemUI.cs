using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{

    public Image Icon;
    public Text Title;

    [Header("Select")]
    public Button SelectButton;
    public GameObject SelectedObject;


    private static ShopItemUI lastSelected;

    public void SetUI(SkinObject item)
    {
        if (item.CanUnlock())//GameManager.Instance.GetCoins))
        {
            item.Unlocked = true;
        }

        if (item == GameManager.instance.CurrentSkin)
        {
            SetCurrent();
        }



        Icon.sprite = item.ShopIcon;
        Title.text = item.Unlocked ? item.name : "Reach \n " + item.UnlockScore;

        SelectButton.interactable = item.Unlocked;

        SelectButton.onClick.AddListener(() =>
        {

            SetCurrent();
            GameManager.instance.CurrentSkin = item;

        });

    }

    public void SetCurrent()
    {
        SelectedObject.SetActive(true);
        if (lastSelected != null && lastSelected != this)
            lastSelected.SelectedObject.SetActive(false);
        lastSelected = this;
        Debug.LogWarning("Selected " + lastSelected.name);

    }
}

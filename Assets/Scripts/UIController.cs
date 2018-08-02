using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Text scoreText;
    public Text ComboText;


    public void UpdateScore(int score)
    {
        scoreText.text = score +"";
    }
    public void UpdateCombo(int combo)
    {
        ComboText.text = combo==0?"":combo+"x";
    }
    

}

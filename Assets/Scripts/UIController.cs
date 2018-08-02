using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Text scoreText;
    public Text ComboText;

    public Slider speedSlider;

    public Text speedText;
    public Slider perfectSlider;
    public Text perfectText;
    public Slider minSlider;
    public Text minText;


    private void Start() {
        GameManager gm = GameManager.instance;
        speedSlider.value = gm.speedModifier;
        perfectSlider.value = gm.perfDist;
        minSlider.value = gm.minDist;

        speedText.text = gm.speedModifier+"";
        perfectText.text = gm.perfDist+"";
        minText.text = gm.minDist+"";
        speedSlider.onValueChanged.AddListener((v)=>
        {
            speedText.text = v+"";
            gm.speedModifier = v;
        });
        perfectSlider.onValueChanged.AddListener((v)=>
        {
            perfectText.text = v+"";
            gm.perfDist = v;
        });
        minSlider.onValueChanged.AddListener((v)=>
        {
            gm.minDist = v;
            minText.text = v+"";
        });
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score +"";
    }
    public void UpdateCombo(int combo)
    {
        ComboText.text = combo==0?"":combo+"x";
    }
    

}

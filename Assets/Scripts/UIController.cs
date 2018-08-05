using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Text currentScoreText;
    public Text ComboText;

    public Slider speedSlider;

    public Text speedText;
    public Slider perfectSlider;
    public Text perfectText;
    public Slider minSlider;
    public Text minText;

    public GameObject gameOverPanel;

    // public Text endScoreText;
    public Text highScoreText;

    private Animation scoreAnimation;
    private Animation comboAnimation;


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

        scoreAnimation = currentScoreText.GetComponent<Animation>();
        comboAnimation = ComboText.GetComponent<Animation>();

        Debug.Log("Init UI");

    }

    public void UpdateScore(int score)
    {
        Debug.Log("UPdate Score");
        currentScoreText.text = score +"";
        if(scoreAnimation.isPlaying)scoreAnimation.Stop();
        scoreAnimation.Play();
    }
    public void UpdateCombo(int combo)
    {
        ComboText.transform.position = Input.mousePosition + Vector3.one*100;
        ComboText.text = combo<2?"":combo+"x";
        if(comboAnimation.isPlaying)comboAnimation.Stop();
        comboAnimation.Play();
    }
    

    public void OnGameOver()
    {
        gameOverPanel.SetActive(true);
        // endScoreText.text = currentScoreText.text;
        highScoreText.text = PlayerPrefs.GetInt("Highscore")+"";
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [Header("Start Panel")]
    public GameObject startPanel;
    public Button StartButton;
    public GameObject SkinPrefab;
    public Transform SkinContainer;

    public Button audioOnButton;
    public Button audioOffButton;
    public Button InstaButton;
    public Button FbButton;
    public Button RateButton;

    [Header("Game Panel")]
    public GameObject gamePanel;
    public Text currentScoreText;
    public Text ComboText;


    [Header("GameOver Panel")]
    public GameObject gameOverPanel;
    public GameObject missedPanel;
    public Text endScoreText;
    public Text highScoreText;



    [Header("Options")]
    public Slider speedSlider;

    public Text speedText;
    public Slider perfectSlider;
    public Text perfectText;
    public Slider minSlider;
    public Text minText;

    public AnalyticsManager analyticsManager;

    public List<string> messages;




    #region delegates_and_events
    public delegate void AudioToggle(bool toggle);
    public static event AudioToggle OnAudioChange;

    #endregion


    private Animation scoreAnimation;
    private Animation comboAnimation;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {


        // PlayerPrefs.DeleteAll();
        StartButton.onClick.AddListener(() =>
        {
            GameManager.instance.canPlay = true;
            startPanel.SetActive(false);
            gamePanel.SetActive(true);
            analyticsManager.StartEvent();
            var startCount = PlayerPrefs.GetInt("start_count");
            startCount++;
            PlayerPrefs.SetInt("start_count", startCount);
            Debug.LogWarning("Start Game: " + startCount);
        });

        return;
        OnAudioChange(PlayerPrefs.GetInt("audio") == 1);


        audioOnButton.onClick.AddListener(() =>
        {
            OnAudioChange(false);
        });

        audioOffButton.onClick.AddListener(() =>
        {
            OnAudioChange(true);
        });

        InstaButton.onClick.AddListener(() => Application.OpenURL("https://www.instagram.com/_u/nefstergames"));
        FbButton.onClick.AddListener(() => Application.OpenURL("fb://page/NefsterEntertainment"));

        OnAudioChange += ToggleAudioButtons;

    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        OnAudioChange -= ToggleAudioButtons;

    }


    void ToggleAudioButtons(bool b)
    {
        audioOnButton.gameObject.SetActive(b);
        audioOffButton.gameObject.SetActive(!b);
    }


    private void Start()
    {
        // PlayerPrefs.DeleteAll();
        GameManager gm = GameManager.instance;
        speedSlider.value = gm.speedModifier;
        perfectSlider.value = gm.perfDist;
        minSlider.value = gm.minDist;

        speedText.text = gm.speedModifier + "";
        perfectText.text = gm.perfDist + "";
        minText.text = gm.minDist + "";
        speedSlider.onValueChanged.AddListener((v) =>
        {
            speedText.text = v + "";
            gm.speedModifier = v;
        });
        perfectSlider.onValueChanged.AddListener((v) =>
        {
            perfectText.text = v + "";
            gm.perfDist = v;
        });
        minSlider.onValueChanged.AddListener((v) =>
        {
            gm.minDist = v;
            minText.text = v + "";
        });

        scoreAnimation = currentScoreText.GetComponent<Animation>();
        comboAnimation = ComboText.GetComponent<Animation>();

        
        var skins = GameManager.instance.skins;

        foreach(var skin in skins)
        {
            Instantiate(SkinPrefab, SkinContainer).GetComponent<ShopItemUI>().SetUI(skin);
        }


    }


    public void UpdateScore(int score)
    {
        currentScoreText.text = score + "";
        if (scoreAnimation.isPlaying) scoreAnimation.Stop();
        scoreAnimation.Play();
    }
    public void UpdateCombo(int combo)
    {
        var pos = LineBehaviour.lastHitLine.Target.transform.position;
        var dir = pos.x < 0 ? 2 : -2;
        ComboText.transform.position = Camera.main.WorldToScreenPoint(pos + new Vector3(dir, 2, 0));
        // ComboText.transform.position = Input.mousePosition + Vector3.one * 100;
        ComboText.text = combo < 2 ? "" : combo + "x \n" + messages[Random.Range(0, messages.Count)];
        ComboText.transform.eulerAngles = Vector3.forward * Random.Range(-20f, 20f);
        // ComboText.fontSize = 
        // ComboText.text = messages[Random.Range(0, messages.Count)];
        if (comboAnimation.isPlaying) comboAnimation.Stop();
        comboAnimation.Play();
    }

    public void Missed()
    {
        missedPanel.SetActive(true);
    }


    public void OnGameOver()
    {
        missedPanel.SetActive(false);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        endScoreText.text = currentScoreText.text;
        highScoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;



    // public delegate void Click();

    // public event Click OnClick;

    [Range(0, 3)]
    public float minDist;
    [Range(0, 1)]
    public float perfDist;

    [Range(1, 10)]
    public float speed = 1;
    [Range(1, 3)]
    public float scrollDownSpeed = 1;
    [Range(0, .5f)]
    public float speedModifier;
    [Range(0, 0.1f)]
    public float hueModifier;


    int score;

    int highScore;
    int maxHeight;

    int perfectCombo;


    // public GameObject linePrefab;

    public List<SkinObject> skins;
    public List<GameObject> linePrefabs;
    public GameObject textPrefab;

    // public GradientScript gr;
    public SpriteRenderer background;
    public Transform board;

    float spawnHeight = -2;

    int direction = 1;

    public UIController ui;
    public AnalyticsManager analyticsManager;

    Queue<LineBehaviour> path;

    LineBehaviour currentLine;

    ParticleSystem currentParticles;
    Gradient currentGradient;
    internal bool canPlay = false;

    int difficulty = 1;



    // Use this for initialization
    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;

        CurrentSkin = CurrentSkin;
        GradientStep = Random.value;
        highScore = PlayerPrefs.GetInt("Highscore");
        maxHeight = PlayerPrefs.GetInt("MaxHeight");
        //PlayerPrefs.SetInt("MaxHeight",0);
        path = new Queue<LineBehaviour>();
        Initialize();
        currentLine = path.Dequeue();
        // OnClick += Clicked;
    }


    // Update is called once per frame
    void Update()
    {
        if (!canPlay) return;
        PingPong(currentLine.Target.transform);

        if (count > 1)
            ScrollDown();

        if ((Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) && canPlay)
        {
            Clicked();
            // OnClick();
        }

    }


    // LineBehaviour lastLine;

    Color currentColor;
    float GradientStep = 0;
    bool maxHeightMarkerSet = false;
    void SpawnLine(GameObject obj, bool animate = true)
    {
        Transform Line = Instantiate(obj, board).transform;

        currentColor = currentGradient.Evaluate(GradientStep);
        GradientStep += hueModifier;
        GradientStep %= 1;

        Line.name = spawnHeight + " Tile";



        var lineBehaviour = Line.GetComponent<LineBehaviour>();
        lineBehaviour.Activate(spawnHeight, currentColor);

        spawnHeight += 1.5f;

        // if (animate) // ?
        //     lineBehaviour.Target.GetComponent<Animation>().Play();


        if (!maxHeightMarkerSet && spawnHeight > maxHeight && maxHeight > 10)
        {
            lineBehaviour.SetMax(textPrefab, board, spawnHeight);
            maxHeightMarkerSet = true;
        }

        path.Enqueue(lineBehaviour);

    }


    float CheckColorDist(Color a, Color b)
    {
        return Mathf.Abs(((a.r - b.r) + (a.g - b.g) + (a.b - b.b)));
    }
    void PingPong(Transform tr)
    {
        if (tr.position.x >= 3)
        {
            direction = -1;
        }
        if (tr.position.x <= -3)
        {
            direction = 1;
        }
        tr.Translate(Vector3.right * Time.deltaTime * speed * direction);
    }

    private void Clicked()
    {
        float dist = currentLine.IsClose(path.Peek());
        if (dist < minDist)
        {
            if (dist < perfDist)
            {
                OnPerfect();
            }
            else
                perfectCombo = 0;
            OnHit();
        }
        else
        {
            OnMiss();
        }

    }

    // private float down = 1f;

    void ScrollDown()
    {
        var y = Camera.main.WorldToViewportPoint(currentLine.transform.position).y;

        var d = 2f;
        if (y > .4f)
            d = y * 10f;
        // board.transform.position = Vector3.Lerp(board.transform.position, new Vector3(0, -currentLine.transform.localPosition.y + 1, 0), Time.deltaTime);
        // else

        board.Translate(Vector3.down * Time.deltaTime * d);// (currentLine.transform.position.y + scrollDownSpeed));

        // down = Mathf.Lerp(down, 2f, Time.deltaTime * scrollDownSpeed);


        Debug.Log(currentLine.transform.position.y);

        if (currentLine.transform.position.y < -4)
        {
            OnLose();
        }
        else
        {
            NullCombo();
        }

    }
    private int count;

    public SkinObject CurrentSkin
    {
        get
        {
            return skins[PlayerPrefs.GetInt("current_skin")];
        }
        set
        {

        background.sprite = value.background;
        currentGradient = value.gradients[Random.Range(0, value.gradients.Count)];
            PlayerPrefs.SetInt("current_skin", skins.IndexOf(value));
        }
    }

    void OnHit()
    {
        count++;
        // if (currentLine.transform.position.y < 1)
        //     down -= 0.05f;

        scrollDownSpeed *= 1.5f;

        // currentLine.transform.SetParent(path.Peek().transform);// change
        currentLine.Follow(path.Peek().transform.GetComponent<LineBehaviour>().Target.transform);

        // Destroy(currentLine.gameObject, 5);
        currentLine = path.Dequeue();

        currentLine.Hit(path.Peek());


        direction = currentLine.DistanceBetween(path.Peek()) < 0 ? 1 : -1;
        SpawnLine(linePrefabs[Random.Range(0, difficulty)]);
        score += 1 + perfectCombo;

        if (score % 30 == 0 && difficulty < linePrefabs.Count)
            difficulty++;
        if (score > highScore)
        {
            analyticsManager.HighScoreEvent(score);
            PlayerPrefs.SetInt("Highscore", score);
        }
        ui.UpdateScore(score);
        ui.UpdateCombo(perfectCombo);
        speed += speedModifier;
    }
    void OnPerfect()
    {
        currentLine.PlayPerfect();
        // currentLine.Find("Perfect").GetComponent<SpriteRenderer>().color = currentColor;
        // currentLine.Find("Perfect").GetComponent<Animation>().Play();
        // currentLine.Find("Perfect").parent = board.transform;
        // particle.Play();
        //Debug.Log("Perfect");
        perfectCombo++;
    }

    void OnMiss()
    {
        Debug.LogWarning("Missed");

        OnLose();
    }



    // float IsClose(Vector3 a, Vector3 b)
    // {
    //     return Mathf.Abs(a.x - b.x);
    // }

    void Initialize()
    {
        for (int i = -1; i < 10; i++)
        {
            SpawnLine(linePrefabs[0], false);
        }
    }
    void NullCombo()
    {
        // ui.UpdateCombo(perfectCombo = 0);
    }
    void NullScore()
    {
        // ui.UpdateScore(score = 0);
    }

    public void OnLose()
    {
        canPlay = false;
        StartCoroutine(GameOver());
        // ui.OnGameOver();
        // NullCombo();
        // NullScore();
        // speed = 4;
        // while (currentLine.position.y > 0)
        // {
        //     board.Translate(Vector3.down * Time.deltaTime * (currentLine.position.y + scrollDownSpeed));
        // }
        // OnClick -= Clicked;

        if (spawnHeight - 12 > PlayerPrefs.GetInt("MaxHeight"))
            PlayerPrefs.SetInt("MaxHeight", (int)spawnHeight - 12);
        //ui.currentScoreText.text = "LOSER";

    }

    IEnumerator GameOver()
    {
        Time.timeScale = 0.1f;
        // yield return new WaitForSecondsRealtime(1f);
        var cam = Camera.main;
        var grey = Camera.main.GetComponent<GreyScale>();

        grey.enabled = true;
        ui.Missed();
        float time = 0;
        while (time < .9f)
        {
            var tm = Time.deltaTime * 3;
            cam.transform.Rotate(0, 0, -tm * 10);
            time += tm;
            cam.fieldOfView -= tm * 10;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(.5f);
        ui.OnGameOver();

        analyticsManager.GameOverEvent(score);
        Debug.Log("Game Over: " + score + " / " + PlayerPrefs.GetInt("Highscore"));


        // time = 0f;
        // while(time < .9f)
        // {
        //     var tm = Time.unscaledDeltaTime * 5;

        //     cam.transform.Rotate(0,0, tm * 10);
        //     time += tm;
        //     cam.fieldOfView += tm*5;
        //     yield return null;
        // }
        // yield return new WaitForSecondsRealtime(.5f);

        // grey.enabled = false;


        // cam.fieldOfView = 60f;
        // cam.transform.rotation = Quaternion.identity;
        // Time.timeScale = 1f;


    }

    public void ResetHeight()
    {
        PlayerPrefs.SetInt("MaxHeight", 0);
        PlayerPrefs.SetInt("Highscore", 0);

    }



}

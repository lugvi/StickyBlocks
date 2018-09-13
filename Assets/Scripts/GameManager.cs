using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;



    // public delegate void Click();

    // public event Click OnClick;

    [Range(0, 5)]
    public float minDist;
    [Range(0, 5)]
    public float perfDist;

    [Range(1, 100)]
    public float speed = 1;
    [Range(1, 10)]
    public float scrollDownSpeed = 1;
    [Range(0, 1)]
    public float speedModifier;
    [Range(0, 0.1f)]
    public float hueModifier;


    int score;

    int highScore;
    int maxHeight;

    int perfectCombo;


    // public GameObject linePrefab;
    public List<GameObject> linePrefabs;
    public GameObject textPrefab;

    public GradientScript gr;

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


        currentGradient = gr.gradients[Random.Range(0, gr.gradients.Count)];
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
        // if(!canPlay)return;
        PingPong(currentLine.transform);
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

        

        var lineBehaviour = Line.GetComponent<LineBehaviour>();
        lineBehaviour.Activate(spawnHeight, currentColor);

        spawnHeight += 2;

        //if(animate) // ?
            // Target.GetComponent<Animation>().Play();


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

    void ScrollDown()
    {
        if (currentLine.transform.position.y > 0)
        {
            board.Translate(Vector3.down * Time.deltaTime * (currentLine.transform.position.y + scrollDownSpeed));
        }
        else
        {
            NullCombo();
        }

    }

    void OnHit()
    {
        currentLine.transform.SetParent(path.Peek().transform);
        // Destroy(currentLine.gameObject, 5);
        currentLine = path.Dequeue();

        currentLine.GetComponent<LineBehaviour>().Hit(path.Peek().currentColor);


        direction =  currentLine.DistanceBetween(path.Peek()) < 0 ? 1 : -1;
        SpawnLine(linePrefabs[Random.Range(0, difficulty)]);
        score += 1 + perfectCombo;

        if(score % 30 == 0 && difficulty < linePrefabs.Count)
            difficulty ++;
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
        for (int i = -1; i < 5; i++)
        {
            SpawnLine(linePrefabs[0],false);
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
        Time.timeScale = 0f;
        // yield return new WaitForSecondsRealtime(1f);
        var cam = Camera.main;
        // var grey = Camera.main.GetComponent<GreyScale>();
        ui.OnGameOver();

        // grey.enabled = true;
        float time = 0;
        while(time < .9f)
        {
            var tm = Time.unscaledDeltaTime*3;
            cam.transform.Rotate(0,0, -tm * 10);
            time += tm;
            cam.fieldOfView -= tm*5;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(.5f);

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

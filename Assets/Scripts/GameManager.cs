using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }



    public delegate void Click();

    public event Click OnClick;

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


    public GameObject linePrefab;
    public GameObject textPrefab;

    public GradientScript gr;

    public Transform board;

    float spawnHeight = -2;

    int direction = 1;

    public UIController ui;
    Queue<Transform> path;

    Transform currentLine;

    ParticleSystem currentParticles;
    // Use this for initialization
    void Start()
    {
        GradientStep = Random.value;
        highScore = PlayerPrefs.GetInt("Highscore");
        maxHeight = PlayerPrefs.GetInt("MaxHeight");
        //PlayerPrefs.SetInt("MaxHeight",0);
        path = new Queue<Transform>();
        Initialize();
        currentLine = path.Dequeue();
        OnClick += Clicked;
    }

    // Update is called once per frame
    void Update()
    {
        PingPong(currentLine);
        ScrollDown();

        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

    }


    Transform lastLine;

    Color currentColor;
    float GradientStep=0;
    bool maxHeightMarkerSet = false;
    void SpawnLine(GameObject obj)
    {
        Transform Line = Instantiate(obj, board).transform;
        Vector3 spawnPos = Vector3.up * spawnHeight + Vector3.left * Random.Range(-3f, 3f);
        Line.localPosition = spawnPos;
        spawnHeight += 2;

        currentColor = gr.gradient.Evaluate(GradientStep);
        GradientStep += hueModifier; 
        GradientStep %=1;


        Transform Target = Line.Find("Target");
        Target.GetComponent<SpriteRenderer>().color = currentColor;
        

        if (!maxHeightMarkerSet && spawnHeight > maxHeight)
        {
            Transform maxHeightLine = Line.Find("BottomBorder");
            maxHeightLine.GetComponent<SpriteRenderer>().color = Color.yellow;
            GameObject TextMesh = Instantiate(textPrefab, board);
            TextMesh.GetComponent<TextMesh>().text = spawnHeight + "";
            TextMesh.transform.position = maxHeightLine.position;

            maxHeightMarkerSet = true;
        }
        if (path.Count > 0 && IsClose(Line.position, lastLine.position) < minDist)
        {
            if (IsClose(Line.position, Vector3.zero) > minDist)
            {
                Line.position = new Vector3(Line.position.x * -1, Line.position.y, Line.position.z);
            }
            else
            {
                Line.Translate(Vector3.left * (Random.value < 0.5f ? 1 : -1));
            }
        }
        path.Enqueue(Line);
        lastLine = Line;

    }


  

    float CheckColorDist(Color a, Color b)
    {
        return Mathf.Abs(((a.r-b.r)+(a.g-b.g)+(a.b-b.b)));
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
        float dist = IsClose(currentLine.position, path.Peek().position);
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
        if (currentLine.position.y > 0)
        {
            board.Translate(Vector3.down * Time.deltaTime * (currentLine.position.y + scrollDownSpeed));
        }
        else
        {
            NullCombo();
        }

    }

    void OnHit()
    {
        currentLine.SetParent(path.Peek());
        Destroy(currentLine.gameObject, 5);
        currentLine = path.Dequeue();
        currentLine.Find("Particle System").GetComponentInChildren<ParticleSystem>().Play();

        direction = currentLine.position.x - path.Peek().position.x < 0 ? 1 : -1;
        SpawnLine(linePrefab);
        score += 1 + perfectCombo;
        if (score > highScore)
        {
            PlayerPrefs.SetInt("Highscore", score);
        }
        ui.UpdateScore(score);
        ui.UpdateCombo(perfectCombo);
        speed += speedModifier;
    }
    void OnPerfect()
    {
        //Debug.Log("Perfect");
        perfectCombo++;
    }

    void OnMiss()

    {
        OnLose();
    }



    float IsClose(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x);
    }

    void Initialize()
    {
        for (int i = -1; i < 5; i++)
        {
            SpawnLine(linePrefab);
        }
    }
    void NullCombo()
    {
        ui.UpdateCombo(perfectCombo = 0);
    }
    void NullScore()
    {
        ui.UpdateScore(score = 0);
    }

    void OnLose()
    {
        ui.OnGameOver();
        NullCombo();
        NullScore();
        speed = 4;
        while (currentLine.position.y > 0)
        {
            board.Translate(Vector3.down * Time.deltaTime * (currentLine.position.y + scrollDownSpeed));
        }
        OnClick -= Clicked;

        if (spawnHeight - 12 > PlayerPrefs.GetInt("MaxHeight"))
            PlayerPrefs.SetInt("MaxHeight", (int)spawnHeight - 12);
        //ui.currentScoreText.text = "LOSER";

    }

    public void ResetHeight()
    {
        PlayerPrefs.SetInt("MaxHeight", 0);
        PlayerPrefs.SetInt("Highscore", 0);

    }



}

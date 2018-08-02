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
    [Range(0,1)]
    public float speedModifier;

    int score;

    int perfectCombo;

    public GameObject prefab;

    public Transform board;

    float spawnHeight = -2;

    int direction = 1;

    public UIController ui;
    Queue<Transform> path;

    Transform currentLine;
    // Use this for initialization
    void Start()
    {
        path = new Queue<Transform>();
        Initialize();
        currentLine = path.Dequeue();
        OnClick += () =>
        {
            Clicked();
        };
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
    void SpawnLine(GameObject obj)
    {
        Transform a = Instantiate(obj, board).transform;
        Vector3 spawnPos = Vector3.up * spawnHeight + Vector3.left * Random.Range(-3f, 3f);
        a.localPosition = spawnPos;
        spawnHeight += 2;
        if (path.Count > 0 && IsClose(a.position, lastLine.position) < minDist)
        {
            if (IsClose(a.position, Vector3.zero) > minDist)
            {
                a.position = new Vector3(a.position.x * -1, a.position.y, a.position.z);
            }
            else
            {
                a.Translate(Vector3.left * (Random.value < 0.5f ? -minDist : minDist));
            }
        }
        path.Enqueue(a);
        lastLine = a;
    }

    void PingPong(Transform tr)
    {
        if (tr.position.x >= 3)
        {
            swapDirection();
        }
        if (tr.position.x <= -3)
        {
            swapDirection();
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
        //direction = Random.value < .5f ? 1 : -1;
        direction = currentLine.position.x - path.Peek().position.x < 0 ? 1 : -1;
        SpawnLine(prefab);
        score += 1 + perfectCombo;
        ui.UpdateScore(score);
        ui.UpdateCombo(perfectCombo);
        speed += speedModifier;
    }
    void OnPerfect()
    {
        Debug.Log("Perfect");
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
            SpawnLine(prefab);
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
        NullCombo();
        NullScore();
        speed = 4;
        while(currentLine.position.y > 0)
        {
            board.Translate(Vector3.down * Time.deltaTime * (currentLine.position.y + scrollDownSpeed));
        }   

        ui.scoreText.text = "LOSER";

    }

    void swapDirection()
    {
        direction *= -1;
        OnLose();
    }
}

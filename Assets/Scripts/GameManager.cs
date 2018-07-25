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

    [Range(1, 10)]
    public float speed = 1;
    public Transform Player;

    public GameObject prefab;

    public Transform board;

    float spawnHeight = 12;

    int direction = 1;

    public List<Transform> startLines;

    public Queue<Transform> path;

    public Transform currentLine;
    // Use this for initialization
    void Start()
    {
        path = new Queue<Transform>();
        foreach (Transform t in startLines)
        {
            path.Enqueue(t);
        }
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



    void SpawnLine(GameObject obj)
    {
        Transform a = Instantiate(obj, board).transform;
        a.localPosition = Vector3.up * spawnHeight;
        path.Enqueue(a);
        a.Translate(Vector3.left * Random.Range(-3f, 3f));
    }

    void PingPong(Transform tr)
    {
        if (tr.position.x >= 3)
            direction = -1;
        if (tr.position.x <= -3)
            direction = 1;
        tr.Translate(Vector3.right * Time.deltaTime * speed * direction);
    }

    private void Clicked()
    {
        float dist = IsClose(currentLine.position, path.Peek().position);
        if (dist < minDist)
        {
            OnHit();
            if (dist < perfDist)
            {
                OnPerfect();
            }
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
            board.Translate(Vector3.down * Time.deltaTime * (currentLine.position.y+1));
        }

    }

    void OnHit()
    {
		currentLine.SetParent(path.Peek());
        Destroy(currentLine.gameObject, 5);
        currentLine = path.Dequeue();
        direction = Random.value < .5f ? 1 : -1;
        spawnHeight += 2;
        SpawnLine(prefab);

    }
    void OnPerfect()
    {
        Debug.Log("Perfect");
    }

    void OnMiss()

    {

    }



    float IsClose(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x);
    }
}

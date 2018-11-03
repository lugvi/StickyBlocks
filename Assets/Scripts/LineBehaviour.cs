using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{

    public SpriteRenderer TopBorder;
    public SpriteRenderer Target;
    // public SpriteRenderer Part;
    public ParticleSystem GoodParticle;

    protected float bounds = 3;

    protected static LineBehaviour lastSpawnLine;
    public static LineBehaviour lastHitLine;

    public Color currentColor;

    // private Vector3 startScale;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // startScale = this.transform.localScale;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // TopBorder.transform.position = new Vector3(0, TopBorder.transform.position.y, TopBorder.transform.position.z);

        var normPos = Mathf.Abs(.5f - Camera.main.WorldToViewportPoint(this.transform.position).y);
        TopBorder.color = new Color(1, 1, 1, 1 - normPos * 1f);
        // this.transform.localScale = new Vector3(startScale.x * (1 - normPos * normPos), startScale.y, startScale.z);

        if (transform.position.y < -5)
            Destroy(gameObject);
    }

    public void Follow(Transform target)
    {
        // Target.transform.parent = null;
        Target.gameObject.AddComponent<SnakeFollow>().SetTarget(target, 10);
    }

    public virtual void Activate(float spawnHeight, Color currentColor)
    {
        Vector3 spawnPos = Vector3.up * spawnHeight;
        this.transform.localPosition = spawnPos;
        this.Target.transform.localPosition = Vector3.left * Random.Range(-bounds, bounds);

        Target.color = currentColor;
        this.currentColor = currentColor;


        // GameManager.instance.FixLine(this.transform);


        // if (lastSpawnLine && IsClose(lastSpawnLine) < 1f)
        // {
        //     float d = DistanceBetween(lastSpawnLine);
        //     Debug.Log(d);
        //     transform.Translate(Vector2.right * d);
        //     // }
        //     // if (transform.position.x < 0)
        //     // {
        //     //     transform.Translate(Vector3.right*2);
        //     //     // transform.position = new Vector3(transform.position.x * -1, transform.position.y, transform.position.z);
        //     // }
        //     // else
        //     // {
        //     //     transform.Translate(Vector3.left*2);
        //     //     // transform.Translate(Vector3.left * (Random.value < 0.5f ? 1 : -1));
        //     // }
        // }


        if (lastSpawnLine && IsClose(lastSpawnLine) < 1f)
        {
            if (IsClose(Vector3.zero) > 1f)
            {
                this.Target.transform.position = new Vector3(this.Target.transform.position.x * -1, this.Target.transform.position.y, this.Target.transform.position.z);
            }
            else
            {
                this.Target.transform.Translate(Vector3.left * (Random.value < 0.5f ? 1 : -1));
            }

        }
        lastSpawnLine = this;

        if (lastHitLine == null)
            lastHitLine = this;

    }

    public void SetMax(GameObject textPrefab, Transform board, float spawnHeight)
    {
        TopBorder.color = Color.yellow;
        GameObject TextMesh = Instantiate(textPrefab, board);
        TextMesh.GetComponent<TextMesh>().text = (int)spawnHeight + "";
        TextMesh.transform.position = TopBorder.transform.position;
    }

    public virtual void Hit(LineBehaviour next)
    {
        if (lastHitLine is LineWithWalls)
        {
            LineWithWalls w = (LineWithWalls)lastHitLine;
            w.HideWalls();
        }

        var col = GoodParticle.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(currentColor, 0.5f), new GradientColorKey(next.currentColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;
        GoodParticle.Play();

        // Debug.Log( lastHitLine.transform.name + " => " + this.transform.name);
        // this.Part.gameObject.SetActive(true);

        // var dis = this.Target.transform.position.x - lastHitLine.Target.transform.position.x;
        // Debug.Log(dis);
        // this.Part.transform.localScale = new Vector3(Mathf.Abs(dis), 1, 1);
        // this.Part.transform.Translate((dis > 0 ? Vector2.right : Vector2.left) * (1-Mathf.Abs(dis))/2);

        // this.Target.transform.localScale = new Vector3(1-Mathf.Abs(dis), 1, 1);
        // this.Target.transform.Translate((dis < 0 ? Vector2.right : Vector2.left) * Mathf.Abs(dis)/2);

        lastHitLine = this;

    }

    public void PlayPerfect()
    {
        StartCoroutine(PerfectAnim(TopBorder));
    }

    public float IsClose(LineBehaviour b)
    {
        return Mathf.Abs(DistanceBetween(b));
    }

    public float IsClose(Vector3 pos)
    {
        return Mathf.Abs(DistanceBetween(pos));
    }

    public float DistanceBetween(LineBehaviour b)
    {
        return this.Target.transform.position.x - b.Target.transform.position.x;
    }

    private float DistanceBetween(Vector3 pos)
    {
        return this.Target.transform.position.x - pos.x;
    }




    IEnumerator PerfectAnim(SpriteRenderer line)
    {
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(currentColor, 0.5f), new GradientColorKey(Color.white, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 1.0f) });
        float time = 0;
        while (time < .9f)
        {
            line.color = grad.Evaluate(time);
            time += Time.deltaTime * 3;
            yield return null;
        }
        // line.color = Color.white;
    }



}

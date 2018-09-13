using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{

    public SpriteRenderer TopBorder;
    public SpriteRenderer Target;
    public ParticleSystem GoodParticle;

    protected float bounds = 3;

    protected static LineBehaviour lastSpawnLine;
    public static LineBehaviour lastHitLine;

    public Color currentColor;


    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(transform.position.y < -5)
            Destroy(gameObject);
    }



    public virtual void Activate(float spawnHeight, Color currentColor)
    {
        Vector3 spawnPos = Vector3.up * spawnHeight + Vector3.left * Random.Range(-bounds, bounds);
        this.transform.localPosition = spawnPos;

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
                transform.position = new Vector3(transform.position.x * -1, transform.position.y, transform.position.z);
            }
            else
            {
                transform.Translate(Vector3.left * (Random.value < 0.5f ? 1 : -1));
            }

        }
        lastSpawnLine = this;
    }

    public void SetMax(GameObject textPrefab, Transform board, float spawnHeight)
    {
        TopBorder.color = Color.yellow;
        GameObject TextMesh = Instantiate(textPrefab, board);
        TextMesh.GetComponent<TextMesh>().text = spawnHeight + "";
        TextMesh.transform.position = TopBorder.transform.position;
    }

    public virtual void Hit(Color next)
    {
        if (lastHitLine is LineWithWalls)
        {
            LineWithWalls w = (LineWithWalls)lastHitLine;
            w.HideWalls();
        }
        lastHitLine = this;

        var col = GoodParticle.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(currentColor, 0.5f), new GradientColorKey(next, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;
        GoodParticle.Play();

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
        return this.transform.position.x - b.transform.position.x;
    }

    private float DistanceBetween(Vector3 pos)
    {
        return this.transform.position.x - pos.x;
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
        line.color = Color.white;
    }



}

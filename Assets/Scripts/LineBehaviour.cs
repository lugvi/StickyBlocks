using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBehaviour : MonoBehaviour
{

    public SpriteRenderer TopBorder;
    public SpriteRenderer Target;
    public ParticleSystem GoodParticle;

    protected float bounds = 3;

    protected static LineBehaviour lastLine;



    public virtual void Activate(float spawnHeight, Color currentColor)
    {
        Vector3 spawnPos = Vector3.up * spawnHeight + Vector3.left * Random.Range(-bounds, bounds);
        this.transform.localPosition = spawnPos;

        Target.color = currentColor;


        GameManager.instance.FixLine(this.transform);


    }

    public void SetMax(GameObject textPrefab, Transform board, float spawnHeight)
    {
        TopBorder.color = Color.yellow;
        GameObject TextMesh = Instantiate(textPrefab, board);
        TextMesh.GetComponent<TextMesh>().text = spawnHeight + "";
        TextMesh.transform.position = TopBorder.transform.position;
    }

    public virtual void Hit(Color current, Color next)
    {
        if (lastLine is LineWithWalls)
        {
            LineWithWalls w = (LineWithWalls)lastLine;
            w.CheckWallCollision = false;
        }
        lastLine = this;

        var col = GoodParticle.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(current, 0.5f), new GradientColorKey(next, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;
        GoodParticle.Play();

    }


}

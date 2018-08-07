using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLine : LineBehaviour
{


    public int maxSpeed;


    public override void Activate(float spawnHeight, Color currentColor)
    {
        base.Activate(spawnHeight, currentColor);
        StartCoroutine(MoveTarget());

    }

    IEnumerator MoveTarget()
    {
		float speed = Random.Range(1, maxSpeed);
		int direction = 1;
        while (true)
        {
            if (this.transform.position.x >= 3)
            {
                direction = -1;
            }
            if (this.transform.position.x <= -3)
            {
                direction = 1;
            }
            this.transform.Translate(Vector3.right * Time.deltaTime * speed * direction);
			yield return null;
        }

    }

	public override void Hit(Color next){
		base.Hit(next);
		StopAllCoroutines();
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeFollow : MonoBehaviour
{

    private Transform target;
	private float speed;

    public void SetTarget(Transform target, float speed)
	{
		this.target = target;
		this.speed = speed;
	}


    void Update()
    {
        if(target)
            this.transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, target.position.x, Time.deltaTime * speed), target.position.y - 1.5f, 0);
        // if()
    }
}

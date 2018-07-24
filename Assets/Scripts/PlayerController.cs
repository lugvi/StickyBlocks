using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


	private void OnTriggerEnter2D(Collider2D other) {
		if(IsClose(other.transform.position,this.transform.position)<GameManager.instance.perfDist)
		{
			GameManager.instance.Advance(other);
			Debug.Log("perfect");
		}
		else if(IsClose(other.transform.position,this.transform.position)<GameManager.instance.minDist)
		{
			GameManager.instance.Advance(other);
			Debug.Log("გუდ");
		}
		else
			Debug.Log("ბანძ");
	}

	

	float IsClose(Vector3 a, Vector3 b)
	{
		return Mathf.Abs(a.x-b.x);
	}
}

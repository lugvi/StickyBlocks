using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[Range(0,5)]
	public float minDist;
	[Range(0,5)]
	public float perfDist;

	public static GameManager instance;
	private void Awake() {
		instance = this;
	}
	public Transform Player;

	[Range(1,300)]
	public float speed=1;

	Camera cam;
	public List<Transform> movables;
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		movables.Add(Player);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0))
		{	
			Vector3 clickpos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,cam.transform.position.z);
			MoveObjectsOnX(movables,Camera.main.ScreenToWorldPoint(clickpos).x);
		}
		movePlayer();
	}

	void MoveObjectsOnX(List<Transform> objects,float x)
	{	
		foreach(Transform t in objects)
		{
			t.transform.position = new Vector3(-x,t.transform.position.y,t.transform.position.z);
		}
	}

	void movePlayer()
	{
		Player.transform.Translate(Vector3.up*Time.deltaTime*speed);
	}

	public void Advance(Collider2D duh)
	{
		duh.transform.SetParent(movables[0]);
	}
}

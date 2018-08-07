using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVChanger : MonoBehaviour {


	private Camera cam;

	private float m_ScreenWidth = 1080;


	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		cam.orthographicSize = cam.orthographicSize * (9/16) / cam.aspect;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

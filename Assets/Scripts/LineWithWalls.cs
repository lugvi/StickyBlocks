﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineWithWalls : LineBehaviour {

	public GameObject Wall;

	public bool left;
	public bool right;

	private GameObject leftWall;
	private GameObject rightWall;
    internal bool CheckWallCollision;


    public override void Activate(float spawnHeight, Color currentColor)
    {
		bounds = 2f;
        base.Activate(spawnHeight, currentColor);
		if(left){
        	leftWall = Instantiate(Wall);
			leftWall.transform.position = new Vector2(-3f, this.transform.position.y);
			leftWall.transform.SetParent(this.transform, true);
		}
		if(right){
        	rightWall = Instantiate(Wall);
			rightWall.transform.Rotate(0, 180, 0);
			rightWall.transform.position = new Vector2(3f, this.transform.position.y);
			rightWall.transform.SetParent(this.transform, true);
		}

		CheckWallCollision = true;

    }


	void Update()
	{
		if(CheckWallCollision && CheckWall() )
		{
			CheckWallCollision = false;
			GameManager.instance.OnLose();
		}
			
	}

	bool CheckWall(){
		if(left && transform.position.x < -3)
			return true;
		if(right && transform.position.x > 3)
			return true;
		return false;
	}

	public override void Hit(Color current, Color next)
	{

		base.Hit(current, next);

		if(left)
			leftWall.transform.SetParent(this.transform.parent, true);
		if(right)
			rightWall.transform.SetParent(this.transform.parent, true);
			// rightWall.transform.parent = this.transform.parent;
	}


}

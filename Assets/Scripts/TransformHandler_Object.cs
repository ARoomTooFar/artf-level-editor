﻿using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This class attaches to objects, and lets the rotate
//by 90 degree increments, for when the user clicks the 
//rotation arrow.
public class TransformHandler_Object : MonoBehaviour {
	public GameObject thing; //object we're messing with
	public Vector3 objectRotation = new Vector3(); //for holding this object's rotation
//	public TileMap tileMap;
	float yRot;

	void Start () {
		//Initialize the object's rotation to 90 degrees
		objectRotation = thing.transform.rotation.eulerAngles;
		objectRotation.x = 0f;
		objectRotation.z = 0f;
		objectRotation.y = 90f;

		//we work in euler angles because Quaternions are tarded
		thing.transform.rotation = Quaternion.Euler(objectRotation);
	
	}

	void Update () {
		//continually update the object's rotation, so it is locked in
		//position and doesn't spin around all crazy
		thing.transform.eulerAngles = objectRotation;

	}

	//This function increments the object's rotation by 90 degrees
	public void rotateObject(float deg){
		objectRotation.x = 0f;
		objectRotation.z = 0f;
		objectRotation.y += deg;
	}

	public void snapObjectToGrid(Vector3 vec){

	}


}

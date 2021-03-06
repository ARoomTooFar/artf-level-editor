﻿using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;

//This class is for keeping the UI that is attached to an object in world
//oriented in the direction of the camera.
public class TransformHandler_ItemObjectUI : MonoBehaviour 
{
//	public GameObject ItemObject; //In-world object this little UI thing is sticking to
//	Camera cam; //Camera to make the UI face (must mimic its rotation)
//	public Canvas objectUICanvas; //so we can access the Event Camera field in the Object UI Canvas
//
////	public GameObject bounding; //the transform of hoveroverthing used to create the bounding box
//	
//	void Start(){
//		//must set these fields programmatically, because when instantiating a prefab,
//		//these fields do not work if things are drag-and-dropped
//		cam = GameObject.Find("UICamera").camera;
////		objectUICanvas = this.gameObject.GetComponent("Canvas") as Canvas;
//		objectUICanvas.worldCamera = cam; //required in order for buttons on canvas to react to clicks
//	}
//
//	//using LateUpdate here rather than Update prevents the object UI
//	//from jittering, which started happening after out first merge.
//	void LateUpdate () 
//	{
//
//		//Set the UI's position to the object's position
//		Vector3 p = new Vector3();
//		p = ItemObject.transform.position;
//		objectUICanvas.transform.position = p; 
//
//		//Set the UI's rotation to the camera's rotation.
//		//This makes it so the UI is always facing the camera
//		p = cam.transform.rotation.eulerAngles;
//		objectUICanvas.transform.rotation = Quaternion.Euler(p);
//
//		//set the bounding box's size to be properly adjusted
//		//bounding.GetComponent<RectTransform>().sizeDelta = new Vector2 (thing.transform.localScale.x * 150, thing.transform.localScale.y * 150);
//
//	}
}

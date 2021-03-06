﻿using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ItemObject : MonoBehaviour
{
	public LayerMask draggingLayerMask;
	static Camera UICamera;
	bool inMouseCheck = false;
	Vector3 initMousePos;
	//static ItemClass itemClass = new ItemClass ();
	TileMapController tilemapcont;
	float mouseDeadZone = 10f;
	Shader focusedShader;
	Shader nonFocusedShader;
	Vector3 newp;

	
	Vector3 rotation;
	//Vector3 position;
	
	void Start ()
	{
		UICamera = GameObject.Find ("UICamera").GetComponent<Camera>();
		tilemapcont = GameObject.Find ("TileMap").GetComponent("TileMapController") as TileMapController;
		
		focusedShader = Shader.Find ("Transparent/Bumped Diffuse");
		nonFocusedShader = Shader.Find ("Bumped Diffuse");
		
		this.gameObject.GetComponent<Renderer>().material.shader = nonFocusedShader;
	}
	
	void Update ()
	{
		if (!Input.GetMouseButtonDown (0) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject () == true) 
			return;
		
		Ray ray = UICamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit; 
		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			
			//check for tilemap so we don't try to drag it
			if (hit.collider.gameObject.name != "TileMap" 
			    && hit.collider.gameObject.GetInstanceID () == this.gameObject.GetInstanceID ()) {
				if (inMouseCheck == false) {
					initMousePos = Input.mousePosition;
					inMouseCheck = true;
				}
				StartCoroutine (DragObject (hit.distance));
				
			}
		}
	}
	
	IEnumerator DragObject (float distance)
	{ 
		//for the ghost-duplicate
		GameObject itemObjectCopy = null;
		ItemObject copy = null;
		
		bool cancellingMove = false;
		bool outOfDeadZone = false;
		bool copyCreated = false;
		newp = this.gameObject.transform.position;
		
		while (Input.GetMouseButton(0)) { 
			
			//if mouse left deadzone, and we haven't made a copy of the object yet
			if (outOfDeadZone && !copyCreated) {
				//create copy of item object
				itemObjectCopy = Instantiate (this.gameObject) as GameObject;
				copy = itemObjectCopy.GetComponent ("ItemObject") as ItemObject;
				copy.changePosition (getPosition ());
				copy.changeOrientation (getRotation ());
				
				//so this code only happens once
				copyCreated = true;
			}
			
			//if user wants to cancel the drag
			if (Input.GetKeyDown (KeyCode.Escape) || Input.GetMouseButton (1)) {
				Destroy (itemObjectCopy);
				cancellingMove = true;
				
				//break out of while loop
				break;
			}
			
			Ray ray = UICamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;
			
			Vector3 mouseChange = initMousePos - Input.mousePosition;
			
			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, draggingLayerMask)) {
				if (hitInfo.collider.gameObject.name == "TileMap") {
					int x = Mathf.RoundToInt (hitInfo.point.x / tilemapcont.tileSize);
					int z = Mathf.RoundToInt (hitInfo.point.z / tilemapcont.tileSize);
					
					//if mouse left deadzone
					if (Math.Abs (mouseChange.x) > mouseDeadZone 
					    || Math.Abs (mouseChange.y) > mouseDeadZone 
					    || Math.Abs (mouseChange.z) > mouseDeadZone) {
						
						outOfDeadZone = true;
						
						//for now y-pos remains as prefab's default.
						newp = new Vector3 (x * 1.0f, getPosition ().y, z * 1.0f);

						//if copy exists
						if (copyCreated) {
							//update the item object things
							//shader has to be set in this loop, or transparency won't work
							itemObjectCopy.gameObject.GetComponent<Renderer>().material.shader = focusedShader;
							Color trans = itemObjectCopy.gameObject.GetComponent<Renderer>().material.color;
							trans.a = 0.5f;
							itemObjectCopy.gameObject.GetComponent<Renderer>().material.SetColor ("_Color", trans);
							itemObjectCopy.transform.position = new Vector3 (x, getPosition ().y, z);
							itemObjectCopy.transform.eulerAngles = getRotation();
	
						}
						
					}
				}
				
			}
			yield return null; 
		}
		
		//destroy the copy
		Destroy (itemObjectCopy);
		
		//if move was cancelled, we don't perform an update on the item object's position
		if (cancellingMove == true) {
			
		} else {
			Vector3 pos = this.gameObject.transform.root.position;

			MapData.moveMonsterScenery(this.gameObject, pos, newp-pos);
		}
		
		
		inMouseCheck = false;
	}

	
	public void changePosition(Vector3 newPos){
		//position = newPos;
	}
	
	public Vector3 getPosition(){
		return this.gameObject.transform.position;
	}

	public Vector3 getRotation(){
		return this.gameObject.transform.eulerAngles;
	}
	
	public void rotate(float deg){
		rotation.x = 0f;
		rotation.z = 0f;
		rotation.y += deg;
	}
	
	public void changeOrientation(Vector3 newRot){
		rotation = newRot;
	}
	

	public string getName(){
		return this.gameObject.name;
	}
	
	public void setName(string s){
		this.gameObject.name = s;
	}
	
	public GameObject getGameObject(){
		return this.gameObject;
	}
}

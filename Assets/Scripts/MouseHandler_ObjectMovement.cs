﻿using UnityEngine; 
using System.Collections;

//This class handles mouse raycasts to objects for the purpose
//of letting the user drag things around.
public class MouseHandler_ObjectMovement : MonoBehaviour
{ 
	public float maxDistance = 100.0f;
	public float spring = 50.0f;
	public float damper = 5.0f;
	public float drag = 10.0f;
	public float angularDrag = 5.0f;
	public float distance = 0.2f;
	public bool attachToCenterOfMass = false;
	private SpringJoint springJoint;
	public TileMap tileMap;
	public GameObject draggedObject;
	public Camera cam;
	
	void Update ()
	{ 
		if (!Input.GetMouseButtonDown (0)) 
			return; 

		Ray ray = cam.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit; 

			
		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

			//if this script grabs ahold of the TileMap (or presumably
			//any collider that isn't an object meant to be dragged),
			//it will jack shit up. must catch them here.
			if (hit.collider.gameObject.name != "TileMap") {
				draggedObject = hit.collider.gameObject; 
				StartCoroutine (DragObject (hit.distance)); 
			}
		}


	}
	
	IEnumerator DragObject (float distance)
	{ 
		while (Input.GetMouseButton(0)) { 
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;

			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity)) {
				if(hitInfo.collider.gameObject.name == "TileMap"){
					int x = Mathf.RoundToInt (hitInfo.point.x / tileMap.tileSize);
					int z = Mathf.RoundToInt (hitInfo.point.z / tileMap.tileSize);
//					BoxCollider objectBoxCollider = draggedObject.GetComponent("BoxCollider") as BoxCollider;

					//for now we leave the y position as it is in the original prefab.
					//eventually we should use the object's collder's size/position/center,
					//and the object's scale/position (and perhaps also the ground's y value), 
					//in order to calculate what its y value should be.
					draggedObject.transform.position = new Vector3 (x * 1.0f, draggedObject.transform.position.y, z * 1.0f);
				}
			}
			yield return null; 
		}
	}
	
	Camera FindCamera ()
	{ 
		if (camera) 
			return camera;
		else 
			return Camera.main; 
	} 
}
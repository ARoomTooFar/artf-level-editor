﻿using UnityEngine;
using System.Collections;
using System;
using System.IO; 
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; 
using System.Runtime.Serialization;
using System.Text;

//This class listens to save/deploy/load buttons
public class UIHandler_FileIO : MonoBehaviour
{
	public Button Button_Save = null;
	public Button Button_Deploy = null;
	public Button Button_Load = null;
	MouseHandler_TileSelection tileSelection;
	Dictionary<string, Vector3> savedState;
	DataHandler_Items data;
	BinaryWriter bin;
	private StreamWriter writer; // This is the writer that writes to the file
	private string assetText;
	ItemClass itemClass = new ItemClass ();


	void Start ()
	{


		Button_Save.onClick.AddListener (() => {
			saveFile (); });
		Button_Load.onClick.AddListener (() => {
			loadFile ();});


		tileSelection = GameObject.Find ("TileMap").GetComponent ("MouseHandler_TileSelection") as MouseHandler_TileSelection;
		savedState = new Dictionary<string, Vector3> ();

		data = GameObject.Find ("ItemObjects").GetComponent ("DataHandler_Items") as DataHandler_Items;



	}

	public void saveFile ()
	{

		
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create ("Assets/Resources/savedLevel.txt");
		if(itemClass.getItemList ().Count != 0){
			bf.Serialize (file, itemClass.getItemList ());
		}else{
			Debug.Log ("ItemClass.itemList is empty. Nothing to write.");
		}
		file.Close ();

//		if (data.getItemDictionary ().Count != 0) {
//			savedState.Clear ();
//			savedState = new Dictionary<string, Vector3> (data.getItemDictionary ());
//		} else {
//			Debug.Log ("Nothing to save");
//		}
	}

	public void loadFile ()
	{

		if (File.Exists ("Assets/Resources/savedLevel.txt")) {
			data.wipeItemObjects ();
			data.clearItemDictionary ();

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open ("Assets/Resources/savedLevel.txt", FileMode.Open);
//			Debug.Log ((List<ItemClass.ItemStruct>)bf.Deserialize (file));
			List<ItemClass.ItemStruct> savedFile = (List<ItemClass.ItemStruct>)bf.Deserialize (file);
			file.Close ();

			for (int i = 0; i < savedFile.Count; i++) {
				Vector3 pos = new Vector3(savedFile[i].x, savedFile[i].y, savedFile[i].z);
				string name = savedFile[i].item.Substring (0, savedFile[i].item.IndexOf ('_'));
				tileSelection.placeItems (name, pos);
			}
		}else{
			Debug.Log ("savedLevel.txt does not exist. Cannot load.");
		}


		
//		if (savedState.Count != 0) {
//			data.wipeItemObjects ();
//			data.clearItemDictionary ();
//			foreach (KeyValuePair<string, Vector3> entry in savedState) {
//				string key = entry.Key.Substring (0, entry.Key.IndexOf ('_'));
//
//				tileSelection.placeItems (key, entry.Value);
//			}
//		} else {
//			Debug.Log ("Nothing to load");
//		}
	}
		

}

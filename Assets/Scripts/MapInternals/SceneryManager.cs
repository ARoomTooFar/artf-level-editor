using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneryManager {

	Dictionary<string, List<SceneryBlock>> dictionary = new Dictionary<string, List<SceneryBlock>>();

	public SceneryManager() {
	}

	#region (un)linkTerrain
	/*
	 * private bool linkTerrain (SceneryBlock block)
	 * 
	 * Gets the adjacent blocks and adds them as neighbors to block.
	 * 
	 * Returns true if successful.
	 * Returns false if a piece of terrain is already linked to scenery
	 */
	private bool linkTerrain(SceneryBlock blk) {
		TerrainBlock terBlk;
		//for each coordinate this block occupies
		foreach(Vector3 coordinate in blk.Coordinates) {
			//get the terrain block in that position
			terBlk = MapData.Instance.TerrainBlocks.find(coordinate);
			//if there's no block there, this block is not placeable
			if(terBlk == null) {
				return false;
			}
			//try to link the scenery, return false if problem
			if(!terBlk.addScenery(blk)) {
				return false;
			}
		}
		return true;
	}
	
	/*
	 * private void unlinkTerrain(SceneryBlock block)
	 * 
	 * Break the link to this SceneryBlock from associated terrain blocks
	 * 
	 */
	private void unlinkTerrain(SceneryBlock blk) {
		TerrainBlock terBlk;
		//for each coordinate this block occupies
		foreach(Vector3 coordinate in blk.Coordinates) {
			//get the terrain block in that position
			terBlk = MapData.Instance.TerrainBlocks.find(coordinate);
			//if there's no block then... what? continue anyways
			if(terBlk == null) {
				continue;
			}
			//if the block is linked to this piece of scenery, then unlink it
			if(terBlk.Scenery.Equals(blk)) {
				terBlk.removeScenery();
			}
		}
	}
	#endregion (un)linkTerrain

	#region Manipulation
	/*
	 * public bool add (SceneryBlock block)
	 * 
	 * Adds a SceneryBlock to the appropriate list.
	 * Returns true if successful.
	 * Returns false if a block already seems to exist in its position.
	 */
	public bool add(SceneryBlock blk) {
		//attempt to link the scenery to the appropriate terrain
		if(!linkTerrain(blk)) {
			//if something goes wrong, 
			unlinkTerrain(blk);
			return false;
		}
		//get the list for the block type
		List<SceneryBlock> lst = dictionary[blk.BlockInfo.BlockID];
		//create one if needed
		if(lst == null) {
			lst = new List<SceneryBlock>();
			dictionary.Add(blk.BlockInfo.BlockID, lst);
		}
		//add the block to the list
		lst.Add(blk);

		return true;
	}

	#region Remove
	/*
	 * public bool remove (Vector3 position)
	 * 
	 * Remove a piece of scenery from the map data.
	 * 
	 * returns true if the scenery wasn't or is no longer part of the data
	 * returns false if something bad happens
	 */
	public void remove(Vector3 pos) {
		remove(find(pos));
	}

	public void remove(SceneryBlock blk) {
		GameObjectResourcePool.returnResource(blk.BlockInfo.BlockID, blk.GameObj);
		unlinkTerrain(blk);
		dictionary[blk.BlockInfo.BlockID].Remove(blk);
	}
	#endregion Remove

	#region Move
	public void move(Vector3 pos, Vector3 offset) {
		move(find(pos), offset);
	}

	public void move(SceneryBlock blk, Vector3 offset) {
		unlinkTerrain(blk);
		blk.move(offset);
		linkTerrain(blk);
	}
	#endregion Move

	#region Rotate
	public void rotate(Vector3 pos, bool goClockwise = true) {
		rotate(find(pos), goClockwise);
	}

	public void rotate(SceneryBlock blk, bool goClockwise = true) {
		unlinkTerrain(blk);
		blk.rotate(goClockwise);
		linkTerrain(blk);
	}
	#endregion Rotate
	#endregion Manipulation

	
	/*
	 * public SceneryBlock find (Vector3 position)
	 * 
	 * Returns the scenery at position
	 * Returns null if there is no block in that position.
	 */
	public SceneryBlock find(Vector3 pos) {
		//round position
		Vector3 intPosition = pos.Round();
		//for each type of block
		foreach(KeyValuePair<string, List<SceneryBlock>> kvPair in dictionary) {
			//check each block
			foreach(SceneryBlock blk in kvPair.Value) {
				//return block if position matches
				if(blk.Position.Equals(intPosition)) {
					return blk;
				}//otherwise continue to next
			}
		}
		//return null if none found
		return null;
	}

	#region Validation
	#region isRotateValid
	public bool isRotateValid(Vector3 pos, bool goClockwise = true) {
		return isRotateValid(find(pos), goClockwise);
	}

	public bool isRotateValid(SceneryBlock blk, bool goClockwise = true) {
		blk.rotate(goClockwise);
		bool retVal = isBlockValid(blk);
		blk.rotate(!goClockwise);
		return retVal;
	}
	#endregion isRotateValid

	#region isMoveValid
	public bool isMoveValid(Vector3 pos, Vector3 offset) {
		return isMoveValid(find(pos), offset);
	}

	public bool isMoveValid(SceneryBlock blk, Vector3 offset) {
		blk.move(offset);
		bool retVal = isBlockValid(blk);
		blk.move(-offset);
		return retVal;
	}
	#endregion isMoveValid

	public bool isAddValid(string type, Vector3 pos, DIRECTION dir = DIRECTION.North) {
		return isBlockValid(new SceneryBlock(type, pos, dir));
	}

	#region isBlockValid
	public bool isBlockValid(Vector3 pos) {
		return isBlockValid(find(pos));
	}

	public bool isBlockValid(SceneryBlock blk) {
		TerrainBlock terBlk;
		foreach(Vector3 coordinate in blk.Coordinates) {
			//get the terrain block in that position
			terBlk = MapData.Instance.TerrainBlocks.find(coordinate);
			//if there's no block there, this block is not placeable
			if(terBlk == null) {
				return false;
			}
			if(terBlk.Scenery != null && terBlk.Scenery != blk) {
				return false;
			}
		}
		return true;
	}
	#endregion isBlockValid
	#endregion Validation
	
	public string ScenerySaveString {
		get {
			string retVal = "";
			string tempVal;
			foreach(KeyValuePair<string, List<SceneryBlock>> kvPair in dictionary) {
				tempVal = "";
				tempVal += kvPair.Key + ": ";
				foreach(SceneryBlock blk in kvPair.Value) {
					tempVal += blk.SaveString + " ";
				}
				retVal += tempVal + "\n";
			}
			return retVal;
		}
	}

}
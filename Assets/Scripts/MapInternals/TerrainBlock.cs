using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Enum for cardinal and ordinal directions
 * 
 * Directions opposite each other on a compass
 * are set to be negative values of each other to
 * make finding the opposite direction easier.
 * 
 */
public enum DIRECTION : int {
	NonDirectional = 0,
	North = 1,
	South = -(int)North,
	East = 2,
	West = -(int)East,
	NorthEast = 3,
	SouthWest = -(int)NorthEast,
	SouthEast = 4,
	NorthWest = -(int)SouthEast,
};

/*
 * Object to represent one tile/block/terrainspace/thing in the map
 * 
 * Contains links to neighboring blocks as well as the scenery and/or monster on the tile
 * 
 */
public class TerrainBlock {

	#region Properties
	public Dictionary<DIRECTION, TerrainBlock> Neighbors {
		get;
		private set;
	}

	public SceneryBlock Scenery {
		get;
		private set;
	}

	public MonsterBlock Monster {
		get;
		private set;
	}

	public ARTFRoom Room {
		get;
		set;
	}

	public Vector3 Position {
		get;
		set;
	}

	public DIRECTION Orientation {
		get;
		private set;
	}

	public TerrainBlockInfo BlockInfo {
		get;
		private set;
	}

	public string SaveString {
		get{ return Position.toCSV() + "," + Orientation.ToString();}
	}

	public bool Pathable{
		get{ return BlockInfo.Pathable && (Scenery == null?true:Scenery.BlockInfo.Pathable); }
	}

	public GameObject GameObj {
		get;
		private set;
	}
	#endregion Properties

	#region Constructors
	/*
	 * Constructor
	 */
	public TerrainBlock(string blockID, Vector3 pos, DIRECTION dir) {
		this.BlockInfo = TerrainBlockInfo.get(blockID);
		this.Position = pos.Round();
		this.Orientation = dir;
		this.GameObj = GameObjectResourcePool.getResource(blockID, pos, dir.toRotationVector());
	}

	/*
	 * Deep Copy constructor
	 */
	public TerrainBlock(TerrainBlock original){
		this.Neighbors = new Dictionary<DIRECTION, TerrainBlock>(original.Neighbors);
		this.Scenery = original.Scenery;
		this.Monster = original.Monster;
		this.Room = original.Room;
		this.Position = original.Position.Copy();
		this.Orientation = original.Orientation;
		this.BlockInfo = original.BlockInfo;
	}
	#endregion Constructors

	#region Neighbors
	/*
	 * public bool addNeighbor(TerrainBlock that, DIRECTION dir)
	 * 
	 * that becomes a neighboring block to this in direction dir.
	 * 
	 * returns true if successfully added.
	 * returns false if the block already has a neighbor in that direction.
	 * 
	 * Throws exception if dir is NotDirection.
	 * 
	 */
	public bool addNeighbor(TerrainBlock blk, DIRECTION dir) {
		if(dir.Equals(DIRECTION.NonDirectional)) {
			throw new Exception("Invalid DIRECTION");
		}
		try {
			Neighbors.Add(dir, blk);
		} catch(ArgumentException) {
			return false;
		}
		return true;
	}

	/*
	 * public bool removeNeighbor(DIRECTION dir)
	 * 
	 * removes the neighbor in direction dir
	 * 
	 * returns true if successfully removed.
	 * returns false if no neighbor was present.
	 * 
	 */
	public bool removeNeighbor(DIRECTION dir) {
		return Neighbors.Remove(dir);
	}

	/*
	 * public void clearNeighbors()
	 * 
	 * Removes all neighbors from TerrainBlock
	 * 
	 */
	public void clearNeighbors() {
		Neighbors.Clear();
	}

	/*
	 * public TerrainBlock getNeighbor(DIRECTION dir)
	 * 
	 * Returns the neighboring TerrainBlock in direction dir.
	 * Returns null if there is no block in that direction
	 */
	public TerrainBlock getNeighbor(DIRECTION dir) {
		try {
			return Neighbors[dir];
		} catch(Exception) {
			return null;
		}
	}

	/*
	 * public DIRECTION isNeighbor(TerrainBlock other)
	 * 
	 * Returns the DIRECTION in which other lies in relation to this.
	 * Returns DIRECTION.NotNeighbor if other is not adjacent.
	 */
	public DIRECTION isNeighbor(TerrainBlock other) {
		//get difference in position
		float xDif = other.Position.x - this.Position.x;
		float zDif = other.Position.z - this.Position.z;

		//cardinal directions
		if(xDif == 0 && zDif == 1) {
			return DIRECTION.North;
		}
		if(xDif == 0 && zDif == -1) {
			return DIRECTION.South;
		}
		if(xDif == 1 && zDif == 0) {
			return DIRECTION.East;
		}
		if(xDif == -1 && zDif == 0) {
			return DIRECTION.West;
		}

		//ordinal directions
		if(xDif == 1 && zDif == 1) {
			return DIRECTION.NorthEast;
		}
		if(xDif == 1 && zDif == -1) {
			return DIRECTION.SouthEast;
		}
		if(xDif == -1 && zDif == 1) {
			return DIRECTION.NorthWest;
		}
		if(xDif == -1 && zDif == -1) {
			return DIRECTION.SouthWest;
		}

		//default value of NotNeighbor
		return DIRECTION.NonDirectional;
	}
	#endregion Neighbors

	#region Scenery
	/*
	 * public bool addScenery(SceneryBlock scenery)
	 * 
	 * Links this block to a piece of scenery
	 * 
	 * Returns true if successfully linked
	 * Returns false if not.
	 */
	public bool addScenery(SceneryBlock scn) {
		//return false if there is already scenery
		if(this.Scenery != null) {
			return false;
		}

		//if the scenery blocks movement and there is a monster, return false
		if(!scn.BlockInfo.Pathable && this.Monster != null) {
			return false;
		}
		
		this.Scenery = scn;
		return true;
	}

	/*
	 * public void removeScenery()
	 * 
	 * Unlinks the piece of scenery linked to this block
	 */
	public void removeScenery() {
		this.Scenery = null;
	}
	#endregion Scenery

	#region Monster
	/*
	 * public bool addMonster(MonsterBlock monster)
	 * 
	 * Links this block to a monster
	 * 
	 * Returns true if successfully linked
	 * Returns false if not.
	 */
	public bool addMonster(MonsterBlock mon) {
		//return false if there is already a monster linked
		if(this.Monster != null) {
			return false;
		}

		//return false if there is a piece of scenery that blocks pathing
		if(this.Scenery != null && !this.Scenery.BlockInfo.Pathable) {
			return false;
		}
		
		this.Monster = mon;
		return true;
	}

	/*
	 * public void removeMonster()
	 * 
	 * Unlinks the monster linked to this block
	 */
	public void removeMonster() {
		this.Monster = null;
	}
	#endregion Monster

	#region Manipulation
	/*
	 * public void move(Vector3 offset)
	 * 
	 * moves the block and associated scenery and monster
	 */
	public void move(Vector3 offset){
		if(this.Scenery != null && this.Scenery.Position.Equals(this.Position)) {
			this.Scenery.move(offset);
		}

		if(this.Monster != null) {
			this.Monster.move(offset);
		}

		Position += offset;
		GameObj.transform.position = Position;

	}

	public bool changeType(string type){
		GameObjectResourcePool.returnResource(BlockInfo.BlockID, GameObj);
		GameObj = null;
		TerrainBlockInfo nInf = TerrainBlockInfo.get(type);
		if(!nInf.Pathable) {
			if(this.Monster != null){
				return false;
			}
			if(this.Scenery != null){
				return false;
			}
		}
		this.BlockInfo = nInf;
		GameObj = GameObjectResourcePool.getResource(BlockInfo.BlockID, Position, Orientation.toRotationVector());

		return true;
	}

	public void rotate(bool goClockwise = true){
		Orientation = Orientation.QuarterTurn(goClockwise);
		GameObj.transform.eulerAngles = Orientation.toRotationVector();
	}
	#endregion Manipulation
}


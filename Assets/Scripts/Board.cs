﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/**
* The Board class acts like a flat 2D board in the 3D world.
* Board.cs applies to a GameObject with the following effects:
* Width (in number of Tiles) = GameObject.Scale.X
* Height (in number of Tiles) = GameObject.Scale.Y
*/
public class Board : MonoBehaviour
{
    /**
    * width and height use number of Tiles as units.
    */
    private uint numberCamps, numberObstacles;
    private float width, height;
    public bool obstacleControlFlag;
    public Tile[] Tiles { get; private set; }
	public Obstacle[] Obstacles { get; private set; }
	public Camp[] Camps { get; private set; }
    public Dice GetDice { get; private set; }
	public BoardEvent Event{ get; private set; }
    /**
    * This is to be used to create a new Board when the root GameObject does have a terrain component (interpolating instead of taking dimensions as parameters).
    * This is currently used for the main game board.
    */
    public static Board Create(GameObject root, uint tilesWidth, uint tilesHeight)
	{
		if(tilesWidth < 5 && tilesHeight < 5 && (tilesWidth * tilesHeight) < 13)
		{
			Debug.LogError("Board has invalid width/height (tile-space). One of width or height must be at least 5 AND width * height MUST be greater than 13.");
		}
		Board board = root.AddComponent<Board>();
        root.tag = "GameBoard";
        root.name += " (Board)";
        board.GetDice = Dice.Create(board.gameObject.transform.position, new Vector3(), new Vector3(1, 1, 1));
		board.Event = new BoardEvent(board);
		//root.transform.localScale = new Vector3(width, 1, height);
		board.GetWidthInTiles = Convert.ToUInt32(tilesWidth);
		board.GetHeightInTiles = Convert.ToUInt32(tilesHeight);

		// The following code block should probably belong in Board::Start() or Board::Awake...
		// The reason is cannot be in Board::Start() is because Board::Start() is ran once the script is initialised and ready to execute, by the time multiple other scripts would have needed references to these camps etc...
		// The reason Board::Awake() cannot have this code block is because that will execute directly after "root.AddComponent<Board>()" which means before board.GetWidthInTiles is assigned.
		// Thus, the initialisation code MUST happen right here, despite being ugly.

		board.Tiles = new Tile[board.GetWidthInTiles * board.GetHeightInTiles];
		for(uint i = 0; i < board.Tiles.Length; i++)
		{
			float xTile = i % board.GetWidthInTiles;
			float zTile = i / board.GetWidthInTiles;
			board.Tiles[i] = Tile.Create(board, xTile, zTile);
			GameObject tileObject = board.Tiles[i].gameObject;
			Vector2 tileSize = Board.ExpectedTileSize(root, board.GetWidthInTiles, board.GetHeightInTiles);
			tileObject.transform.position = Game.MinWorldSpace(root) + (new Vector3(xTile * tileSize.x, 0, zTile * tileSize.y));
			tileObject.transform.position = new Vector3(tileObject.transform.position.x, Game.InterpolateYWorldSpace(root, tileObject.transform.position), tileObject.transform.position.z);
			tileObject.transform.localScale = new Vector3(tileSize.x, 1, tileSize.y);
			tileObject.name = "Tile " + (i + 1);
		}
        board.Cull();
        board.GetGoalTile().gameObject.GetComponent<Renderer>().material.color = Color.yellow / 1.2f;
        board.numberCamps = 5;
		board.numberObstacles = 13;

		board.Camps = new Camp[board.numberCamps];
        for (uint i = 0; i < board.numberCamps; i++)
        {
            Color color = Color.black;
            switch (i % 5)
            {
                case 0:
                    color = Color.red;
                    break;
                case 1:
                    color = Color.green;
                    break;
                case 2:
                    color = Color.blue;
                    break;
                case 3:
                    color = Color.yellow;
                    break;
                case 4:
                    color = Color.magenta;
                    break;
            }
            board.Camps[i] = Camp.Create(board, board.Tiles[i], color);
        }
		board.Obstacles = new Obstacle[board.numberObstacles];
		for (uint i = 0; i < board.numberObstacles; i++)
			board.Obstacles[i] = Obstacle.Create(board, board.Tiles[i + 30]);
		return board;
	}

    /**
    * This is to be used to create a new Board when the root GameObject has no terrain component (taking dimensions as parameters instead of interpolating).
    * This is currently used for TestBoard.
    */
	public static Board CreateNoTerrain(GameObject root, float width, float height, uint tilesWidth, uint tilesHeight)
	{
		if(tilesWidth < 5 && tilesHeight < 5 && (tilesWidth * tilesHeight) < 13)
		{
			Debug.LogError("Board has invalid width/height (tile-space). One of width or height must be at least 5 AND width * height MUST be greater than 13.");
		}
		Board board = root.AddComponent<Board>();
		root.name = "Board";
		root.transform.localScale = new Vector3(width, 1, height);
		board.GetWidthInTiles = Convert.ToUInt32(tilesWidth);
		board.GetHeightInTiles = Convert.ToUInt32(tilesHeight);

		// The following code block should probably belong in Board::Start() or Board::Awake...
		// The reason is cannot be in Board::Start() is because Board::Start() is ran once the script is initialised and ready to execute, by the time multiple other scripts would have needed references to these camps etc...
		// The reason Board::Awake() cannot have this code block is because that will execute directly after "root.AddComponent<Board>()" which means before board.GetWidthInTiles is assigned.
		// Thus, the initialisation code MUST happen right here, despite being ugly.

		board.Tiles = new Tile[board.GetWidthInTiles * board.GetHeightInTiles];
        Vector2 tileSize = new Vector2(width / tilesWidth, height / tilesHeight);
		for(uint i = 0; i < board.Tiles.Length; i++)
		{
			float xTile = i % board.GetWidthInTiles;
			float zTile = i / board.GetWidthInTiles;
			board.Tiles[i] = Tile.Create(board, xTile, zTile);
			GameObject tileObject = board.Tiles[i].gameObject;
            tileObject.transform.position = new Vector3(xTile * tileSize.x, 0, zTile * tileSize.y);
            tileObject.transform.localScale = new Vector3(tileSize.x, 1, tileSize.y);
			tileObject.name = "Tile " + (i + 1);
		}
		board.numberCamps = 5;
		board.numberObstacles = 13;

		board.Camps = new Camp[board.numberCamps];
        for (uint i = 0; i < board.numberCamps; i++)
        {
            Color color = Color.black;
            switch (i % 4)
            {
                case 0:
                    color = Color.red;
                    break;
                case 1:
                    color = Color.green;
                    break;
                case 2:
                    color = Color.blue;
                    break;
                case 3:
                    color = Color.yellow;
                    break;
                default:
                    color = Color.magenta;
                    break;
            }
            board.Camps[i] = Camp.Create(board, board.Tiles[i], color);
        }
        board.Obstacles = new Obstacle[board.numberObstacles];
		for (uint i = 0; i < board.numberObstacles; i++)
			board.Obstacles[i] = Obstacle.Create(board, board.Tiles[i]);
		return board;
	}

    public Material TileMaterial()
    {
        GameObject temp = Instantiate(Resources.Load("Prefabs/Tile")) as GameObject;
        Material defaultMaterial = temp.GetComponent<Renderer>().material;
        Destroy(temp);
        return defaultMaterial;
    }

	void Start()
    {
        this.ResetTurns();
    }
	
	void Update()
    {
		
	}

    /**
    * Returns the Goal Tile, which will be the tile with the greatest y-coordinate (as it is at the top of the mountain).
    */
    public Tile GetGoalTile()
    {
        Tile max = this.Tiles[0];
        foreach(Tile t in this.Tiles)
        {
            if (max.gameObject.transform.position.y < t.gameObject.transform.position.y)
                max = t;
        }
        return max;
    }

    /**
     * This removes all of the Tiles that do not actually belong to the game board. 
     * This transforms a grid of Tiles to the actual Chimera of Gold game board. Well, it will when it is fully implemented. -Harry
     * 
     * A thousand curses on your head for leaving this to me -Aswin
     */
    public void Cull()
    {
        Tile[] gameTiles = new Tile[138];
        gameTiles[0] = this.GetGoalTile();
        Vector2 goalTileSpace = gameTiles[0].PositionTileSpace;
        /*
        // start going left from goal node.
        for (uint i = 1; i < 7; i++)
            gameTiles[i] = this.GetTileByTileSpace(new Vector2(goalTileSpace.x - i, goalTileSpace.y));
        // now start going right
        for(uint i = 7; i < 14; i++)
            gameTiles[i] = this.GetTileByTileSpace(new Vector2(goalTileSpace.x + (i - 6), goalTileSpace.y));

        gameTiles[14] = this.GetTileByTileSpace(new Vector2(gameTiles[6].PositionTileSpace.x, gameTiles[6].PositionTileSpace.y - 1));
        for (uint i = 15; i < 18; i++)
            gameTiles[i] = this.GetTileByTileSpace(new Vector2(gameTiles[i - 1].PositionTileSpace.x, gameTiles[i - 1].PositionTileSpace.y - 1));
        foreach (Tile t in gameTiles)
            if(t != null)
				t.GetComponent<Renderer>().material.color = new Color(0, 1, 0, this.TileMaterial().color.a);
        */
        foreach (Tile t in gameTiles)
        {
            if (t != null)
                t.GetComponent<Renderer>().material.color = new Color(0, 255, 0, this.TileMaterial().color.a);
        }

    }

    /**
     * Given a Vector2 representing tile-space, returns a Tile if it shares the exact position, or null if no tiles share such position.
     */
    public Tile GetTileByTileSpace(Vector2 positionTileSpace)
    {
        foreach (Tile t in this.Tiles)
            if (t.PositionTileSpace == positionTileSpace)
                return t;
        return null;
    }

    /**
    * Returns true if the tile has an obstacle ontop of it.
    */
    public bool TileOccupiedByObstacle(Tile t)
    {
        return this.TileOccupiedByObstacle(t.PositionTileSpace);
    }

    /**
     * Returns true if the tilePosition contains an obstacle.
     */
    public bool TileOccupiedByObstacle(Vector2 tilePosition)
    {
        foreach (Obstacle obstacle in this.Obstacles)
            if (obstacle.CurrentTile.PositionTileSpace == tilePosition)
                return true;
        return false;
    }

    /**
     * Resets the... well... turns.
     */
    public void ResetTurns()
    {
        this.PlayerTurn = this.Camps[0].TeamPlayers[0];
    }
		
    /**
     * Simulates the end of the current turn and sets Board::PlayerTurn to the "next" player accordingly.
     */
    public void NextTurn()
    {
        this.obstacleControlFlag = false;
		int campId = -1, playerId = -1;
		// Set campId and playerId to the corresponding indices for the current Player
        for(uint campCounter = 0; campCounter < this.Camps.Length; campCounter++)
        {
            // Iterate through all players in all camps, trying to find a match. If a match is found, then set campId and playerId correspondingly.
            for(uint playerCounter = 0; playerCounter < this.Camps[campCounter].TeamPlayers.Length; playerCounter++)
            {
				if(this.Camps[campCounter].TeamPlayers[playerCounter] == this.PlayerTurn)
				{
					campId = (int)campCounter;
					playerId = (int)playerCounter;
				}
            }
        }
		// If neither campId or playerId have changed, then the current player wasnt even found and something is going to go very wrong.
		if(campId == -1 || playerId == -1)
		{
			Debug.LogError("Board::PlayerTurn is not a member of any of its camp members.");
		}
		// Try to increment playerId. If it goes higher than the number of players per camp, increment the camp id and set player id to 0.
		if(++playerId >= Game.PLAYERS_PER_CAMP)
		{
			campId++;
			playerId = 0;
		}
		// If camp id is now greater than the actual number of camps, then we've gone past the final player of the final camp and need to go back to the beginning.
		if(campId >= Game.NUMBER_CAMPS)
			campId = 0;
		this.PlayerTurn = this.Camps[campId].TeamPlayers[playerId];
    }

    public void HighlightTiles(Color color)
    {
        foreach (Tile tile in this.Tiles)
            tile.GetComponent<Renderer>().material.color = color;
    }

    public void RemoveTileHighlights()
    {
        foreach (Tile tile in this.Tiles)
			tile.GetComponent<Renderer>().material = this.TileMaterial();
    }

	/**
	 * Getters for width and height (in units of Tiles and pixels respectively)
	 */
	public uint GetWidthInTiles{get; private set;}
	public uint GetHeightInTiles{get; private set;}
	public float GetWidthInPixels{get{return this.GetWidthInTiles * Board.ExpectedTileSize(this.gameObject, this.GetWidthInTiles, this.GetHeightInTiles).x;}}
	public float GetHeightInPixels{get{return this.GetHeightInTiles * Board.ExpectedTileSize(this.gameObject, this.GetWidthInTiles, this.GetHeightInTiles).y;}}
    public Player PlayerTurn {get; private set;}

    /**
    * Uses the dimensions of the GameObject root parameter and the number of tiles to discern the size that each tile should be.
    * Note: Game.MinWorldSpace and Game.MaxWorldSpace require a Terrain component. Thus, invoking ExpectedTileSize with a root GameObject with no Terrain component WILL throw.
    */
	public static Vector2 ExpectedTileSize(GameObject root, float widthTiles, float heightTiles)
	{
		Vector3 min = Game.MinWorldSpace(root), max = Game.MaxWorldSpace(root);
		float deltaX = max.x - min.x, deltaZ = max.z - min.z;
		return new Vector2(deltaX / widthTiles, deltaZ / heightTiles);
	}
}
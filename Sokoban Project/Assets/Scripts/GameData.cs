using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
   	public int invalidTile;
	public int groundTile;
	public int destinationTile;
	public int heroTile;
	public int ballTile;
    public int [,] levelData;
    public int tileSize;
    int rows;
    int cols;
    int ballCount;
    GameObject hero;
    Dictionary<GameObject, Vector2> occupants;

    public void SetLevelDataStructure(int[,] updatedLevel) {
        levelData = updatedLevel;
    }

    public void SetLevelDimensions(int x, int y) {
        rows = x;
        cols = y;
    }

    public int GetRowCount() {
        return rows;
    }

    public int GetColumnCount() {
        return cols;
    }



}

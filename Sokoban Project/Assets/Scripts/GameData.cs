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

	public Vector2 middleOffset=new Vector2();//offset for aligning the level to middle of the screen

    
    void Start() {
        
    }

    public void SetLevelDataStructure(int[,] updatedLevel) {
        levelData = updatedLevel;
    }

    public void SetLevelDimensions(int x, int y) {
        rows = x;
        cols = y;
    }

    public void SetMiddleOffset() {
        middleOffset.x=cols*tileSize*0.5f-tileSize*0.5f;
		middleOffset.y=rows*tileSize*0.5f-tileSize*0.5f;;
    }
    public int GetRowCount() {
        return rows;
    }

    public int GetColumnCount() {
        return cols;
    }

	public Vector2 getScreenPointFromLevelIndices(int rowInput,int colInput){
		return new Vector2(colInput*tileSize-middleOffset.x,rowInput*-tileSize+middleOffset.y);
	}
    	/*//the reverse methods to find indices from a screen point
	Vector2 GetLevelIndicesFromScreenPoint(float xVal,float yVal){
		return new Vector2((int)(yVal-middleOffset.y)/-tileSize,(int)(xVal+middleOffset.x)/tileSize);
	}
	Vector2 GetLevelIndicesFromScreenPoint(Vector2 pos){
		return GetLevelIndicesFromScreenPoint(pos.x,pos.y);
	}*/


}

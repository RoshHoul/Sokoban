using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
	public string levelName;//name of the text file in resources folder
	
	public Color destinationColor;//destination tile has a different color

	//sprites for different tiles
	public Sprite tileSprite;
	public Sprite heroSprite;
	public Sprite ballSprite;
	int[,] levelData;
	

	//the user input keys
	private int rows;
	private int cols;
	Vector2 middleOffset=new Vector2();//offset for aligning the level to middle of the screen
	int ballCount;//number of balls in level
	GameObject hero;
	Dictionary<GameObject,Vector2> occupants;//reference to balls & hero
	bool gameOver;

	GameData dataContainer;

	void Awake () {
		dataContainer = gameObject.GetComponent<GameData>();
		gameOver=false;
		ballCount=0;
		occupants=new Dictionary<GameObject, Vector2>();
		ParseLevel();//load text file & parse our level 2d array
		CreateLevel();//create the level based on the array
		dataContainer.SetLevelDataStructure(levelData);
		dataContainer.SetLevelDimensions(rows, cols);
	}
	void ParseLevel(){
		TextAsset textFile = Resources.Load (levelName) as TextAsset;
		string[] lines = textFile.text.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);//split by new line, return
		string[] nums = lines[0].Split(new[] { ',' });//split by ,
		rows=lines.Length;//number of rows
		cols=nums.Length;//number of columns
		levelData = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
			string st = lines[i];
            nums = st.Split(new[] { ',' });
			for (int j = 0; j < cols; j++) {
                int val;
                if (int.TryParse (nums[j], out val)){
                	levelData[i,j] = val;
				}
                else{
                    levelData[i,j] = dataContainer.invalidTile;
				}
            }
        }
	}
	void CreateLevel(){
		//calculate the offset to align whole level to scene middle
		middleOffset.x=cols*dataContainer.tileSize*0.5f-dataContainer.tileSize*0.5f;
		middleOffset.y=rows*dataContainer.tileSize*0.5f-dataContainer.tileSize*0.5f;;
		GameObject tile;
		SpriteRenderer sr;
		GameObject ball;
		int destinationCount=0;
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
                int val=levelData[i,j];
				if(val!=dataContainer.invalidTile){//a valid tile
					tile = new GameObject("tile"+i.ToString()+"_"+j.ToString());//create new tile
					tile.transform.localScale=Vector2.one*(dataContainer.tileSize-1);//set tile size
					sr = tile.AddComponent<SpriteRenderer>();
					//assign tile sprite
					sr.sprite=tileSprite;
					//place in scene based on level indices
					tile.transform.position = GetScreenPointFromLevelIndices(i,j);
					if(val==dataContainer.destinationTile){//if it is a destination tile, give different color
						sr.color = destinationColor;
						destinationCount++;//count destinations
					}else{
						if(val==dataContainer.heroTile){//the hero tile
							hero = new GameObject("hero");
							hero.transform.localScale=Vector2.one*(dataContainer.tileSize-1);
							sr = hero.AddComponent<SpriteRenderer>();
							sr.sprite=heroSprite;
							sr.sortingOrder=1;//hero needs to be over the ground tile
							sr.color=Color.red;
							hero.transform.position = GetScreenPointFromLevelIndices(i,j);
							occupants.Add(hero, new Vector2(i,j));//store the level indices of hero in dict
						}else if(val==dataContainer.ballTile){//ball tile
							ballCount++;//increment number of balls in level
							ball = new GameObject("ball"+ballCount.ToString());
							ball.transform.localScale=Vector2.one*(dataContainer.tileSize-1);
							sr = ball.AddComponent<SpriteRenderer>();
							sr.sprite=ballSprite;
							sr.sortingOrder=1;//ball needs to be over the ground tile
							sr.color=Color.black;
							ball.transform.position = GetScreenPointFromLevelIndices(i,j);
							occupants.Add(ball, new Vector2(i,j));//store the level indices of ball in dict
						}
					}
				} 
            }
        }
		if(ballCount>destinationCount)Debug.LogError("there are more balls than destinations");
	}
	
	public Vector2 GetScreenPointFromLevelIndices(int row,int col){
		return new Vector2(col*dataContainer.tileSize-middleOffset.x,row*-dataContainer.tileSize+middleOffset.y);
	}

}
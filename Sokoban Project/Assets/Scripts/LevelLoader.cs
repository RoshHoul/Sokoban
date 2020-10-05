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

	int ballCount;//number of balls in level
	int heroCount;

//	Dictionary<GameObject,Vector2> occupants;//reference to balls & hero
	bool gameOver;

	GameData dataContainer;
	GameManager gameManager;

	void Awake () {
		dataContainer = gameObject.GetComponent<GameData>();
		gameManager = gameObject.GetComponent<GameManager>();
		destinationColor = Color.blue;

		gameOver=false;
		ballCount=0;
//		occupants=new Dictionary<GameObject, Vector2>();
		ParseLevel();//load text file & parse our level 2d array
		dataContainer.SetLevelDataStructure(levelData);
		dataContainer.SetLevelDimensions(rows, cols);
		dataContainer.SetMiddleOffset();
		CreateLevel();//create the level based on the array

	}
	void ParseLevel(){
		TextAsset textFile = Resources.Load (levelName) as TextAsset;
		string[] lines = textFile.text.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);//split by new line, return
		string[] nums;
		int longestLine = 0;
		foreach (string str in lines) {
			nums = str.Split(new[] { ','});
			if (longestLine <= nums.Length) {
				longestLine = nums.Length;
			}
		}
//		string[] nums = lines[0].Split(new[] { ',' });//split by ,

		rows=lines.Length;//number of rows
		cols=longestLine;//number of columns
		levelData = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
			string st = lines[i];
			nums = st.Split(new[] { ',' });
			for (int j = 0; j < cols; j++) {
                int val;
				if (j < nums.Length && nums[j] != ",") {
					if (int.TryParse (nums[j], out val)){
						levelData[i,j] = val;
					} else {
                    	levelData[i,j] = dataContainer.invalidTile;
					}				
				}
                else{
                    levelData[i,j] = dataContainer.invalidTile;
				}
            }
        }
	}
	void CreateLevel(){
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
					sr.sortingOrder=1;
					//place in scene based on level indices
					tile.transform.position = dataContainer.getScreenPointFromLevelIndices(i,j);
					if(val==dataContainer.destinationTile){//if it is a destination tile, give different color
						sr.color = destinationColor;
						sr.sortingOrder=2;
						destinationCount++;//count destinations
					}else{
						if (val==dataContainer.heroTile) {//the hero tile
							heroCount++;
							GameObject hero = new GameObject("hero");
							hero.transform.localScale=Vector2.one*(dataContainer.tileSize-1);
							sr = hero.AddComponent<SpriteRenderer>();
							sr.sprite=heroSprite;
							sr.sortingOrder=3;
							sr.color=Color.red;
							hero.transform.position = dataContainer.getScreenPointFromLevelIndices(i,j);
							
							gameManager.SetupPlayer(hero, new Vector2(i,j));

						} else if (val==dataContainer.ballTile) {
							ballCount++;
							ball = new GameObject("ball"+ballCount.ToString());
							ball.transform.localScale=Vector2.one*(dataContainer.tileSize-1);
							sr = ball.AddComponent<SpriteRenderer>();
							sr.sprite=ballSprite;
							sr.sortingOrder=3;
							sr.color=Color.black;
							ball.transform.position = dataContainer.getScreenPointFromLevelIndices(i,j);
							gameManager.SetupBalls(ball, new Vector2(i,j), ballCount);
						}
					}
				} 
            }
        }



		if(heroCount != 1) {
			Debug.LogError("There must be exactly 1 hero in the config file. Hero is marked by tileID: 2");
		}

		if(ballCount>destinationCount) {
			Debug.LogError("There are more balls than targets. Please ensure there are more targets than balls in the config file. ");
		} 
		
		if (ballCount < 1) {
			Debug.LogError("There must be at least 1 ball in the config file. Balls are marked by tileID: 3");
		}
	}
	


}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public float tileSize;//we are using square tiles tileSize x tileSize
	
	//tile values for different tile types
	public int invalidTile;
	public int groundTile;
	public int destinationTile;
	public int heroTile;
	public int ballTile;
	public int heroOnDestinationTile;
	public int ballOnDestinationTile;

	private int rows;
	private int cols;

	//the user input keys
	public KeyCode[] userInputKeys;//up, right, down, left
	Vector2 middleOffset=new Vector2();//offset for aligning the level to middle of the screen
	int ballCount;//number of balls in level
	GameObject hero;//out triangular hero
	Dictionary<GameObject,Vector2> occupants;//reference to balls & hero
	bool gameOver;

	GameData dataContainer;

	void Start () {
		dataContainer = gameObject.GetComponent<GameData>();
		gameOver=false;
		ballCount=0;
		occupants=new Dictionary<GameObject, Vector2>();
		rows = dataContainer.GetRowCount();
		cols = dataContainer.GetColumnCount();
	}

	void Update(){
		if(gameOver)return;
		ApplyUserInput();//check & use user input to move hero and balls
	}

    private void ApplyUserInput()
    {
        if(Input.GetKeyUp(userInputKeys[0])){
			TryMoveHero(0);//up
		}else if(Input.GetKeyUp(userInputKeys[1])){
			TryMoveHero(1);//right
		}else if(Input.GetKeyUp(userInputKeys[2])){
			TryMoveHero(2);//down
		}else if(Input.GetKeyUp(userInputKeys[3])){
			TryMoveHero(3);//left
		}
    }
    private void TryMoveHero(int direction)
    {
        Vector2 heroPos;
		Vector2 oldHeroPos;
		Vector2 nextPos;
		occupants.TryGetValue(hero,out oldHeroPos);
		heroPos=GetNextPositionAlong(oldHeroPos,direction);//find the next array position in given direction
		
		if(IsValidPosition(heroPos)){//check if it is a valid position & falls inside the level array
			if(!IsOccuppied(heroPos)){//check if it is occuppied by a ball
				//move hero
				RemoveOccuppant(oldHeroPos);//reset old level data at old position
				hero.transform.position=GetScreenPointFromLevelIndices((int)heroPos.x,(int)heroPos.y);
				occupants[hero]=heroPos;
				if(dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]==groundTile){//moving onto a ground tile
					dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]=heroTile;
				}else if(dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]==destinationTile){//moving onto a destination tile
					dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]=heroOnDestinationTile;
				}
			}else{
				//we have a ball next to hero, check if it is empty on the other side of the ball
				nextPos=GetNextPositionAlong(heroPos,direction);
				if(IsValidPosition(nextPos)){
					if(!IsOccuppied(nextPos)){//we found empty neighbor, so we need to move both ball & hero
						GameObject ball=GetOccupantAtPosition(heroPos);//find the ball at this position
						if(ball==null)Debug.Log("no ball");
						RemoveOccuppant(heroPos);//ball should be moved first before moving the hero
						ball.transform.position=GetScreenPointFromLevelIndices((int)nextPos.x,(int)nextPos.y);
						occupants[ball]=nextPos;
						if(dataContainer.levelData[(int)nextPos.x,(int)nextPos.y]==groundTile){
							dataContainer.levelData[(int)nextPos.x,(int)nextPos.y]=ballTile;
						}else if(dataContainer.levelData[(int)nextPos.x,(int)nextPos.y]==destinationTile){
							dataContainer.levelData[(int)nextPos.x,(int)nextPos.y]=ballOnDestinationTile;
						}
						RemoveOccuppant(oldHeroPos);//now move hero
						hero.transform.position=GetScreenPointFromLevelIndices((int)heroPos.x,(int)heroPos.y);
						occupants[hero]=heroPos;
						if(dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]==groundTile){
							dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]=heroTile;
						}else if(dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]==destinationTile){
							dataContainer.levelData[(int)heroPos.x,(int)heroPos.y]=heroOnDestinationTile;
						}
					}
				}
			}
			CheckCompletion();//check if all balls have reached destinations
		}
    }

    private void CheckCompletion()
    {

        int ballsOnDestination=0;
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
                if(dataContainer.levelData[i,j]==ballOnDestinationTile){
					ballsOnDestination++;
				}
			}
		}
		if(ballsOnDestination==ballCount){
			Debug.Log("level complete");
			gameOver=true;
		}
    }
    private GameObject GetOccupantAtPosition(Vector2 heroPos)
    {//loop through the occupants to find the ball at given position
        GameObject ball;
		foreach (KeyValuePair<GameObject, Vector2> pair in occupants)
		{
			if (pair.Value == heroPos)
			{
				ball = pair.Key;
				return ball;
			}
		}
		return null;
    }

    private void RemoveOccuppant(Vector2 objPos)
    {
        if(dataContainer.levelData[(int)objPos.x,(int)objPos.y]==heroTile||dataContainer.levelData[(int)objPos.x,(int)objPos.y]==ballTile){
			dataContainer.levelData[(int)objPos.x,(int)objPos.y]=groundTile;//ball moving from ground tile
		}else if(dataContainer.levelData[(int)objPos.x,(int)objPos.y]==heroOnDestinationTile){
			dataContainer.levelData[(int)objPos.x,(int)objPos.y]=destinationTile;//hero moving from destination tile
		}else if(dataContainer.levelData[(int)objPos.x,(int)objPos.y]==ballOnDestinationTile){
			dataContainer.levelData[(int)objPos.x,(int)objPos.y]=destinationTile;//ball moving from destination tile
		}
    }

    private bool IsOccuppied(Vector2 objPos)
    {//check if there is a ball at given array position
        return (dataContainer.levelData[(int)objPos.x,(int)objPos.y]==ballTile || dataContainer.levelData[(int)objPos.x,(int)objPos.y]==ballOnDestinationTile);
    }

    private bool IsValidPosition(Vector2 objPos)
    {//check if the given indices fall within the array dimensions
        if(objPos.x>-1&&objPos.x<rows&&objPos.y>-1&&objPos.y<cols){
			return dataContainer.levelData[(int)objPos.x,(int)objPos.y]!=invalidTile;
		}else return false;
    }

    private Vector2 GetNextPositionAlong(Vector2 objPos, int direction)
    {
        switch(direction){
			case 0:
			objPos.x-=1;//up
			break;
			case 1:
			objPos.y+=1;//right
			break;
			case 2:
			objPos.x+=1;//down
			break;
			case 3:
			objPos.y-=1;//left
			break;
		}
		return objPos;
    }
	public Vector2 GetScreenPointFromLevelIndices(int row,int col){
		return new Vector2(col*tileSize-middleOffset.x,row*-tileSize+middleOffset.y);
	}
	/*//the reverse methods to find indices from a screen point
	Vector2 GetLevelIndicesFromScreenPoint(float xVal,float yVal){
		return new Vector2((int)(yVal-middleOffset.y)/-tileSize,(int)(xVal+middleOffset.x)/tileSize);
	}
	Vector2 GetLevelIndicesFromScreenPoint(Vector2 pos){
		return GetLevelIndicesFromScreenPoint(pos.x,pos.y);
	}*/
	public void RestartLevel(){
		//Application.LoadLevel(0);
		SceneManager.LoadScene(0);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour {

	public const int xSizeInTiles=128;
	public const int ySizeInTiles=128;
	//public Sprite terrainSprite; // In futher adaption, this will be an array of sprites
	public GameObject terrainCube;
	public GameObject basePlatform;
	public float initialTileThreshold=0.45f; //Probability of tile being generated in initial spawn
	public Text text;

	public int[,] tileValue = new int[xSizeInTiles, ySizeInTiles];
	int[,] tileTemp = new int[xSizeInTiles, ySizeInTiles];

	//Debug Only
	int iterationCount=0;
	public GameObject indicator;

	// Use this for initialization
	void Start () {
		//Debug.Log("Hello World!!!!");
		//Debug.Log ("Start Time: " + Time.realtimeSinceStartup);
		TerrainGeneration();
		//Debug.Log ("Seed Time: " + Time.realtimeSinceStartup);
		CreateTerrain();
		//Debug.Log ("Instantiate Time: " + Time.realtimeSinceStartup);
		//Debug.Log ("Inside Terran, 32,0 is " + tileValue[32,0]);
		CreateBase();
	}
	
	// Update is called once per frame
	void Update () {
		//IterativeUpdate();
	}

	void IterativeUpdate(){
		if (Input.anyKeyDown){
			text.text = "Iteration Number: " + iterationCount.ToString();
			DestroyTerrain();
			DebugIteration(iterationCount);
			CreateTerrain();
			iterationCount++;
		}

	}
	void DebugIteration(int iteration){
		int loopAIterations=5, loopBIterations=0;

		if (iteration==0){
			InitialSeed();
			//ZoomIn(5,5);
		} else if (iteration>0 && iteration<=loopAIterations){
			AlgorithmA();
			//ZoomIn(5,5);
		} else if (iteration>loopAIterations&&iteration<=loopAIterations+loopBIterations){
			AlgorithmB();
		} else {Debug.Log("Iterations Complete");}
	}

	void ZoomIn(int x, int y){
		Camera.main.transform.position=new Vector3(x,y,-5);
		Instantiate (indicator, new Vector3(x,y,-1), Quaternion.identity);
		Debug.Log (tileValue[x,y]);
		Debug.Log (CountNeighbours(x,y));
	}

	void TerrainGeneration(){
		int loopAIterations=3, loopBIterations=0;


		InitialSeed();

		for (int loopA=0; loopA <loopAIterations; loopA++){
			AlgorithmA();
		}

		for (int loopB=0; loopB <loopBIterations; loopB++){
			AlgorithmB();
		}

		MakeExit();
	}

	void InitialSeed(){
		

		for (int x=0; x<xSizeInTiles;x++){
			for (int y=0; y<ySizeInTiles;y++){
				float tileGen=Random.value;
				if (x==0 || y==0||x==xSizeInTiles-1||y==ySizeInTiles-1){// Edges are always filled in
					//Debug.Log (x + " " + y + "is good.");
					tileValue[x,y]=1;
				} else if (x==xSizeInTiles/2){ //Create empty channel up centre of map
						tileValue[x,y]=0;
				} else if (tileGen<initialTileThreshold){
						tileValue[x,y]=1;
				}
					//Debug.Log (x + " " +y);
			}

		}		

	}

	void AlgorithmA(){
		for (int x=1; x<xSizeInTiles-1;x++){// Disregarding outer tiles during count
			for (int y=1; y<ySizeInTiles-1;y++){
				int count=CountNeighbours(x,y);
				if (count>=5){
					tileValue[x,y]=count;
					//Debug.Log("Hit");
				}	else{
					tileValue[x,y]=0;
				}
			}
		}		
		//SwapTemp();
	}

	void AlgorithmB(){

	}

	void MakeExit(){
		for (int x=(xSizeInTiles/2)-5; x<=(xSizeInTiles/2)+5; x++) {
			for (int y=ySizeInTiles-5; y<=ySizeInTiles-1; y++) {
				tileValue[x,y]=0;
			}
		}
	}

	void SwapTemp(){
		for (int x=1; x<xSizeInTiles-1;x++){
			for (int y=1; y<ySizeInTiles-1;y++){
				tileValue[x,y]=tileTemp[x,y];
			}
		}
	}

	int CountNeighbours(int x, int y){
		int count=0;
		for (int deltaX=-1;deltaX<2;deltaX++){
			for (int deltaY=-1;deltaY<2;deltaY++){
				//if (!(deltaX==0&&deltaY==0)){//Skip target square - didn't initially realise, but not required
					//Debug.Log ((x+deltaX) + " " + (y+deltaY));
					if (tileValue[x+deltaX, y+deltaY]>0){ // If there is any kind of tile in a surround square
						count++; //... increase count of target square
					}
				//}
			}
		}
		return count;
	}

	void CreateTerrain(){
		float xOffset=-63.5f, yOffset=-127.5f;

		for (int x=0; x<xSizeInTiles;x++){
			for (int y=-10; y<0; y++){
				Instantiate (terrainCube, new Vector3(x+xOffset,y+yOffset,0), Quaternion.identity, transform);
			}
			for (int y=0; y<ySizeInTiles;y++){
				if (tileValue[x,y]>0&&tileValue[x,y]<=9){
					Instantiate (terrainCube, new Vector3(x+xOffset,y+yOffset,0), Quaternion.identity, transform);

				}	
			}
		}		

	}

	void DestroyTerrain(){
		foreach (Transform cube in transform){
			Destroy (cube.gameObject);
		}
	}

	void CreateBase(){
		float xOffset=-63.5f, yOffset=-127.5f;
		int startY;
		int startX=xSizeInTiles/2;

		for (startY=1;startY<ySizeInTiles; startY++){
			// Look for three free horizontal tiles in a row
			if (tileValue[startX, startY]+tileValue[startX-1, startY]+tileValue[startX+1, startY]==0){
				break;
			}
		}
		if (startY==ySizeInTiles-1){
			Debug.Log ("No free space along entrance channel!");
		}
		else {
			
			Instantiate (basePlatform, new Vector3((float)startX+xOffset, (float)startY-0.18f+yOffset, 0.65f), Quaternion.Euler(new Vector3(-90,90,0)));
			//transform.position=new Vector3 (startX, startY, 0f);
			//Debug.Log (startX +" " +startY);
			//Debug.Log ("Outside Terran, 32,0 is " +terrain.tileValue[32,0]);
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[Header("Map")]

	[Min(1)] public int mapWidth;
	[Min(1)] public int mapHeight;
	private int halfMapWidth;
	private int halfMapHeight;

	public Voxel voxelPrefab;
	private Queue<GameObject> voxels = new Queue<GameObject>();

	public Material[] voxelMats;

	private float voxelWidth;
	private float voxelHeight;	
	private float halfVoxelWidth;
	private float halfVoxelHeight;	

	[Header("Noise Parameters")]

	[Min(1)] public int width;
	[Min(1)] public int height;

	public float noiseScale;

	[Min(0)] public int octaves;
	[Range(0, 1)] public float persistance;
	[Min(1)] public float lacunarity;

	public int seed;

	public Vector2 offset;

	private float[,] noiseMap;

	private void Start()
	{
		// Initialize player
		Player player = FindObjectOfType<Player>();
		player.Init(RedrawMap);

		// calculate some values
		halfMapWidth = mapWidth / 2;
		halfMapHeight = mapHeight / 2;

		// calculate some values
		voxelWidth = voxelPrefab.transform.localScale.x;
		voxelHeight = voxelPrefab.transform.localScale.z;
		halfVoxelWidth = voxelPrefab.transform.localScale.x / 2;
		halfVoxelHeight = voxelPrefab.transform.localScale.z / 2;

		// Instantiate Voxels
		Transform voxelHolder = new GameObject("VoxelHolder").transform;
		for (int i = 0; i < mapWidth * mapHeight * 10; i++)
		{
			Voxel voxel = Instantiate(voxelPrefab);
			voxel.Init(ReturnVoxel);
			voxel.gameObject.SetActive(false);
			voxel.transform.SetParent(voxelHolder);
			voxels.Enqueue(voxel.gameObject);
		}

		// Generate Noise map and draw map with voxels;
		GenerateNoiseMap();
		DrawMap();
	}

	// Generate a perlin noise map
	public void GenerateNoiseMap()
	{
		noiseMap = Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);		
	}

	// At first, draw map around player
	public void DrawMap()
	{
		for (int y = -halfMapHeight; y <= halfMapHeight; y++)
		{
			for (int x = -halfMapWidth; x <= halfMapWidth; x++)
			{
				int PosX = Mathf.Clamp(Player.Pos.x / 10 + x, 0, noiseMap.GetLength(0) - 1);
				int PosY = Mathf.Clamp(Player.Pos.z / 10 + y, 0, noiseMap.GetLength(1) - 1);

				GetNewVoxel(PosX, PosY, (int)(noiseMap[PosX, PosY] / 0.2f) + 1);
			}
		}
	}

	// Redraw map about player movement
	public void RedrawMap(Vector2 offset)
	{
		// Appear
		if (offset.x != 0)
		{
			int PosX = Mathf.Clamp(Player.Pos.x / 10 + (offset.x > 0 ? halfMapWidth : -halfMapWidth), 0, noiseMap.GetLength(0) - 1);
			for (int y = -halfMapHeight; y <= halfMapHeight; y++)
			{
				int PosY = Mathf.Clamp(Player.Pos.z / 10 + y, 0, noiseMap.GetLength(1) - 1);

				GetNewVoxel(PosX, PosY, (int)(noiseMap[PosX, PosY] / 0.2f) + 1);
			}
		}

		if (offset.y != 0)
		{
			int PosY = Mathf.Clamp(Player.Pos.z / 10 + (offset.y > 0 ? halfMapHeight : -halfMapHeight), 0, noiseMap.GetLength(1) - 1);
			for (int x = -halfMapWidth; x <= halfMapWidth; x++)
			{
				int PosX = Mathf.Clamp(Player.Pos.x / 10 + x, 0, noiseMap.GetLength(0) - 1);

				GetNewVoxel(PosX, PosY, (int)(noiseMap[PosX, PosY] / 0.2f) + 1);
			}
		}
	}

	// Place new voxel
	private void GetNewVoxel(int PosX, int PosY, int index)
    {
		GameObject voxel = voxels.Dequeue();
		voxel.transform.localScale = new Vector3(voxelWidth, index * 5, voxelHeight);
		voxel.transform.position = new Vector3(PosX * voxelWidth + halfVoxelWidth, index * 2.5f, PosY * voxelHeight + halfVoxelHeight);
		Material mat = voxelMats[index - 1];
		voxel.GetComponent<MeshRenderer>().material = mat;
		voxel.SetActive(true);
	}

	// Return voxel that is far away from player
	private void ReturnVoxel(GameObject voxel)
	{
		voxel.SetActive(false);
		voxels.Enqueue(voxel);
	}
}

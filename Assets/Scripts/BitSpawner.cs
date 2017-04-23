using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitSpawner : MonoBehaviour {
    [Serializable]
    public struct BitSpawnBounds
    {
        public int leftBounds;
        public int rightBounds;
        public int yMin;
        public int yMax;
    }
    [Serializable]
    public struct BitDropTable
    {
        public int smallBitChance;
        public int largeBitChance;
        public int trailChance;
    }
    [Header("Level Bounds")]
    public BitSpawnBounds bitSpawnBounds;
    [Header("Bit Drop Percentage")]
    public BitDropTable bitDropTable;
    [Header("Object References")]
    public GameObject bitContainer;
    public GameObject bitSmall;
    public GameObject bitLarge;
    public GameObject[] bitTrails;
    [Header("Public Variables")]
    public int bitSpawnFrequency = 5000;
    public int distanceBetweenBits = 1;

	private void Start()
    {
        SpawnBits(); 
    }

    public void DespawnBits()
    {
        var children = new List<GameObject>();
        foreach (Transform child in bitContainer.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void SpawnBits()
    {
        //Vars used to spawn and move the bits
        int newPosX = 0;
        int newPosY = 0;
        int rand = 0;
        Vector3 spawnPoint;
        GameObject objectToSpawn = null;
        Collider2D objectsHit;
        //Loop to spawn enough bits to cover the map
        for (int i = 0; i < bitSpawnFrequency; i++)
        {
            //Try to find the position in the level bounds to spawn the bits
            try
            {
                newPosX = UnityEngine.Random.Range(bitSpawnBounds.leftBounds, bitSpawnBounds.rightBounds);
                newPosY = UnityEngine.Random.Range(bitSpawnBounds.yMin, bitSpawnBounds.yMax);
                spawnPoint = new Vector2(newPosX, newPosY);
            }
            catch
            {
                throw new Exception("Your level bounds are invalid, check them and try again!");
            }
            //Randomly pick a bit to spawn from the drop table
            rand = UnityEngine.Random.Range(0, (bitDropTable.smallBitChance + bitDropTable.largeBitChance + bitDropTable.trailChance));
            if (rand < bitDropTable.smallBitChance)
            {
                objectToSpawn = bitSmall;
            }
            else if (rand < bitDropTable.smallBitChance + bitDropTable.largeBitChance)
            {
                objectToSpawn = bitLarge;
            }
            else
            {
                objectToSpawn = bitTrails[UnityEngine.Random.Range(0, bitTrails.Length)];
            }
            //Spawn the bit
            objectToSpawn = Instantiate(objectToSpawn) as GameObject;
            //Check for overlap
            objectsHit = Physics2D.OverlapCircle(spawnPoint, distanceBetweenBits);
            if (objectsHit == null)
            {
                objectToSpawn.transform.position = spawnPoint;
            }
            else
            {
                objectToSpawn.SetActive(false);
            }
            objectToSpawn.transform.SetParent(bitContainer.transform);
        }
    }
}

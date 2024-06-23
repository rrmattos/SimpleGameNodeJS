using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private float spawnTime;
    [SerializeField] private List<Transform> fruits = new();
    [SerializeField] private List<Transform> enemies = new();

    private bool isSpawning = false;
    private bool increaseDificulty = false;

    private void Start()
    {
        StartCoroutine(DificultyTimer());
    }

    private void FixedUpdate()
    {
        if (isSpawning) return;

        StartCoroutine(SpawnTimer());
    }

    private IEnumerator DificultyTimer()
    {
        yield return new WaitForSeconds(60);

        increaseDificulty = true;
        spawnTime = 1;
    }

    private IEnumerator SpawnTimer()
    {
        isSpawning = true;

        if (increaseDificulty) spawnTime -= 0.01f;
            
        int sortedNum = Random.Range(0, 2);
        string pathLoader = sortedNum == 0 ? "Fruits" : "Enemies";

        string objName = SortAssets($"Assets/Resources/{pathLoader}");

        GameObject newObj = Instantiate(Resources.Load<GameObject>($"{pathLoader}/{objName}"));

        Camera mainCamera = Camera.main;

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;

        float minX = (mainCamera.transform.position.x - width / 2) + 0.2f;
        float maxX = (mainCamera.transform.position.x + width / 2) - 0.2f;

        float xPosition = Random.Range(minX, maxX);

        newObj.transform.position = new Vector2(xPosition, transform.position.y);

        if (sortedNum == 0)
            fruits.Add(newObj.transform);
        else
            enemies.Add(newObj.transform);

        yield return new WaitForSeconds(spawnTime);

        isSpawning = false;
    }

    private string SortAssets(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError($"The path {path} does not exist.");
            return null;
        }

        string[] fileEntries = Directory.GetFiles(path, "*.*");
        //Debug.Log(fileEntries[0]);
        int sortedObjNum = Random.Range(0, fileEntries.Length);
        string[] parts = fileEntries[sortedObjNum].Split("\\");
        return parts[1].Replace(".prefab", "").Replace(".meta", "");
    }
}

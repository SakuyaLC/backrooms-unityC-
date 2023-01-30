using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacer : MonoBehaviour
{

    [Header("Check radius")]
    public float coordinatesSum = 0;
    public float generationRadius = 10000;

    public Transform Player;
    public float playerX = 0;
    public float playerY = 0;
    public float playerZ = 0;
    public Chunk[] Prefabs;
    public int chunksCount = 0;

    private List<Chunk> highPriorityChunks = new List<Chunk>();
    private List<Chunk> lowPriorityChunks = new List<Chunk>();
    private List<Chunk> finalChunks = new List<Chunk>();
    private List<Transform> connectionsList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        playerX = Player.position.x;
        playerY = Player.position.y;
        playerZ = Player.position.z;
        //Заготовка стартовой локации
        spawnFirstChunk();
        //Player.transform.position = new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        playerX = Player.position.x;
        playerZ = Player.position.z;
        generateChunks(playerX, playerZ);
    }

    private void spawnFirstChunk()
    {
        Chunk newChunk = Instantiate(Prefabs[0], new UnityEngine.Vector3(0, 0, 0), Quaternion.identity);
        connectionsList.Add(newChunk.CLeft);
        connectionsList.Add(newChunk.CUpper);
        connectionsList.Add(newChunk.CRight);
        connectionsList.Add(newChunk.CBottom);
        highPriorityChunks.Add(newChunk);
        //Player.localPosition = newChunk.transform.localPosition;
    }

    private void generateChunks(float playerX, float playerZ)
    {
        Debug.Log("Chunks with connections - " + (highPriorityChunks.Count + lowPriorityChunks.Count));
        Debug.Log("High priority chunks - " + highPriorityChunks.Count);
        Debug.Log("Low priority chunks - " + lowPriorityChunks.Count);

        bool canGenerate = false;
        int randomChunkIndex = Random.Range(0, highPriorityChunks.Count);
        int randomConnectionIndex = 0;
        int i = 0;
        int j = 0;

        //Убираем приоритет у чанков, которые далеко от игрока и подгружаем те, которые близко
        if (highPriorityChunks.Count > 0)
        {
            foreach (Chunk chunk in highPriorityChunks)
            {
                coordinatesSum = Mathf.Pow(chunk.transform.position.x - playerX, 2) + Mathf.Pow(chunk.transform.position.z - playerZ, 2);
                if (chunk == null)
                {
                    highPriorityChunks.Remove(chunk);
                }
                else if (coordinatesSum > generationRadius)
                {
                    lowPriorityChunks.Add(chunk);
                    highPriorityChunks.Remove(chunk);
                }

                if (coordinatesSum <= generationRadius)
                {
                    chunk.forRender.SetActive(true);
                }

            }
        }

        //Даем приоритет чанкам, которые близко с игроком и выгружаем те, которые далеко
        if (lowPriorityChunks.Count > 0)
        {
            foreach (Chunk chunk in lowPriorityChunks)
            {
                coordinatesSum = Mathf.Pow(chunk.transform.position.x - playerX, 2) + Mathf.Pow(chunk.transform.position.z - playerZ, 2);
                if (chunk == null)
                {
                    lowPriorityChunks.Remove(chunk);
                }
                else if (coordinatesSum <= generationRadius)
                {
                    highPriorityChunks.Add(chunk);
                    lowPriorityChunks.Remove(chunk);
                }

                if (coordinatesSum > generationRadius)
                {
                    chunk.forRender.SetActive(false);
                }
            }
        }

        //Расчитываем радиус
        coordinatesSum = Mathf.Pow(highPriorityChunks[randomChunkIndex].transform.position.x - playerX, 2) + Mathf.Pow(highPriorityChunks[randomChunkIndex].transform.position.z - playerZ, 2);

        //Если чанк в опасной близости от игрока
        if (coordinatesSum <= generationRadius)
        {

            if (highPriorityChunks[randomChunkIndex].CLeft != null)
            {
                randomConnectionIndex = 1;
                canGenerate = true;
            }
            else if (highPriorityChunks[randomChunkIndex].CUpper != null)
            {
                randomConnectionIndex = 2;
                canGenerate = true;
            }
            else if (highPriorityChunks[randomChunkIndex].CRight != null)
            {
                randomConnectionIndex = 3;
                canGenerate = true;
            }
            else if (highPriorityChunks[randomChunkIndex].CBottom != null)
            {
                randomConnectionIndex = 4;
                canGenerate = true;
            }
            else if (highPriorityChunks[randomChunkIndex].CLeft == null && highPriorityChunks[randomChunkIndex].CUpper == null && highPriorityChunks[randomChunkIndex].CRight == null && highPriorityChunks[randomChunkIndex].CBottom == null)
            {
                finalChunks.Add(highPriorityChunks[randomChunkIndex]);
                highPriorityChunks.Remove(highPriorityChunks[randomChunkIndex]);
            }
        }

        if (canGenerate)
        {
            Chunk newChunk = Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], new Vector3(0, 0, 0), Quaternion.identity);

            List<Transform> tempConnections = new List<Transform>();
            tempConnections.Add(newChunk.CLeft);
            tempConnections.Add(newChunk.CUpper);
            tempConnections.Add(newChunk.CRight);
            tempConnections.Add(newChunk.CBottom);

            newChunk.transform.SetPositionAndRotation(highPriorityChunks[randomChunkIndex].transform.position, highPriorityChunks[randomChunkIndex].transform.rotation);

            if (randomConnectionIndex == 1)
            {
                //Debug.Log("Generating room on the left");
                newChunk.transform.position += new Vector3(0, 0, -150);
                Destroy(newChunk.CRight.gameObject);
                Destroy(highPriorityChunks[randomChunkIndex].CLeft.gameObject);
                connectionsList.Remove(highPriorityChunks[randomChunkIndex].CLeft);
                connectionsList.Remove(newChunk.CRight);
                tempConnections.Remove(newChunk.CRight);
            }

            if (randomConnectionIndex == 2)
            {
                //Debug.Log("Generating room on the upper");
                newChunk.transform.position += new Vector3(-150, 0, 0);
                Destroy(newChunk.CBottom.gameObject);
                Destroy(highPriorityChunks[randomChunkIndex].CUpper.gameObject);
                connectionsList.Remove(highPriorityChunks[randomChunkIndex].CUpper);
                connectionsList.Remove(newChunk.CBottom);
                tempConnections.Remove(newChunk.CBottom);
            }

            if (randomConnectionIndex == 3)
            {
                //Debug.Log("Generating room on the right");
                newChunk.transform.position += new Vector3(0, 0, 150);
                Destroy(newChunk.CLeft.gameObject);
                Destroy(highPriorityChunks[randomChunkIndex].CRight.gameObject);
                connectionsList.Remove(highPriorityChunks[randomChunkIndex].CRight);
                connectionsList.Remove(newChunk.CLeft);
                tempConnections.Remove(newChunk.CLeft);
            }

            if (randomConnectionIndex == 4)
            {
                //Debug.Log("Generating room on the bottom");
                newChunk.transform.position += new Vector3(150, 0, 0);
                Destroy(newChunk.CUpper.gameObject);
                Destroy(highPriorityChunks[randomChunkIndex].CBottom.gameObject);
                connectionsList.Remove(highPriorityChunks[randomChunkIndex].CBottom);
                connectionsList.Remove(newChunk.CUpper);
                tempConnections.Remove(newChunk.CUpper);
            }

            for (i = 0; i < tempConnections.Count; i++)
            {
                for (j = 0; j < connectionsList.Count; j++)
                {
                    if (tempConnections[i] != null && connectionsList[j] != null && tempConnections[i].position == connectionsList[j].position)
                    {
                        //Debug.Log("Temp Connection " + tempConnections[i].position + " and connection " + connectionsList[j].position + " have been removed");
                        Destroy(tempConnections[i].gameObject);
                        Destroy(connectionsList[j].gameObject);
                        connectionsList.Remove(connectionsList[j]);
                    }
                }
            }

            for (i = 0; i < connectionsList.Count; i++)
            {
                if (connectionsList[i] == null)
                {
                    Debug.Log("Connection #" + i + " has been removed, because it's null");
                    connectionsList.Remove(connectionsList[i]);
                }
            }

            tempConnections.Clear();

            if (newChunk.CLeft != null) connectionsList.Add(newChunk.CLeft);
            if (newChunk.CUpper != null) connectionsList.Add(newChunk.CUpper);
            if (newChunk.CRight != null) connectionsList.Add(newChunk.CRight);
            if (newChunk.CBottom != null) connectionsList.Add(newChunk.CBottom);

            highPriorityChunks.Add(newChunk);
            chunksCount++;
            canGenerate = false;

        }

    }

}

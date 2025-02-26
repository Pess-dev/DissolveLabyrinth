using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] float chunkSize = 50f;
    List<Transform> chunks = new List<Transform>();

    Transform initial;
    Transform currentChunk;

    void Start()
    {
        currentChunk = transform.GetChild(0);
        chunks.Add(currentChunk);

        GenerateSurroundingChunks();
    }

    void Update(){
        MoveInDelta(-PlayerController.deltaPosition);
    }

    void MoveInDelta(Vector3 delta){
        delta = Vector3.ProjectOnPlane(delta, Vector3.up)/2;

        foreach (Transform chunk in chunks){
            chunk.position += delta;
        }
    }
    void UpdateChunks()
    {
        // Определяем, сместился ли текущий центральный чанк
        Vector3 currentChunkPosition = currentChunk.position;
        Vector3 playerPosition = PlayerController.position;

        // Если игрок сместился на расстояние большее, чем размер чанка, обновляем чанки
        if (Vector3.Distance(playerPosition, currentChunkPosition) > chunkSize)
        {
            // Находим новый центральный чанк
            foreach (Transform chunk in chunks)
            {
                if (Vector3.Distance(playerPosition, chunk.position) <= chunkSize / 2)
                {
                    currentChunk = chunk;
                    break;
                }
            }

            // Уничтожаем чанки, которые ушли слишком далеко
            DestroyDistantChunks();

            // Создаём недостающие чанки вокруг нового центрального
            GenerateSurroundingChunks();
        }
    }

    void GenerateSurroundingChunks()
    {
        Vector3 currentPosition = currentChunk.position;

        // Проходим по всем возможным позициям чанков в сетке 3x3
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Пропускаем центральный чанк
                if (x == 0 && z == 0) continue;

                // Вычисляем позицию нового чанка
                Vector3 chunkPosition = currentPosition + new Vector3(x * chunkSize, 0, z * chunkSize);

                // Проверяем, существует ли уже чанк в этой позиции
                if (!IsChunkAtPosition(chunkPosition))
                {
                    // Создаём новый чанк
                    GameObject newChunk = Instantiate(currentChunk.gameObject, chunkPosition, Quaternion.identity, transform);
                    chunks.Add(newChunk.transform);
                }
            }
        }
    }

    void DestroyDistantChunks()
    {
        // Уничтожаем чанки, которые находятся слишком далеко от текущего центрального
        for (int i = chunks.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(chunks[i].position, currentChunk.position) > chunkSize * 1.5f)
            {
                Destroy(chunks[i].gameObject);
                chunks.RemoveAt(i);
            }
        }
    }

    bool IsChunkAtPosition(Vector3 position)
    {
        // Проверяем, есть ли чанк в указанной позиции
        foreach (Transform chunk in chunks)
        {
            if (Vector3.Distance(chunk.position, position) < 0.1f)
            {
                return true;
            }
        }
        return false;
    }
}

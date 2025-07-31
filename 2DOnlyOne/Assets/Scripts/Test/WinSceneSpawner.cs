using UnityEngine;

public class WinSceneSpawner : MonoBehaviour
{
    [Header("Prefab and Spawn Settings")]
    public GameObject prefabToSpawn;
    public int numberToSpawn = 10;

    [Header("Spawn Bounds")]
    public Vector2 xBounds = new Vector2(-10f, 10f);
    public Vector2 yBounds = new Vector2(-5f, 5f);

    void Start()
    {
        Vector2 spawnPos;
        GameObject obj;
        for (int i = 0; i < numberToSpawn; i++)
        {
            spawnPos = new Vector2(
                Random.Range(xBounds.x, xBounds.y),
                Random.Range(yBounds.x, yBounds.y)
            );

            obj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            obj.GetComponent<NPCBehavior>().Dance();
        }
    }

}

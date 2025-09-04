using UnityEngine;

public class CubeHome : MonoBehaviour
{
    public int cubeCount;
    public GameObject CubePrefab; //What we spawn.
    public Transform spawnPoint;
    public delegate void intUpdate(int wow);
    public intUpdate cubeCountUpdated;
    public void SpawnCube()
    { 
        if(cubeCount < 20)
        {
            GameObject newCube = GameObject.Instantiate(CubePrefab, transform);
            newCube.transform.position = spawnPoint.position;
            cubeCount++;
            cubeCountUpdated.Invoke(cubeCount);
        }
    }
}

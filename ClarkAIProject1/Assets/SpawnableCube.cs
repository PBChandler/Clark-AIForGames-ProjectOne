using UnityEngine;

public class SpawnableCube : MonoBehaviour
{
    public Color color_Red, color_Blue, color_Green;
    public MeshRenderer cubeMesh;
    public enum CubeType
    {
        NULL,
        RED,
        BLUE,
        GREEN
    }

    public CubeType myCubeType;

    public void Start()
    {
        myCubeType = (CubeType)(int)Random.Range(1, 4);
        switch (myCubeType)
        {
            case CubeType.NULL:
                break;
            case CubeType.RED:
                cubeMesh.material.color = color_Red;
                break;
            case CubeType.BLUE:
                cubeMesh.material.color = color_Blue;
                break;
            case CubeType.GREEN:
                cubeMesh.material.color = color_Green;
                break;
            default:
                break;
        }
    }
}

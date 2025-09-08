using System.Collections.Generic;
using UnityEngine;

public class TitleScreenTV : MonoBehaviour
{
    public List<MeshRenderer> screens;
    public Material red, blue, green, white;

    public AudioSource source;

    public void Start()
    {
        InvokeRepeating("ChangeRandomScreen", 0f, 0.4f);
    }
    public void Update()
    {
        
    }

    public void ChangeRandomScreen()
    {
        int colorType = Random.Range(0, 3);
        foreach(MeshRenderer mesh in screens)
        {
            mesh.material = white;
        }
        switch (colorType)
        {
            case 0:
                screens[Random.Range(0, screens.Count)].material = red;
                break;
            case 1:
                screens[Random.Range(0, screens.Count)].material = blue;
                break;
            case 2:
                screens[Random.Range(0, screens.Count)].material = green;
                break;
        }
    }
}

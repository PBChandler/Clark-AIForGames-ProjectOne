using UnityEngine;

public class TitleScreenCameraMovement : MonoBehaviour
{
    public bool upward;
    public float strength;
    public float speed;
    Vector3 basepos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basepos = transform.position;
    }
    float f = 0;
    float time = 0;
   
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;
        float x = Mathf.Sin(time);
        float y = Mathf.Sin(2 * time);
        
        transform.position = new Vector3(x+basepos.x,y+basepos.y,basepos.z);
    }
}

using System.Collections;
using UnityEngine;

public class PhysicalButtonAnimation : MonoBehaviour
{
    public float speed;
    public float dropDownAmount;
    private float baseheight;
    bool process;

    public void Start()
    {
        baseheight = transform.localPosition.y;
    }
    public void Depress()
    {
        if(!process)
        {
            process = true;
            StartCoroutine(PressButtonInteraction());
        }
        
    }
    /// <summary>
    /// could do some more smoothing but not necessary.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PressButtonInteraction()
    {
        while(transform.localPosition.y > dropDownAmount)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - speed, transform.localPosition.z);
            yield return new WaitForSeconds(0.01f);
        }
        transform.localPosition = new Vector3(transform.localPosition.x, dropDownAmount, transform.localPosition.z);
        yield return new WaitForSeconds(0.1f);
        while (transform.localPosition.y < baseheight)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + speed, transform.localPosition.z);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = new Vector3(transform.localPosition.x, baseheight, transform.localPosition.z);
        process = false;

    }
}

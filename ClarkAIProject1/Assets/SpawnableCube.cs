using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SpawnableCube : MonoBehaviour, Interactable
{
    public Color color_Red, color_Blue, color_Green;
    public MeshRenderer cubeMesh;
    public List<SpawnableCube> neighbors;
    public GameObject Stackmessage;
    public GameObject fullstackmessage;
    public CubeType myCubeType;
    public List<SpawnableCube> chain = new List<SpawnableCube>();
    public bool isBeingHeld = false;
    public Rigidbody rb;
    public float lifeTime;
    public Transform hat, boots;
    public float sphereCastRange;
    private Collider me;
    public enum CubeType
    {
        NULL,
        RED,
        BLUE,
        GREEN
    }
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        me = GetComponent<Collider>();
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
        GameManager.instance.cubes.Add(this);
        GameManager.instance.CubeCount = GameManager.instance.cubes.Count;
    }

    public void OnInteracted()
    {
        if (GameManager.playerAlreadyHoldingCube)
            return;
        isBeingHeld = true;
        GameManager.playerAlreadyHoldingCube = true;
    }

    public void Update()
    {
        lifeTime += Time.deltaTime;
        if(isBeingHeld)
        {
            rb.isKinematic = false;
            if (Input.GetKeyUp(KeyCode.E))
            {
                GameManager.playerAlreadyHoldingCube = false;
                isBeingHeld = false;
            }
            transform.position = GameManager.instance.playerHoldPoint.position;
            transform.rotation = GameManager.instance.playerHoldPoint.rotation;
        }
        else
        {
            if (rb.angularVelocity.magnitude == 0 && rb.linearVelocity.magnitude == 0 && lifeTime > 2f)
            {
                
               rb.isKinematic = true;
            }
        }
        CheckFor3Stack();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Launchable")
        {
            #region oldcode
            //if (neighbors.Contains(collision.gameObject.GetComponent<SpawnableCube>()))
            //    return;
            //neighbors.Add(collision.gameObject.GetComponent<SpawnableCube>());
            #endregion
            Invoke("CheckFor3Stack", 0.1f); //allow for bounce settling.
        }
    }

    public void CheckFor3Stack()
    {
        chain.Clear();
        neighbors.Clear();

        Collider[] aboveCollides = Physics.OverlapSphere(hat.position, 1f);
        Collider[] belowCollides = Physics.OverlapSphere(boots.position, 1f);

        List<Collider> combines;
        combines = aboveCollides.ToList<Collider>();
        foreach(Collider c in belowCollides)
        {
            combines.Add(c);
        }
        foreach(Collider c in combines)
        {
            Component spawn;
            c.TryGetComponent(typeof(SpawnableCube), out spawn);
            if(spawn != null && spawn != me)
            {
                SpawnableCube sp = (SpawnableCube)spawn;
                neighbors.Add(sp);
            }
        }
        foreach (SpawnableCube c in neighbors)
        {
            chain.Add(c);
            foreach (SpawnableCube d in c.chain)
            {
                if (!chain.Contains(d) && d != this)
                    chain.Add(d);
            }  
        }
        chain = RemoveDuplicates(chain);
        GameManager.iCheckStacks();
    }

    public List<SpawnableCube> RemoveDuplicates(List<SpawnableCube> pluh)
    {
        List<SpawnableCube> retval = new List<SpawnableCube>();
        foreach (SpawnableCube sad in pluh)
        {
            if(!retval.Contains(sad))
            {
                retval.Add(sad);
            }
        }
        return retval;
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Launchable")
        {
            chain.Remove(collision.gameObject.GetComponent<SpawnableCube>());
            neighbors.Remove(collision.gameObject.GetComponent<SpawnableCube>());
        }
        Invoke("CheckFor3Stack", 0.1f);
    }
}

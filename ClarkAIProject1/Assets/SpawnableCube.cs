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
    public bool HasLanded { get; private set; } = false;

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
            case CubeType.RED:
                cubeMesh.material.color = color_Red;
                break;
            case CubeType.BLUE:
                cubeMesh.material.color = color_Blue;
                break;
            case CubeType.GREEN:
                cubeMesh.material.color = color_Green;
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

        // tell manager that cubes should stay unfrozen
        GameManager.instance.cubesShouldStayUnfrozen = true;

        // unfreeze all cubes
        foreach (var cube in GameManager.instance.cubes)
        {
            if (cube != null && cube.rb != null)
                cube.rb.isKinematic = false;
        }
    }

    public void Update()
    {
        lifeTime += Time.deltaTime;

        if (isBeingHeld)
        {
            rb.isKinematic = false;

            if (Input.GetKeyUp(KeyCode.E))
            {
                GameManager.playerAlreadyHoldingCube = false;
                isBeingHeld = false;

                // allow cubes to freeze again once dropped
                GameManager.instance.cubesShouldStayUnfrozen = false;
            }

            transform.position = GameManager.instance.playerHoldPoint.position;
            transform.rotation = GameManager.instance.playerHoldPoint.rotation;
        }
        else
        {
            // only freeze cubes if player isn't holding one
            if (!GameManager.instance.cubesShouldStayUnfrozen)
            {
                if (rb.angularVelocity.magnitude == 0 && rb.linearVelocity.magnitude == 0 && lifeTime > 2f)
                {
                    rb.isKinematic = true;
                }
            }
        }

        CheckFor3Stack();
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (!HasLanded && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Cube")))
        {
            HasLanded = true;
            if (StackManager.Instance != null)
                StackManager.Instance.RegisterCube(transform);
        }
    }


    public void CheckFor3Stack()
    {
        chain.Clear();
        neighbors.Clear();

        Collider[] aboveCollides = Physics.OverlapSphere(hat.position, 1f);
        Collider[] belowCollides = Physics.OverlapSphere(boots.position, 1f);

        List<Collider> combines = aboveCollides.ToList();
        combines.AddRange(belowCollides);

        foreach (Collider c in combines)
        {
            if (c.TryGetComponent(out SpawnableCube sp) && sp != this)
            {
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
            if (!retval.Contains(sad))
                retval.Add(sad);
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

using UnityEngine;

public class ChainCheckChild : MonoBehaviour
{
    public SpawnableCube parent;

    //public void OnCollisionEnter(Collider other)
    //{
    //    if(other.gameObject.tag == "Launchable")
    //    {
    //        SpawnableCube s = other.GetComponent<SpawnableCube>();
    //        if (parent.neighbors.Contains(s))
    //            return;
    //        parent.neighbors.Add(s);
    //    }
    //}

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Launchable")
    //    {
    //        SpawnableCube s = collision.gameObject.GetComponent<SpawnableCube>();
    //        if (parent.neighbors.Contains(s))
    //            return;
    //        parent.neighbors.Add(s);
    //    }
    //    parent.CheckFor3Stack();
    //}


    //public void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Launchable")
    //    {
    //        SpawnableCube s = collision.gameObject.GetComponent<SpawnableCube>();
    //        if (parent.neighbors.Contains(s))
    //            return;
    //        parent.neighbors.Add(s);
    //    }
    //    parent.CheckFor3Stack();
    //}
    //public void OnCollisionExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Launchable")
    //    {
    //        SpawnableCube s = other.GetComponent<SpawnableCube>();
    //        if (parent.neighbors.Contains(s))
    //            parent.neighbors.Remove(s);
    //    }
    //}
}

using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    public Transform face;
    public LayerMask avoidPlayer;
    public GameObject activeInteractable;
    public void Update()
    {
        Ray r;
        RaycastHit hit;
        Physics.Raycast(face.transform.position, face.transform.forward, avoidPlayer);


        //debug, send message is awful but its just for testing for now.
        if (Physics.Raycast(face.position, face.TransformDirection(Vector3.forward), out hit, 20f, avoidPlayer))
        {
            activeInteractable = hit.collider.gameObject;
            try
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    activeInteractable.SendMessage("OnInteracted");
                }
                
            }
            catch { }
           
        }
    }
}

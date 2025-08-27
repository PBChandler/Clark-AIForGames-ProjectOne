using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// An incredibly versatile but overly-simple version of any interactable object, should only be used in niche cases or when there's only one of the object this interactable is on.
/// </summary>
public class UnityEventInteractable : MonoBehaviour, Interactable
{
    public UnityEvent myEvent;
    public void OnInteracted()
    {
        myEvent.Invoke();
    }
}

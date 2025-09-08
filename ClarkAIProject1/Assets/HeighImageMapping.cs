using UnityEngine;

[System.Serializable]
public class HeightImageMapping
{
    public string description; // For easier organization in the Inspector
    public int minHeight;
    public int maxHeight;

    [Header("Textures")]
    public Texture textureOnIncrease;
    public Texture textureOnDecrease;
}
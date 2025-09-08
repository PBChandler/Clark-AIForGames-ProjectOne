using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer))]
public class TVScreenController : MonoBehaviour
{
    [Tooltip("The default texture for the screen when no stack is present.")]
    [SerializeField] private Texture defaultScreenTexture;

    [Tooltip("Define the textures for each height range.")]
    [SerializeField] private List<HeightImageMapping> imageMappings;

    private Renderer _screenRenderer;
    private Material _screenMaterialInstance;

    private void Awake()
    {

        _screenRenderer = GetComponent<Renderer>();

        // Creates a unique instance of the material so we don't change the project asset.
        _screenMaterialInstance = _screenRenderer.material;

        _screenMaterialInstance.mainTexture = defaultScreenTexture;
    }

    private void OnEnable()
    {
        StackManager.OnStackHeightChanged += HandleStackHeightChanged;
    }

    private void OnDisable()
    {
        // avoid that scary memory leak
        StackManager.OnStackHeightChanged -= HandleStackHeightChanged;
    }

    private void HandleStackHeightChanged(StackHeightChangedEventArgs eventArgs)
    {
        int newHeight = eventArgs.NewHeight;
        HeightChangeDirection direction = eventArgs.Direction;

        // If the stack is gone, revert to the default texture.
        if (newHeight <= 0)
        {
            _screenMaterialInstance.mainTexture = defaultScreenTexture;
            return;
        }

        // Find the correct mapping for the current height.
        foreach (var mapping in imageMappings)
        {
            if (newHeight >= mapping.minHeight && newHeight <= mapping.maxHeight)
            {
                //Now pick the texture based on direction.
                Texture textureToDisplay = null;
                if (direction == HeightChangeDirection.Increased)
                {
                    textureToDisplay = mapping.textureOnIncrease;
                }
                else
                {
                    textureToDisplay = mapping.textureOnDecrease;
                }

               
                if (textureToDisplay != null)
                {
                    _screenMaterialInstance.mainTexture = textureToDisplay;
                }

   
                return;
            }
        }
    }
}
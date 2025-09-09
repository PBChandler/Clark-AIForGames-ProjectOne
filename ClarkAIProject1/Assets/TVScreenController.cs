using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TVScreenController : MonoBehaviour
{
    [Header("Screen Content")]
    [SerializeField] private Texture defaultScreenTexture;
    [SerializeField] private List<HeightImageMapping> imageMappings;

    [Header("Transition Effect")]
    [Tooltip("How long the channel change effect lasts in seconds.")]
    [SerializeField] private float transitionDuration = 0.4f;

    private Renderer _screenRenderer;
    private Material _screenMaterialInstance;
    private Coroutine _transitionCoroutine;

    private static readonly int TransitionAmountID = Shader.PropertyToID("_TransitionAmount");
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        _screenRenderer = GetComponent<Renderer>();
        _screenMaterialInstance = _screenRenderer.material;
        _screenMaterialInstance.mainTexture = defaultScreenTexture;
    }

    private void OnEnable()
    {
        StackManager.OnStackHeightChanged += HandleStackHeightChanged;
    }

    private void OnDisable()
    {
        StackManager.OnStackHeightChanged -= HandleStackHeightChanged;
    }

    private void HandleStackHeightChanged(StackHeightChangedEventArgs eventArgs)
    {
        Texture textureToDisplay = defaultScreenTexture;
        bool textureFound = false;

        if (eventArgs.NewHeight > 0)
        {
            foreach (var mapping in imageMappings)
            {
                if (eventArgs.NewHeight >= mapping.minHeight && eventArgs.NewHeight <= mapping.maxHeight)
                {
                    textureToDisplay = (eventArgs.Direction == HeightChangeDirection.Increased)
                        ? mapping.textureOnIncrease
                        : mapping.textureOnDecrease;

                    textureFound = textureToDisplay != null;
                    break;
                }
            }
        }

        if (textureFound || eventArgs.NewHeight <= 0)
        {
            // If a transition is already running, stop it first
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            _transitionCoroutine = StartCoroutine(ChangeChannelRoutine(textureToDisplay));
        }
    }

    private IEnumerator ChangeChannelRoutine(Texture newTexture)
    {
        float elapsedTime = 0f;

        // Animate the effect IN
        while (elapsedTime < transitionDuration / 2)
        {
            float progress = elapsedTime / (transitionDuration / 2);
            _screenMaterialInstance.SetFloat(TransitionAmountID, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- The exact moment the channel switches ---
        _screenMaterialInstance.SetTexture(MainTexID, newTexture);

        elapsedTime = 0f;
        while (elapsedTime < transitionDuration / 2)
        {
            float progress = 1.0f - (elapsedTime / (transitionDuration / 2));
            _screenMaterialInstance.SetFloat(TransitionAmountID, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the effect is fully off
        _screenMaterialInstance.SetFloat(TransitionAmountID, 0f);
        _transitionCoroutine = null;
    }
}
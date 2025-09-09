using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class TVScreenController : MonoBehaviour
{
    [Header("Screen Content")]
    [SerializeField] private Texture defaultScreenTexture;
    [SerializeField] private List<HeightImageMapping> imageMappings;

    [Header("Audio")]
    [Tooltip("Sound played every time the channel changes.")]
    [SerializeField] private AudioClip channelChangeSFX;
    [Tooltip("Looping background sound for the TV when on a channel.")]
    [SerializeField] private AudioClip tvLoopSound;

    [Header("Transition Effect")]
    [Tooltip("How long the channel change effect lasts in seconds.")]
    [SerializeField] private float transitionDuration = 0.4f;

    private Renderer _screenRenderer;
    private Material _screenMaterialInstance;
    private Coroutine _transitionCoroutine;

    private AudioSource _audioSource;  // TV loop sound
    private AudioSource _sfxSource;    // Channel change sound

    private static readonly int TransitionAmountID = Shader.PropertyToID("_TransitionAmount");
    private static readonly int MainTexID = Shader.PropertyToID("_MainTex");

    private void Awake()
    {
        _screenRenderer = GetComponent<Renderer>();
        _screenMaterialInstance = _screenRenderer.material;
        _screenMaterialInstance.mainTexture = defaultScreenTexture;

        // Main audio for looping TV sound
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.clip = tvLoopSound;

        // Separate source for channel-change SFX
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
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
            if (_transitionCoroutine != null)
                StopCoroutine(_transitionCoroutine);

            _transitionCoroutine = StartCoroutine(ChangeChannelRoutine(textureToDisplay, eventArgs.NewHeight > 0));
        }
    }

    private IEnumerator ChangeChannelRoutine(Texture newTexture, bool channelOn)
    {
        float elapsedTime = 0f;

        // Play the channel-change sound immediately
        if (channelChangeSFX != null)
            _sfxSource.PlayOneShot(channelChangeSFX);

        // Animate the effect IN
        while (elapsedTime < transitionDuration / 2)
        {
            float progress = elapsedTime / (transitionDuration / 2);
            _screenMaterialInstance.SetFloat(TransitionAmountID, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- Switch the channel ---
        _screenMaterialInstance.SetTexture(MainTexID, newTexture);

        // Start or stop looping TV sound
        if (channelOn && tvLoopSound != null)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }

        elapsedTime = 0f;
        while (elapsedTime < transitionDuration / 2)
        {
            float progress = 1.0f - (elapsedTime / (transitionDuration / 2));
            _screenMaterialInstance.SetFloat(TransitionAmountID, progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _screenMaterialInstance.SetFloat(TransitionAmountID, 0f);
        _transitionCoroutine = null;
    }
}



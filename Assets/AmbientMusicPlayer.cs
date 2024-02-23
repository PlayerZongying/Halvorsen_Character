using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AmbientMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    public float clipDuration;

    public AnimationCurve amplitudeDecay;
    
    public float desiredVolume;
    public float fadeSpeed = 5;


    private void Awake()
    {
    }

    private void OnEnable()
    {
        desiredVolume = 1;
        audioSource.volume = Mathf.Lerp(audioSource.volume, desiredVolume, fadeSpeed * Time.deltaTime) *
                             amplitudeDecay.Evaluate(1.0f / clipDuration);
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = Mathf.Lerp(audioSource.volume, desiredVolume, fadeSpeed * Time.deltaTime) *
                             amplitudeDecay.Evaluate(1.0f / clipDuration);
        if (audioSource.volume <= 0.01f)
        {
            gameObject.SetActive(false);
        }
    }
}
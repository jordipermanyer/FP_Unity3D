using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UVic.jordipermanyerandalbertelgstrom.Vgame3D.fps
{
    public class VolumenManagerScript : MonoBehaviour
    {
        public AudioMixer audioMixer; // Reference to the AudioMixer
        public Slider musicSlider; // Slider for music volume
        public Slider vfxSlider; // Slider for VFX volume

        void Start()
        {
            // Initialize sliders with values from AudioSettings
            musicSlider.value = AudioSettings.MusicVolume;
            vfxSlider.value = AudioSettings.SFXVolume;

            // Apply volumes to the AudioMixer
            SetMusicVolume(AudioSettings.MusicVolume);
            SetVFXVolume(AudioSettings.SFXVolume);

            // Add listeners to sliders
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            vfxSlider.onValueChanged.AddListener(SetVFXVolume);
        }

        public void SetMusicVolume(float volume)
        {
            AudioSettings.MusicVolume = volume; // Update static value

            // Convert volume to decibels and set it in the AudioMixer
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB);
        }

        public void SetVFXVolume(float volume)
        {
            AudioSettings.SFXVolume = volume; // Update static value

            // Convert volume to decibels and set it in the AudioMixer
            float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SFXVolume", dB);
        }
    }
}

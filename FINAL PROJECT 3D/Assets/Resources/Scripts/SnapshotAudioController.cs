using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SnapshotAudioController : MonoBehaviour
{

    public AudioMixerSnapshot indoors;
    public AudioMixerSnapshot outdoors;

    private void Start()
    {
        // Default to indoor sound
        indoors.TransitionTo(0); 
    }
    //Snapshot transitions with the lowpass filter
    public void indoorTransition()
    {
        indoors.TransitionTo(1);
    }

    public void outdoorTransition()
    {
        outdoors.TransitionTo(1);
    }

}

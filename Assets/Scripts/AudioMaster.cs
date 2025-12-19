using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMaster : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    public void SwitchMuteMode()
    {
        _audioMixer.GetFloat("MasterVolume", out float volume);
        _audioMixer.SetFloat("MasterVolume", volume == 0 ? -80 : 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //单例
    public AudioSource audioSource; //音频源组件

    void Awake()
    {
        instance = this;
    }
    //播放音频
    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

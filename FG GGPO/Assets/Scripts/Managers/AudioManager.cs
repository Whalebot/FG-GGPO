using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool randomSong;
    public int bgmID;
    public AudioClip[] BGM;
    public AudioSource bgmAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (randomSong)
            bgmID = Random.Range(0, BGM.Length);

        if (bgmID <= BGM.Length - 1)
        {
            bgmAudioSource.clip = BGM[bgmID];
            bgmAudioSource.Play();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}

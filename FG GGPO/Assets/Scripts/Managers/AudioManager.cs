using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool randomSong;
    public static int bgmID = -1;
    public int bgmIDEditor;
    public AudioClip[] BGM;
    public AudioSource bgmAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.gameStartEvent += StartMusic;

    }

    void StartMusic()
    {
        if (bgmIDEditor > 0) bgmID = bgmIDEditor;
        if (bgmID == -1) randomSong = true;

        if (randomSong)
            bgmID = Random.Range(0, BGM.Length - 1);

        bgmAudioSource.clip = BGM[bgmID];
        bgmAudioSource.Play();

    }
}

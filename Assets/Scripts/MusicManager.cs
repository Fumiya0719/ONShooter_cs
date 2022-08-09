using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource song;
    AudioClip Music;
    string title;
    bool played;

    // Start is called before the first frame update
    void Start()
    {
        title = "パンとフィルム";
        song = GetComponent<AudioSource>();
        Music = (AudioClip)Resources.Load("Musics/" + title);
        played = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !played) {
            EntireManager.instance.Start = true;
            EntireManager.instance.StartTime = Time.time;
            played = true;
            song.Play();
        } 
    }
}

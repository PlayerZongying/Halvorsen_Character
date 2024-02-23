using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientMusicPlayerManager : MonoBehaviour
{
    public List<AudioClip> audioClips;
    public List<AmbientMusicPlayer> musicPlayers;
    public GameObject musicPlayerPrefab;
    public AmbientMusicPlayer selectedMusicPlayer = null;
    public int baseCount = 15;
    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayers = new List<AmbientMusicPlayer>();
        for (int i = 0; i < baseCount; i++)
        {
            GameObject newMusicPlayer = Instantiate(musicPlayerPrefab, Vector3.zero, Quaternion.identity, transform);
            musicPlayers.Add(newMusicPlayer.GetComponent<AmbientMusicPlayer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedMusicPlayer = FindNextSleepingMusicPlayer();
            selectedMusicPlayer.audioSource.clip = audioClips[Random.Range(0, audioClips.Count)];
            selectedMusicPlayer.gameObject.SetActive(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedMusicPlayer.desiredVolume = 0;
        }
    }

    AmbientMusicPlayer FindNextSleepingMusicPlayer()
    {
        foreach (var ambientMusicPlayer in musicPlayers)
        {
            if (ambientMusicPlayer.gameObject.activeSelf == false)
            {
                return ambientMusicPlayer;
            }
        }
        
        GameObject newMusicPlayer = Instantiate(musicPlayerPrefab, Vector3.zero, Quaternion.identity, transform);
        AmbientMusicPlayer newAmbientMusicPlayer = newMusicPlayer.GetComponent<AmbientMusicPlayer>();
        musicPlayers.Add(newAmbientMusicPlayer);
        return newAmbientMusicPlayer;
    }
}

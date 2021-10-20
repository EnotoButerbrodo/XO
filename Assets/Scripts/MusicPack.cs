using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPack : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]public MusicPackType packType;
    [SerializeField]public List<AudioClip> musics;
    

    public enum MusicPackType{
        Turn,
        Win,
        Lose
    }

    public AudioClip GetRandom(){
        return musics[Random.Range(0, musics.Count)];
    }
}

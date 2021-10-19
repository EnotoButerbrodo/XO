using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]public Sprite appearance;
    [SerializeField]public string Name;
    [SerializeField]public int Id;
    [SerializeField]public AudioSource turnAudio;
    [SerializeField]public AudioSource winAudio;

}

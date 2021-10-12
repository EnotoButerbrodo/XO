using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]private GameObject[] colliders;
    [SerializeField]private Sprite Players_1_Sprite;
    [SerializeField]private Sprite Players_2_Sprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    
    public void SetCageOwner(GameObject cage, int playerId){
        cage.GetComponent<SpriteRenderer>().sprite = Players_1_Sprite;

    }
}

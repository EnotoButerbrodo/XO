using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLogic : MonoBehaviour
{
    [SerializeField] private List<GameObject> cages;
    private Cage[,] GameField;
        // Start is called before the first frame update
    void Start()
    {
        if(cages.Count != 9){
            throw new System.Exception("Пошел нахуй");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

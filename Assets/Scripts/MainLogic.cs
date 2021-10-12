using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        if( Input.GetMouseButtonDown(0) )
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if(rayHit.transform != null){
                var cage = rayHit.transform.GetComponent<Cage>();
                if(cage != null){
                    Debug.Log(cage.Id);
                }
                else   
                    Debug.Log("Не cage");
            }
        }
    }

    public void SetCageOwner(Cage cage, int playerId){
        cage.Id = playerId;
    }
}

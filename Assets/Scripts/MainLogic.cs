using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainLogic : MonoBehaviour
{
    [SerializeField] private Character[] characters;
    [SerializeField] private List<Cage> cages;

    [SerializeField]private Character currentTurnCharacter;
    private Cage[,] GameField;


        // Start is called before the first frame update
    void Start()
    {
        if(cages.Count != 9){
            throw new System.Exception("Пошел нахуй");
        }
        FillGameField(); 
        ChooseFirstCharacter();
    }
    void FillGameField()
    {
        GameField = new Cage[3,3];
        foreach(Cage cage in cages){
            
            GameField[cage.coordinates.x, cage.coordinates.y] = cage;
            Debug.Log(GameField[cage.coordinates.x, cage.coordinates.y]);
        }
        
    }
    void ChooseFirstCharacter(){
        currentTurnCharacter = characters[0];
    }

    void Update()
    {
        if( Input.GetMouseButtonDown(0) )
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if(rayHit.transform != null){
                var cage = rayHit.transform.GetComponent<Cage>();
                if(!(cage is null)){
                   CageClickLogic(cage);
                }
                else   
                    Debug.Log("Не cage");
            }
        }
    }

    void CageClickLogic(Cage cage)
    {
        if(!cage.IsFilled){
            SetCageOwner(cage, currentTurnCharacter);
            NextTurnLogic();
        }
    }
    void SetCageOwner(Cage cage, Character character)
    {
        cage.OwnerId = currentTurnCharacter.Id;
        cage.IsFilled = true;
    }

    void NextTurnLogic()
    {
        currentTurnCharacter = characters.Where(character=> character !=currentTurnCharacter).First();
        CheckWin(out Character chara);
        Debug.Log(currentTurnCharacter.Name);
    }

    bool CheckWin(out Character winner){
        //Проверить столбцы
        for(int row = 0; row < 3; row++){
            int avverage = 0;
            for(int column = 0; column < 3; column++)
            {
                avverage += GameField[row, column].OwnerId;
            }
            avverage /= 3;
            Debug.Log($"avverage {avverage}");

        }
        winner = null;
        return true;
    }


}

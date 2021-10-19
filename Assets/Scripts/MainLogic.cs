using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainLogic :  MonoBehaviour
{
    [SerializeField] private Character[] characters;
    [SerializeField] private GameObject gameField;
    private List<Cage> cages;

    [SerializeField]private Character currentTurnCharacter;

        // Start is called before the first frame update
    void Start()
    {
        cages = gameField.GetComponentsInChildren<Cage>()
                                               .ToList();
        if(cages.Count != 9){
            throw new System.Exception("Пошел нахуй");
        }
        ChooseFirstCharacter();
    }
    void FillGameField()
    {

        
    }
    void ChooseFirstCharacter(){
        currentTurnCharacter = characters.Where(character=> character.name.Contains("Jotaro")).First();
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
        if(!cage.IsFilled)
        {
            SetCageOwner(cage, characters[0]);
            CheckWin();
           // EnemyTurnLogic();
           // CheckWin();
        }
        
    }
    void SetCageOwner(Cage cage, Character character)
    {
        cage.OwnerId = character.Id;
        cage.IsFilled = true;
        SpriteRenderer renderer = cage.gameObject.GetComponentsInChildren<SpriteRenderer>()
        .Where(sr=> sr.gameObject.name == "Main").FirstOrDefault();

        if(renderer is SpriteRenderer)
            renderer.sprite = character.appearance;
    }

   

    void EnemyTurnLogic(){
        
        var freeCages = cages.Where(cage=> cage.IsFilled == false);
        Cage turnCage = freeCages.Skip(Random.Range(0, freeCages.Count())).First();
        SetCageOwner(turnCage, characters[1]);
    }

    bool CheckWin(){
        //Проверить столбцы
        var gameStats =cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.row})
                    .Select(group=> new {OwnerId = group.Key.OwnerId, CageCount = group.Count(), row = group.Key.row})
                    .ToList();
           gameStats.ForEach(group => Debug.Log($"row {group.row}) owner:{group.OwnerId} count:{group.CageCount}"));
           Debug.Log(new string('-', 50));
       return false;
    }


}

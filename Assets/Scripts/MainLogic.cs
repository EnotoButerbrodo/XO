using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainLogic :  MonoBehaviour
{
    [SerializeField] private Character[] characters;
    [SerializeField] private GameObject gameField;
    private List<Cage> cages;
    [SerializeField]int Rows;
    [SerializeField]int Collumns;
    [SerializeField]int UpDiagonal;
    [SerializeField]int DownDiagonal;

     struct Stat{
        public int OwnerId; 
        public int CageCount; 
        public string Entity; 
        public int EntityCount;

        public override string ToString()
        {
            return $"Owner:{OwnerId} CageCount:{CageCount} {Entity}: {EntityCount}";
        }
    }

    List<Stat> GameStats;
    [SerializeField]private Character currentTurnCharacter;

        // Start is called before the first frame update
    void Start()
    {
        cages = gameField.GetComponentsInChildren<Cage>()
                                               .ToList();
        Rows = cages.GroupBy(prop=> prop.coordinates.row).Count();
        Collumns = cages.GroupBy(prop=> prop.coordinates.column).Count();
        UpDiagonal = cages.Where(prop=> prop.coordinates.column == prop.coordinates.row)
                .GroupBy(prop=> prop).Count();
        DownDiagonal = cages.Where(prop=> ((Collumns - 1) - prop.coordinates.column) == prop.coordinates.row)
                .GroupBy(prop=> prop).Count();
        
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
            EnemyTurnLogic();
            CheckWin();
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

    
    bool CheckWin()
    {
        //Rows
        GetGameStats();
        //Проверить строки
        var rowResult = GameStats.Where(stat=> stat.Entity == "row" && stat.CageCount == Rows)
                .FirstOrDefault();
       if(!rowResult.Equals(default(Stat)))
           Debug.Log(rowResult);
       

       var columnResult = GameStats.Where(stat=> stat.Entity == "column" && stat.CageCount == Collumns)
                .FirstOrDefault();
        if(!columnResult.Equals(default(Stat)))
           Debug.Log(columnResult);
       
       var UpDiagonalResult = GameStats.Where(stat=> stat.Entity == "Up Diagonal" && stat.CageCount == UpDiagonal)
                .FirstOrDefault();
        if(!UpDiagonalResult.Equals(default(Stat)))
           Debug.Log(UpDiagonalResult);

        var DownDiagonalResult = GameStats.Where(stat=> stat.Entity == "Down Diagonal" && stat.CageCount == DownDiagonal)
                .FirstOrDefault();
        if(!DownDiagonalResult.Equals(default(Stat)))
           Debug.Log(DownDiagonalResult);




       return false;
    }

    void GetGameStats(){
        GameStats =
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.row})
                    .Select(group=> new Stat{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Entity = "row", EntityCount = group.Key.row})
                    .ToList();
        //Columns
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.column})
                    .Select(group=> new Stat{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Entity = "column", EntityCount = group.Key.column})
                    .ToList());
        //Up diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && cage.coordinates.column == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new Stat{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Entity = "Up Diagonal", EntityCount = 0})
                    .ToList());
        //Down diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && ((Collumns - 1) - cage.coordinates.column) == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new Stat{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Entity = "Down Diagonal", EntityCount = 1})
                    .ToList());
        //GameStats.ForEach(group => Debug.Log($"{group.Entity} {group.EntityCount}: owner:{group.OwnerId} count:{group.CageCount}"));
        //Debug.Log(new string('=', 100));
    }

   
}

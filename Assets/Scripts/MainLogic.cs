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

    struct WinPattern{
        public int OwnerId; 
        public int CageCount; 
        public PatternType Pattern; 
        public int PatternCount;

        public enum PatternType{
        Row,
        Collumn,
        UpDiagonal,
        DownDiagonal
        }

        public override string ToString()
        {
            return $"Owner:{OwnerId} CageCount:{CageCount} {Pattern}: {PatternCount}";
        }
    }
    
    

    List<WinPattern> GameStats;
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
        var rowResult = GameStats.Where(stat=> stat.Pattern == WinPattern.PatternType.Row && stat.CageCount == Rows)
                .FirstOrDefault();
       if(!rowResult.Equals(default(WinPattern)))
           Debug.Log(rowResult);
       

       var columnResult = GameStats.Where(stat=> stat.Pattern == WinPattern.PatternType.Collumn && stat.CageCount == Collumns)
                .FirstOrDefault();
        if(!columnResult.Equals(default(WinPattern)))
           Debug.Log(columnResult);
       
       var UpDiagonalResult = GameStats.Where(stat=> stat.Pattern == WinPattern.PatternType.UpDiagonal && stat.CageCount == UpDiagonal)
                .FirstOrDefault();
        if(!UpDiagonalResult.Equals(default(WinPattern)))
           Debug.Log(UpDiagonalResult);

        var DownDiagonalResult = GameStats.Where(stat=> stat.Pattern == WinPattern.PatternType.DownDiagonal && stat.CageCount == DownDiagonal)
                .FirstOrDefault();
        if(!DownDiagonalResult.Equals(default(WinPattern)))
           Debug.Log(DownDiagonalResult);

       return false;
    }

    

    void GetGameStats(){
        GameStats =
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.row})
                    .Select(group=> new WinPattern{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPattern.PatternType.Row, PatternCount = group.Key.row})
                    .ToList();
        //Columns
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.column})
                    .Select(group=> new WinPattern{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPattern.PatternType.Collumn, PatternCount = group.Key.column})
                    .ToList());
        //Up diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && cage.coordinates.column == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new WinPattern{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPattern.PatternType.UpDiagonal, PatternCount = 0})
                    .ToList());
        //Down diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && ((Collumns - 1) - cage.coordinates.column) == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new WinPattern{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPattern.PatternType.DownDiagonal, PatternCount = 1})
                    .ToList());
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class MainLogic :  MonoBehaviour
{
    [SerializeField] private Character[] characters;
    [SerializeField] private GameObject gameField;
    private List<Cage> cages;

    struct WinPatternInfo
    {
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
    
    [SerializeField]Dictionary<WinPatternInfo.PatternType, int> PatternsGoal = new Dictionary<WinPatternInfo.PatternType, int>();

    List<WinPatternInfo> GameStats;
    [SerializeField]private Character currentTurnCharacter;

        // Start is called before the first frame update
    void Start()
    {
        cages = gameField.GetComponentsInChildren<Cage>()
                                               .ToList();
        PatternsGoal.Add(WinPatternInfo.PatternType.Row
                ,cages.GroupBy(prop=> prop.coordinates.row).Count());
        PatternsGoal.Add(WinPatternInfo.PatternType.Collumn
                ,cages.GroupBy(prop=> prop.coordinates.column).Count());
        PatternsGoal.Add(WinPatternInfo.PatternType.UpDiagonal
                ,cages.Where(prop=> prop.coordinates.column == prop.coordinates.row)
                .GroupBy(prop=> prop).Count());
        PatternsGoal.Add(WinPatternInfo.PatternType.DownDiagonal
                ,cages.Where(prop=> ((PatternsGoal[WinPatternInfo.PatternType.Collumn] - 1) - prop.coordinates.column) == prop.coordinates.row)
                .GroupBy(prop=> prop).Count());
        
        foreach(var goal in PatternsGoal){
            Debug.Log($"{goal.Key} {goal.Value}");
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
        if(ChechPatterns(out WinPatternInfo winPattern)){
            Debug.Log(winPattern);
            EditorUtility.DisplayDialog("Победа", 
                characters.Where(character=> character.Id == winPattern.OwnerId)
                    .Select(character=> character.Name)
                    .First(), "Заебись");
        }

       return false;
    }

    bool ChechPatterns(out WinPatternInfo winPattern){
        winPattern = GameStats.Where(wP => wP.CageCount == PatternsGoal[wP.Pattern])
                    .FirstOrDefault();
        if(!winPattern.Equals(default(WinPatternInfo)))
            return true;

        return false;
    }
    


    void GetGameStats(){
        GameStats =
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.row})
                    .Select(group=> new WinPatternInfo{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPatternInfo.PatternType.Row, PatternCount = group.Key.row})
                    .ToList();
        //Columns
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0)
                    .GroupBy(group=> new {group.OwnerId, group.coordinates.column})
                    .Select(group=> new WinPatternInfo{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPatternInfo.PatternType.Collumn, PatternCount = group.Key.column})
                    .ToList());
        //Up diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && cage.coordinates.column == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new WinPatternInfo{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPatternInfo.PatternType.UpDiagonal, PatternCount = 0})
                    .ToList());
        //Down diagonal
        GameStats.AddRange(
                    cages.Where(cage=> cage.OwnerId != 0 && ((PatternsGoal[WinPatternInfo.PatternType.Collumn] - 1) - cage.coordinates.column) == cage.coordinates.row)
                    .GroupBy(group=> new {group.OwnerId})
                    .Select(group=> new WinPatternInfo{OwnerId = group.Key.OwnerId, CageCount = group.Count(), 
                                        Pattern = WinPatternInfo.PatternType.DownDiagonal, PatternCount = 1})
                    .ToList());
    }

   
}

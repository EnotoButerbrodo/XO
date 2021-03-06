using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MainLogic :  MonoBehaviour
{
    [SerializeField] private Sprite defaulCageSprite;
    [SerializeField] private Character[] characters;
    [SerializeField] private GameObject gameField;
    [SerializeField] private AudioSource audioSource;
    private List<Cage> cages;
    [SerializeField]Character currentCharacter;
    bool MayTurn = true;
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



        // Start is called before the first frame update
    void Start()
    {
        AnalysGameField();
        SetFirtsPlayer();
    }

    void SetFirtsPlayer(){
        currentCharacter = characters[Random.Range(0, characters.Count())];
    }

    void AnalysGameField()
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
    }
    

    void Update()
    {
        if( Input.GetMouseButtonDown(0) )
        {
            RaycastHit2D rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if(rayHit.transform != null){
                var cage = rayHit.transform.GetComponent<Cage>();
                if(!(cage is null)){
                   StartCoroutine(CageClickLogic(cage));
                }
                else   
                    Debug.Log("???? cage");
            }
        }
    }

    IEnumerator CageClickLogic(Cage cage)
    {
        if(!cage.IsFilled && MayTurn){
            SetCageOwner(cage, currentCharacter);
            currentCharacter = characters
                        .Where(Character => Character!= currentCharacter).First();
            yield return new WaitForSecondsRealtime(0.5f); 
            if(CheckWin()) 
            {
                    yield return new WaitForSecondsRealtime(5);
                    ClearGameField();
            }   
            
            /*
            if(MayTurn){
                MayTurn = false;
                SetCageOwner(cage, characters[0]);
                yield return new WaitForSecondsRealtime(0.5f); 
                if(CheckWin()) 
                {
                    yield return new WaitForSecondsRealtime(5);
                    //SceneManager.LoadScene("FirstBattle");
                    ClearGameField();
                    MayTurn = true;
                }   
                yield return new WaitForSecondsRealtime(0.5f); 
                EnemyTurnLogic();
                yield return new WaitForSecondsRealtime(0.5f); 
                if(CheckWin())  
                {
                    yield return new WaitForSecondsRealtime(5);
                    //SceneManager.LoadScene("FirstBattle");
                    ClearGameField();
                    MayTurn = true;
                }
                yield return new WaitForSecondsRealtime(0.5f);
                MayTurn = true;
            }
            */
           
        }
        
    }

    void SetCageOwner(Cage cage, Character character)
    {
        cage.OwnerId = character.Id;
        cage.IsFilled = true;
        SpriteRenderer renderer = cage.gameObject.GetComponentsInChildren<SpriteRenderer>()
        .Where(sr=> sr.gameObject.name == "Main").FirstOrDefault();
        //character.turnAudio.Play();
        PlayRandomAudio(character, MusicPack.MusicPackType.Turn);
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
        if(CheckPat()){
            EditorUtility.DisplayDialog("OYAYAYAAAI",
            "Pat", "Ok");
            ClearGameField();
            return false;
        }
        //Rows
        GetGameStats();
        //?????????????????? ????????????
        if(ChechPatterns(out WinPatternInfo winPattern)){
            StartCoroutine(Win(winPattern.OwnerId));
            return true;
        }

       return false;
    }

    bool CheckPat(){
        return cages.Where(cage=> !cage.IsFilled).Count() == 0;
    }
    IEnumerator Win(int id)
    {
        MayTurn = false;
        Character winner = characters.Where(character=> character.Id == id).First();
        PlayRandomAudio(winner, MusicPack.MusicPackType.Win);
        yield return new WaitForSecondsRealtime(5);
        EditorUtility.DisplayDialog("????????????", 
                winner.Name, "??????????????");
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

   void ClearGameField(){
       cages.ForEach(cage=> {
           cage.OwnerId = 0;
           cage.gameObject.GetComponentsInChildren<SpriteRenderer>()
            .Where(sr=> sr.gameObject.name == "Main").FirstOrDefault().sprite = defaulCageSprite;
            
           cage.IsFilled = false;
       });
       MayTurn = true;
   }

   AudioClip GetRandomAudio(Character character, MusicPack.MusicPackType type){
       return currentCharacter.musicPacks
            .Where(musicPack=> musicPack.packType == type)
            .First().GetRandom();
   }

   void PlayAudioClip(AudioClip clip){
       audioSource.PlayOneShot(clip);
   }

   void PlayRandomAudio(Character character, MusicPack.MusicPackType type){
       PlayAudioClip(character.musicPacks
            .Where(musicPack=> musicPack.packType == type)
            .First().GetRandom());
   }
}

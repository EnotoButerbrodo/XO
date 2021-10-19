
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public int Id;
    public int OwnerId;
    [SerializeField]public Point coordinates;
    public bool IsFilled = false;

    public Cage(int Id, int OwnerId, Point coordinates){
        this.Id = Id;
        this.OwnerId = OwnerId;
        this.coordinates = coordinates;
    }
   
}
[System.Serializable]
public struct Point{
    public int row;
    public int column;
    public Point(int row, int column){
        this.row = row;
        this.column = column;
    }
}

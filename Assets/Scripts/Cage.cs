
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public int Id;
    public int OwnerId;
    [SerializeField]public Point coordinates;

    public Cage(int Id, int OwnerId, Point coordinates){
        this.Id = Id;
        this.OwnerId = OwnerId;
        this.coordinates = coordinates;
    }
   
}
[System.Serializable]
public struct Point{
    public int x;
    public int y;
    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }
}

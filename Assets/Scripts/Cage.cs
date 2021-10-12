using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public int Id;
    public int OwnerId;
    public Point coordinates;

    public Cage(int Id, int OwnerId, Point coordinates){
        this.Id = Id;
        this.OwnerId = OwnerId;
        this.coordinates = coordinates;
    }
    
}

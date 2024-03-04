using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    //test purpose:
    private int pointCount = 5;
    private int finalPointIndex = 2; //这个一定要根据实际情况修改！哪一个是最后一个按钮！

    public List<Puzzle2_Point> points = new List<Puzzle2_Point>(); //This List has to be in the order of correct clicking
    //public List<bool> pointsClicked = new List<bool>();

    private void Awake()
    {
        SetAllToFalse();
    }

    private void SetAllToFalse()
    {
        for (int i = 0; i < pointCount; i++)
        {
            points[i].clicked = false;
        }
    }

    private bool CheckIfAllTrue() //Except for the last one in the list!!
    {
        for (int i = 0; i < pointCount-1; i++)
        {
            if (points[i].clicked == false)
            {
                return false;
            }
            
        }
        return true;
    }

    public bool CheckVadality(int index)
    {
        for (int i = 0; i < index; i++)
        {
            if (points[i].clicked) //玩家点击错误
            {
                SetAllToFalse();
                return false;
            }
        }
        //玩家到目前为止点击正确
        if (index == finalPointIndex && CheckIfAllTrue())
        {
            print("You Win the Puzzle! (add more codes here for the next step)");
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    //test purpose:
    private int pointCount = 5;

    public List<Puzzle2_Point> points = new List<Puzzle2_Point>(); //This List has to be in the order of correct clicking

    private void Awake()
    {
        SetAllToFalse();
    }

    private void SetAllToFalse()
    {
        for (int i = 0; i < pointCount; i++)
        {
            points[i].clicked = false;
            points[i].toOriginalSpr();
        }
    }

    private bool CheckIfAllTrue() //Except for the last one in the list!!
    {
        for (int i = 0; i < pointCount-1; i++)
        {
            if (points[i].clicked == false)
            {
                //print("check is all true is FALSE");
                return false;
            }
        }
        return true;
    }

    public bool CheckVadality(int num)
    {
        int posAtList = 10;
        //find with Point's position at the List by its own given index
        for (int i = 0; i < pointCount; i++)
        {
            if (points[i].index == num)
            {
                posAtList = i;
                break;
            }
        }
        //print("posAtList = " + posAtList);

            for (int i = 0; i < posAtList; i++)
        {
            if (!points[i].clicked) //玩家点击错误
            {
                print("Wrong point clicked!");
                SetAllToFalse();
                return false;
            }
        }
        //玩家到目前为止点击正确
        if (posAtList == (pointCount-1) && CheckIfAllTrue())
        {
            print("You Win the Puzzle! (add more codes here for the next step)");
        }
        return true;
    }
}

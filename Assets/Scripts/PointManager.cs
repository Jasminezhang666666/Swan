using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private int pointCount = 5; //how many points there are in total
    private bool isWrong = false; //used to check is the player clicked things wrong

    public List<Puzzle2_Point> points = new List<Puzzle2_Point>(); //This List has to be in the order of correct clicking

    private void Start()
    {
        SetAllToFalse();
    }

    private void SetAllToFalse()
    {
        for (int i = 0; i < pointCount; i++)
        {
            points[i].chosed = false;
            points[i].toOriginalSpr();
        }
    }

    private bool CheckIfAllTrue() //Except for the last one in the list!!
    {
        int count = 0;
        for (int i = 0; i < pointCount; i++)
        {
            if (points[i].chosed)
            {
                count++;
            }
        }

        if (count < pointCount)
        {
            return false;
        }
        return true;
    }

    public void CheckVadality(int num)
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
            if (!points[i].chosed) //玩家点击错误
            {
                isWrong = true;
            }
        }
        //最后一步check
        if (CheckIfAllTrue())
        {
            if (isWrong)
            {
                print("The order is wrong!");
                SetAllToFalse();
                isWrong = false;
            } else
            {
                print("You Win the Puzzle! (add more codes here for the next step)");
            }
        }
    }
}

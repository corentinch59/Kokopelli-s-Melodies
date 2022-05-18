using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villagers : MonoBehaviour
{
    private IVillagersBehavior _villagersBehavior;

    private void Start()
    {
        
    }

    private void Update()
    {
        float rand = Random.Range(1.0f, 3.0f);
        _villagersBehavior.ExecuteBehavior(rand);
    }

    private bool CheckOBB(Vector3 villagerPosition, List<Transform> pointList)
    {
        bool isInside = false;
        List<float> floatList = new();
        for (int i = 0; i < pointList.Count; i++)
        {
            Vector3 mouseVect = villagerPosition - pointList[i].position;
            if (i == pointList.Count - 1)
            {
                Vector3 vect = pointList[0].position - pointList[i].position;
                float dotproduct = Vector3.Dot((Vector3)Vector2.Perpendicular(vect), mouseVect);
                floatList.Add(dotproduct);
            }
            else
            {
                Vector3 vect = pointList[i + 1].position - pointList[i].position;
                float dotproduct = Vector3.Dot((Vector3)Vector2.Perpendicular(vect), mouseVect);
                floatList.Add(dotproduct);
            }
        }

        uint nbOfPositive = 0;
        for (int i = 0; i < floatList.Count; i++)
        {
            if (floatList[i] > 0)
                nbOfPositive += 1;
        }

        if (nbOfPositive != 0)
            isInside = false;
        else
        {
            isInside = true;
        }

        return isInside;
    }
}

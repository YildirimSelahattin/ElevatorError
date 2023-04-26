using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataList
{
    [System.Serializable]
    public class GeneralDataStructure
    {
        public int gridHeight;
        public int gridWidth;
        public int totalFloor;
        public List<People> peopleList;
        public List<BoardingPeople> boardingPeopleList;
    }

    public GeneralDataStructure[] elevatorArray;
}
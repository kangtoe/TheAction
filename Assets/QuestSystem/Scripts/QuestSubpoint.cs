using System;
using UnityEngine;
using FastCampus.QuestSystem;

public enum QuestsubpointType
{ 
    DestroyEnemy,
    GetItem
}

//public enum QuestSubpointStatus
//{
//    Uncompleted,
//    Completed,    
//}

// 퀘스트 목표. 하나의 퀘스트는 여러 퀘스트 목표를 가질 수 있다.
[Serializable]
public class QuestSubpoint
{
    //public QuestSubpointStatus status; // 각각 목표의 진행상황
    public QuestsubpointType type; // 목표 타입

    public int targetID; // 목표 대상(아이템, 적)

    public bool isCompleted => completedCount >= targetCount;

    [Min(0)]
    public int targetCount; // 처치하거나 취득해야하는 적/아이템 수량.

    [Min(0)]
    public int completedCount; // 처치하거나 취득한 적/아이템 수량.
}

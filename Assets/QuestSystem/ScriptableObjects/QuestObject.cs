using System;
using UnityEngine;

namespace FastCampus.QuestSystem
{
    [CreateAssetMenu(fileName ="New Quest", menuName = "Quest System/Quests/New Quest")]
    public class QuestObject : ScriptableObject
    {
        public Quest data = new Quest();

        public QuestStatus status;        

        // for debug
        void OnEnable() {
            //status = QuestStatus.None;
        }
    }
}
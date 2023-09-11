using UnityEngine;
using System.Collections;
using System;

namespace FastCampus.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        #region Variables
        private static QuestManager instance;

        public QuestDatabaseObject allQuestDatabase;
        //public QuestDatabaseObject acceptedQuestDatabass;

        public event Action<QuestObject> OnCompletedQuest;

        #endregion Variables

        #region Properties
        public static QuestManager Instance => instance;
        #endregion Properties

        #region Unity Methods
        private void Awake()
        {
            instance = this;
        }
        #endregion Unity Methods

        #region Methods
        //public void ProcessQuest(QuestType type, int targetId)
        //{
        //    Debug.Log("ProcessQuest");

        //    foreach (QuestObject questObject in allQuestDatabase.questObjects)
        //    {
        //        if (questObject.status == QuestStatus.Accepted && questObject.data.type == type && questObject.data.targetId == targetId)
        //        {
        //            Debug.Log("ProcessQuest : Accepted");

        //            questObject.data.completedCount++;
        //            if (questObject.data.completedCount >= questObject.data.count)
        //            {
        //                questObject.status = QuestStatus.Completed;
        //                OnCompletedQuest?.Invoke(questObject);
        //            }
        //        }
        //    }
        //}

        // 퀘스트 진행 상황 갱신, 완료 시 handler 호출
        public void ProcessQuest(QuestsubpointType subpointType, int targetId)
        {
            Debug.Log("ProcessQuest 0");

            foreach (QuestObject questObject in allQuestDatabase.questObjects)
            {
                // 현재 진행 중인 퀘스트만
                if (questObject.status != QuestStatus.Accepted) return;
                Debug.Log("ProcessQuest 1");
                
                foreach (QuestSubpoint subpoint in questObject.data.subpoints)
                {
                    // 현재 진행중인 퀘스트 세부 목적에 알맞는 타입과 목표일 경우만
                    if (subpoint.type != subpointType) continue;
                    if (subpoint.targetID != targetId) continue;
                    if (subpoint.isCompleted) continue;

                    Debug.Log("ProcessQuest 2");                    

                    // 처치하거나 취득해야하는 적/아이템 수량 더하기.
                    subpoint.completedCount++;                    
                }

                // 완료된 세부목표 수 구하기
                int completedSubpoint = 0;
                foreach (QuestSubpoint subpoint in questObject.data.subpoints)
                {
                    if (subpoint.isCompleted) completedSubpoint++;
                }

                Debug.Log("completedSubpoint : " + completedSubpoint);

                // 모든 세부 목표 완료 시 퀘스트 완료
                if (completedSubpoint == questObject.data.subpoints.Length)
                {
                    Debug.Log("ProcessQuest 3");

                    OnCompletedQuest?.Invoke(questObject);
                }                
            }
        }
        #endregion Methods
    }
}
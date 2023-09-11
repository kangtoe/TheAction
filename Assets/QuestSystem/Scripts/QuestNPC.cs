using FastCampus.Core;
using FastCampus.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.QuestSystem
{
    public class QuestNPC : MonoBehaviour, IInteractable
    {
        #region Variables
        //public int questId = -1;
        
        Animator animator;
        readonly int interactTiriggerHash = Animator.StringToHash("InteractTrigger");

        public QuestObject questObject;

        public Dialogue readyDialogue;
        public Dialogue acceptedDialogue;
        public Dialogue completedDialogue;

        bool isStartQuestDialogue = false;
        GameObject interactGO = null;

        [SerializeField]
        private GameObject questEffectGO;
        [SerializeField]
        private GameObject questProcessGO;
        [SerializeField]
        private GameObject questRewardGO;
        
        // 현재 대화중인 상대
        public GameObject interactTarget;

        #endregion Variables

        #region Unity Methods
        private void Start()
        {
            animator = GetComponent<Animator>();

            questEffectGO.SetActive(false);
            questProcessGO.SetActive(false);
            questRewardGO.SetActive(false);
            
            if (questObject.status == QuestStatus.None)
            {
                questEffectGO.SetActive(true);
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                questProcessGO.SetActive(true);
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                questRewardGO.SetActive(true);
            }

            QuestManager.Instance.OnCompletedQuest += OnCompletedQuest;            
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, distance);
        }
        #endregion Unity Methods

        #region IInteractable Interface
        public float distance = 2.0f;
        public float Distance => distance;

        public void Interact(GameObject other)
        {            
            if (interactTarget != null) return;
            else interactTarget = other;

            Debug.Log("interact 1");

            float calcDistance = Vector3.Distance(other.transform.position, transform.position);
            if (calcDistance > Distance)
            {
                return;
            }

            Debug.Log("interact2");

            if (isStartQuestDialogue)
            {
                return;
            }

            Debug.Log("interact3");

            interactGO = other;
            isStartQuestDialogue = true;

            animator.SetTrigger(interactTiriggerHash);

            //if (questObject.status == QuestStatus.None)
            //{
            //    DialogueManager.Instance.StartDialogue(readyDialogue);
            //    questObject.status = QuestStatus.Accepted;
            //    AcceptedQuest(questObject);
            //}
            //else if (questObject.status == QuestStatus.Accepted)
            //{
            //    DialogueManager.Instance.StartDialogue(acceptedDialogue);                
            //}
            //else if (questObject.status == QuestStatus.Completed)
            //{
            //    // Reward quest
            //    DialogueManager.Instance.StartDialogue(completedDialogue);
            //    questObject.status = QuestStatus.Rewarded;
            //    questEffectGO.SetActive(false);
            //    questProcessGO.SetActive(false);
            //    questRewardGO.SetActive(false);
            //}

            if (questObject.status == QuestStatus.None)
            {
                DialogueManager.Instance.StartDialogue(readyDialogue);
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                DialogueManager.Instance.StartDialogue(acceptedDialogue);
            }
            else if (questObject.status == QuestStatus.Completed)
            {                
                DialogueManager.Instance.StartDialogue(completedDialogue);
            }

            Debug.Log("interact4");

            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;            
        }

        public void StopInteract(GameObject other)
        {
            Debug.Log("StopInteract");

            DialogueManager.Instance.OnEndDialogue -= OnEndDialogue;

            isStartQuestDialogue = false;

            PlayerCharacter playerCharacter = other?.GetComponent<PlayerCharacter>();
            if (playerCharacter)
            {
                playerCharacter.RemoveTarget();
            }

            interactTarget = null;
        }
        #endregion IInteractable Interface

        #region Methods
        private void OnEndDialogue()
        {
            Debug.Log("OnEndDialogue 0");

            // 대화 시작 시점 퀘스트 진행 => 대화 종료 후 퀘스트 진행
            if (questObject.status == QuestStatus.None)
            {                
                questObject.status = QuestStatus.Accepted;
                questEffectGO.SetActive(false);
                questProcessGO.SetActive(true);
                questRewardGO.SetActive(false);
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                Debug.Log("OnEndDialogue 1");

                if (RewardQuest())
                {
                    Debug.Log("OnEndDialogue 2");

                    questObject.status = QuestStatus.Rewarded;
                    questEffectGO.SetActive(false);
                    questProcessGO.SetActive(false);
                    questRewardGO.SetActive(false);
                }                
            }

            StopInteract(interactGO);
        }

        //private void OnAcceptedQuest(QuestObject questObject)
        //{            
        //    if (questObject.data.id == this.questObject.data.id && questObject.status == QuestStatus.Accepted)
        //    {
        //        questEffectGO.SetActive(false);
        //        questProcessGO.SetActive(true);
        //        questRewardGO.SetActive(false);
        //    }
        //}

        private void OnCompletedQuest(QuestObject questObject)
        {
            Debug.Log("OnCompletedQuest");

            if (questObject.data.id != this.questObject.data.id) return;
            if (questObject.status != QuestStatus.Accepted) return;

            questObject.status = QuestStatus.Completed;
            questEffectGO.SetActive(false);
            questProcessGO.SetActive(false);
            questRewardGO.SetActive(true);
        }

        bool RewardQuest()
        {
            Debug.Log("RewardQuest");            

            PlayerCharacter playerCharacter = interactTarget.GetComponent<PlayerCharacter>();
            return playerCharacter?.PickupItems(questObject.data.rewardItems) ?? false;            
        }
        #endregion Methods
    }
}
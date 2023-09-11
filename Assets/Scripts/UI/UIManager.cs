using FastCampus.InventorySystem.Items;
using FastCampus.InventorySystem.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public ItemDatabaseObject itemDatabase;

    //public StaticInventoryUI equipmentUI;
    //public DynamicInventoryUI inventoryUI;
    public DynamicInventoryUI_New inventoryUI;

    [SerializeField]
    private GameObject dieUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }

        if (Input.GetKeyDown("e"))
        {
            //equipmentUI.gameObject.SetActive(!equipmentUI.gameObject.activeSelf);
        }

        if (Input.GetKeyDown("r"))
        {
            PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
            if (player && !player.IsAlive) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void DisplayDieUI()
    {
        dieUI.SetActive(true);
    }
}

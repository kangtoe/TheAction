using FastCampus.InventorySystem.Inventory;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats", menuName = "Stats System/New Character Stats")]
public class StatsObject : ScriptableObject
{
    public Attribute[] attributes;

    public int level;
    public int exp;

    public Action<StatsObject> OnChangedStats;

    public int MaxHealth
    {
        get { return GetModifiedValue(AttributeType.Health); }
    }

    public int MaxMana
    {
        get { return GetModifiedValue(AttributeType.Mana); }
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (value > MaxHealth) value = MaxHealth;
            health = value;
        }
    }

    public int Mana
    {
        get
        {
            return mana;
        }
        set
        {
            if (value > MaxMana) value = MaxMana;
            mana = value;
        }
    }

    int health;
    int mana;


    public float HealthPercentage
    {
        get
        {
            return (MaxHealth > 0 ? ((float)Health / (float)MaxHealth) : 0f);
        }
    }
    public float ManaPercentage
    {
        get
        {
            return (MaxMana > 0 ? ((float)Mana / (float)MaxMana) : 0f);
        }
    }

    public void OnEnable()
    {        
        InitializeAttributes();
    }

    public int GetBaseValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.BaseValue;
            }
        }

        return -1;
    }

    public int GetModifiedValue(AttributeType type)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                return attribute.value.ModifiedValue;
            }
        }

        return -1;
    }

    public void SetBaseValue(AttributeType type, int value)
    {
        foreach (Attribute attribute in attributes)
        {
            if (attribute.type == type)
            {
                attribute.value.BaseValue = value;
            }
        }
    }

    public int AddHealth(int value)
    {
        Health += value;        

        OnChangedStats?.Invoke(this);

        return Health;
    }

    public int AddMana(int value)
    {
        Mana += value;

        OnChangedStats?.Invoke(this);
        return Mana;
    }

    [NonSerialized]
    private bool isInitialized = false;

    public void InitializeAttributes()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;
        Debug.Log("InitializeAttributes");

        foreach (Attribute attribute in attributes)
        {
            attribute.value = new ModifiableInt(OnModifiedValue);
        }

        level = 1;
        exp = 0;

        SetBaseValue(AttributeType.Agility, 100);
        SetBaseValue(AttributeType.Intellect, 100);
        SetBaseValue(AttributeType.Stamina, 100);
        SetBaseValue(AttributeType.Strength, 100);
        SetBaseValue(AttributeType.Health, 100);
        SetBaseValue(AttributeType.Mana, 100);

        Health = GetModifiedValue(AttributeType.Health);
        Mana = GetModifiedValue(AttributeType.Health);
    }

    private void OnModifiedValue(ModifiableInt value)
    {
        OnChangedStats?.Invoke(this);
    }
}

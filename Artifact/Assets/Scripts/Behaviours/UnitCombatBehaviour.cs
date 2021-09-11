using UnityEngine;
using System;

[Serializable]
public class UnitCombatBehaviour
{
    [SerializeField]
    private int attack;

    [SerializeField]
    private int health;

    [SerializeField]
    private int armor;

    public event Action<int> OnHealthChange;

    public int Attack { get => attack; }
    public int Health { get => health; }
    public int Armor { get => armor; }

    public UnitCombatBehaviour()
    {

    }

    public UnitCombatBehaviour(int attack, int health, int armor)
    {
        this.attack = attack;
        this.health = health;
        this.armor = armor;
    }

    public void TakeDamage(UnitCombatBehaviour attacker)
    {
        health -= attacker.attack - armor;
        health = Mathf.Max(0, health);
        OnHealthChange?.Invoke(health);
    }
}
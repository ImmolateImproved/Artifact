using Unity.Entities;
using UnityEngine.UI;

[GenerateAuthoringComponent]
public class UnitUi : IComponentData
{
    public Text attackText;
    public Text healthText;
    public Text armorText;

    public void Init(UnitCombat unitCombat)
    {
        unitCombat.combatBehaviour.OnHealthChange += CombatBehaviour_OnHealthChange;

        attackText.text = unitCombat.combatBehaviour.Attack.ToString();
        healthText.text = unitCombat.combatBehaviour.Health.ToString();
        armorText.text = unitCombat.combatBehaviour.Armor.ToString();
    }

    public void Reset(UnitCombat unitCombat)
    {
        unitCombat.combatBehaviour.OnHealthChange -= CombatBehaviour_OnHealthChange;
    }

    private void CombatBehaviour_OnHealthChange(int currentHealth)
    {
        healthText.text = currentHealth.ToString();
    }
}
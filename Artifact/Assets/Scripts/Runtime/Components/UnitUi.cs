using Unity.Entities;
using UnityEngine.UI;

[GenerateAuthoringComponent]
public class UnitUi : IComponentData
{
    public Text attackText;
    public Text healthText;
    public Text armorText;

    private void CombatBehaviour_OnHealthChange(int currentHealth)
    {
        healthText.text = currentHealth.ToString();
    }
}
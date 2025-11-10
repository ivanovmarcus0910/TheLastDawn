using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Collect Objective")]
public class CollectObjectiveDefinition : ObjectiveDefinition
{
    [Tooltip("ID của vật phẩm. PHẢI KHỚP với ItemData.name")]
    public string itemId;

    public override void OnEvent(IQuestEvent evt, ref ObjectiveRuntimeState runtime)
    {
        if (evt is ItemCollectedEvent e && e.itemId == itemId)
        {
            runtime.current = Mathf.Min(runtime.current + e.amount, targetAmount);
        }
    }
}
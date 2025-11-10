using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Interact Objective")]
public class InteractObjectiveDefinition : ObjectiveDefinition
{
    public string interactableId; // ID của đối tượng có thể tương tác

    public override void OnEvent(IQuestEvent evt, ref ObjectiveRuntimeState runtime)
    {
        if (evt is InteractedEvent e && e.interactableId == interactableId)
        {
            runtime.current = Mathf.Min(runtime.current + 1, targetAmount);
        }
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Reach Area Objective")]
public class ReachAreaObjectiveDefinition : ObjectiveDefinition
{
    [Tooltip("ID của khu vực. PHẢI KHỚP với ID trong QuestTriggerArea")]
    public string areaId;

    public override void OnEvent(IQuestEvent evt, ref ObjectiveRuntimeState runtime)
    {
        if (evt is AreaEnteredEvent e && e.areaId == areaId)
        {
            runtime.current = targetAmount;
        }
    }
}
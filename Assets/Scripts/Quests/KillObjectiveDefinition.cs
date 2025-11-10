using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Objectives/Kill Objective")]
public class KillObjectiveDefinition : ObjectiveDefinition
{
    [Tooltip("ID của kẻ thù. PHẢI KHỚP với EnemyData.name")]
    public string enemyId;

    public override void OnEvent(IQuestEvent evt, ref ObjectiveRuntimeState runtime)
    {
        if (evt is EnemyKilledEvent e && e.enemyId == enemyId)
        {
            runtime.current = Mathf.Min(runtime.current + 1, targetAmount);
        }
    }
}
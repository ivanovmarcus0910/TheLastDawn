using UnityEngine;

public abstract class ObjectiveDefinition : ScriptableObject
{
    [SerializeField] private string objectiveId = System.Guid.NewGuid().ToString("N");

    [TextArea] public string description; // Mô tả mục tiêu
    public int targetAmount = 1; // Số lượng cần đạt

    public string ObjectiveId => objectiveId;

    public abstract void OnEvent(IQuestEvent evt, ref ObjectiveRuntimeState runtime);
    public virtual bool IsCompleted(ObjectiveRuntimeState runtime) => runtime.current >= targetAmount;
    public virtual string GetProgressText(ObjectiveRuntimeState runtime)
        => $"{Mathf.Clamp(runtime.current, 0, targetAmount)}/{targetAmount}";
}
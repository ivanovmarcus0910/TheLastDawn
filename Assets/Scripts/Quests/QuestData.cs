using UnityEngine;
public enum ObjectiveType
{
    Kill,
    Collect
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/Create New Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int questID; 
    public string title; 

    [TextArea]
    public string description;

    [Header("Mục tiêu (Goal)")]
    public ObjectiveType objectiveType; 
    public int targetAmount; 

    public string targetID;

    [Header("Phần thưởng")]
    public int rewardExp;
    public ItemData rewardItem; 
    public int rewardItemAmount;

  
}
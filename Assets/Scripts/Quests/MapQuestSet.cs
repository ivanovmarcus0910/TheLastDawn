using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Map Quest Set")]
public class MapQuestSet : ScriptableObject
{
    [Tooltip("ID của map (phải khớp tên Scene)")]
    public string mapId;

    [Tooltip("Kéo các file QuestDefinition của map này vào đây")]
    public List<QuestDefinition> quests = new List<QuestDefinition>();
}
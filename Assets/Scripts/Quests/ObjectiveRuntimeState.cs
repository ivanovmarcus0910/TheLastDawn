using System;

[Serializable]
public struct ObjectiveRuntimeState
{
    public string objectiveId; // ID của ObjectiveDefinition tương ứng
    public int current; // Tiến độ hiện tại

    // Hàm khởi tạo tiện lợi
    public static ObjectiveRuntimeState New(string id)
        => new ObjectiveRuntimeState { objectiveId = id, current = 0 };
}
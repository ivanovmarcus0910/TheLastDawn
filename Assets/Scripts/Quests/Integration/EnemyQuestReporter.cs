using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyQuestReporter : MonoBehaviour
{
    private EnemyBase _enemy;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBase>();
    }

    private void OnEnable()
    {
        // Khi _enemy.onDeath được gọi, nó cũng sẽ gọi hàm ReportDeath
        if (_enemy != null) _enemy.onDeath += ReportDeath;
    }

    private void OnDisable()
    {
        // Hủy đăng ký
        if (_enemy != null) _enemy.onDeath -= ReportDeath;
    }

    private void ReportDeath()
    {
        // Lấy 'enemyId'. Ưu tiên lấy từ 'EnemyData.name'
        var id = _enemy != null && _enemy.data != null ? _enemy.data.name : gameObject.name;

        // Bắn sự kiện lên Trung tâm
        GameEventHub.EnemyKilled(id, transform.position, 1);
    }
}
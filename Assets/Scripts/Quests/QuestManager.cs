using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Configuration")]
    [Tooltip("Kéo các file MapQuestSet (ví dụ: MapQuestSet_Desert) vào đây")]
    public List<MapQuestSet> mapQuestSets = new List<MapQuestSet>();

    [Header("Debug")]
    [Tooltip("Bật cờ này để xem log sự kiện và save/load trong Console")]
    public bool logEvents = false;

    [Serializable]
    public class QuestRuntimeState
    {
        public string questId; // ID của QuestDefinition tương ứng
        public QuestStatus status; // Trạng thái hiện tại
        public float timeLeft; // Thời gian còn lại (nếu có)
        // Danh sách trạng thái của các objective con
        public List<ObjectiveRuntimeState> objectives = new List<ObjectiveRuntimeState>();
    }

    //------------------------------------------------------------------
    // BIẾN NỘI BỘ (Private)
    //------------------------------------------------------------------

    // Một Dictionary (giống như "sổ theo dõi") lưu trạng thái của TẤT CẢ các quest
    // mà người chơi đã từng gặp.
    // Key (khóa) là string 'questId', Value (giá trị) là 'QuestRuntimeState'.
    private readonly Dictionary<string, QuestRuntimeState> _questStates = new Dictionary<string, QuestRuntimeState>();

    // Tên của Scene (map) hiện tại
    private string _currentMapId = "";

    //------------------------------------------------------------------
    // CÁC HÀM CỦA UNITY (Awake, OnEnable, OnDisable, Start...)
    //------------------------------------------------------------------

    /// Awake được gọi trước Start. Dùng để thiết lập Singleton.
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            // Thì tự hủy 'this' đi để đảm bảo chỉ có 1 QuestManager
            Destroy(gameObject);
            return;
        }
        // Nếu không, gán 'Instance' là 'this'
        Instance = this;

        // Giữ cho GameObject này không bị hủy khi chuyển Scene
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Đăng ký: "Khi nào GameEventHub.OnEvent phát sự kiện, hãy gọi hàm OnQuestEvent"
        GameEventHub.OnEvent += OnQuestEvent;
        // Đăng ký: "Khi nào Scene được tải xong, hãy gọi hàm OnSceneLoaded"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Hủy đăng ký
        GameEventHub.OnEvent -= OnQuestEvent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Start được gọi 1 lần sau Awake. Dùng để tải dữ liệu (nếu có)
    /// và đăng ký các quest của map đầu tiên.
    /// </summary>
    private void Start()
    {
        _currentMapId = SceneManager.GetActiveScene().name;
        Load(); // Thử tải dữ liệu đã lưu
        RegisterMapQuests(); // Đăng ký các quest cho map hiện tại
    }

    //------------------------------------------------------------------
    // HÀM XỬ LÝ SỰ KIỆN (Event Handlers)
    //------------------------------------------------------------------

    /// <summary>
    /// Được gọi mỗi khi có một Scene mới được tải xong.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentMapId = scene.name; // Cập nhật tên map hiện tại
        RegisterMapQuests(); // Đăng ký các quest cho map MỚI này
        Save(); // Lưu trạng thái
        if (logEvents) Debug.Log($"[QuestManager] Scene loaded: {_currentMapId}");
    }

    private void OnQuestEvent(IQuestEvent evt)
    {
        if (logEvents) Debug.Log($"[QuestManager] Event Received: {evt.GetType().Name}");

        // 1. Duyệt qua TẤT CẢ các QuestDefinition trong phạm vi (map hiện tại)
        foreach (var def in EnumerateAllQuestsInScope())
        {
            // 2. Lấy trạng thái runtime của quest này (từ 'sổ theo dõi' _questStates)
            if (!_questStates.TryGetValue(def.QuestId, out var st)) continue; // Không tìm thấy, bỏ qua

            // 3. CHỈ xử lý các quest đang "Active" (đã nhận)
            if (st.status != QuestStatus.Active) continue;

            bool anyChange = false; // Cờ (flag) để theo dõi xem quest có thay đổi tiến độ không

            // 4. Duyệt qua TỪNG objective con của quest này
            for (int i = 0; i < def.objectives.Count; i++)
            {
                var objDef = def.objectives[i]; // Định nghĩa objective (ví dụ: Kill_SandWraith)
                var objSt = st.objectives[i]; // Trạng thái runtime (ví dụ: {current: 5})

                // 5. Đưa sự kiện 'evt' cho objective xử lý
                // Hàm 'OnEvent' của 'objDef' (ví dụ: KillObjectiveDefinition) sẽ được gọi.
                // Nó sẽ tự kiểm tra xem sự kiện này có liên quan đến nó không
                // và TỰ CẬP NHẬT 'objSt' (nhờ từ khóa 'ref')
                objDef.OnEvent(evt, ref objSt);

                // 6. Kiểm tra xem 'current' có thay đổi không
                if (objSt.current != st.objectives[i].current)
                {
                    anyChange = true;
                    st.objectives[i] = objSt; // Lưu lại trạng thái mới
                }
            }

            // 7. Nếu có bất kỳ thay đổi nào về tiến độ
            if (anyChange)
            {
                // 8. Kiểm tra xem toàn bộ quest đã hoàn thành chưa
                if (IsQuestCompleted(def, st))
                {
                    // Nếu rồi, đổi trạng thái sang "Completed"
                    st.status = QuestStatus.Completed;
                    _questStates[def.QuestId] = st; // Cập nhật vào 'sổ theo dõi'
                    Debug.Log($"[QuestManager] Quest Completed: {def.title}");

                    // Nếu quest này tự động trả (autoTurnIn)
                    if (def.autoTurnIn)
                    {
                        TurnIn(def.QuestId); // Gọi hàm trả quest ngay lập tức
                    }
                }
                else
                {
                    // Nếu chưa xong, chỉ cần cập nhật lại 'sổ theo dõi'
                    _questStates[def.QuestId] = st;
                }
            }
        }
    }

    //------------------------------------------------------------------
    // CÁC HÀM LOGIC CỐT LÕI (Public/Private Helpers)
    //------------------------------------------------------------------

    /// <summary>
    /// Đăng ký các quest của map hiện tại.
    /// </summary>
    private void RegisterMapQuests()
    {
        // 1. Lấy tất cả các QuestDefinition trong phạm vi
        foreach (var q in EnumerateAllQuestsInScope())
        {
            // 2. Nếu quest này chưa có trong 'sổ theo dõi' (_questStates)
            if (!_questStates.ContainsKey(q.QuestId))
            {
                // Tạo một trạng thái runtime MỚI cho nó (mặc định là 'Available')
                _questStates[q.QuestId] = NewStateFor(q);
            }

            // 3. Lấy trạng thái (vừa tạo hoặc có sẵn)
            var st = _questStates[q.QuestId];

            // 4. Nếu quest này 'autoAccept' và đang 'Available'
            if (q.autoAcceptOnMapEnter && st.status == QuestStatus.Available)
            {
                // Nhận quest này ngay lập tức
                AcceptQuest(q.QuestId);
            }
        }
    }

    public bool AcceptQuest(string questId)
    {
        var def = FindQuestDef(questId); // Tìm định nghĩa của quest
        if (def == null)
        {
            Debug.LogWarning($"[QuestManager] Không tìm thấy QuestDefinition cho ID: {questId}");
            return false;
        }

        if (!_questStates.TryGetValue(questId, out var st))
        {
            st = NewStateFor(def);
            _questStates[questId] = st;
        }

        // Chỉ chấp nhận khi quest đang 'Available'
        // (Hoặc 'TurnedIn' nếu quest này 'repeatable')
        if (st.status != QuestStatus.Available && !(def.repeatable && st.status == QuestStatus.TurnedIn))
        {
            return false;
        }

        // Đặt lại trạng thái quest
        st.status = QuestStatus.Active;
        // Reset tất cả tiến độ objective về 0
        st.objectives = def.objectives.Select(o => ObjectiveRuntimeState.New(o.ObjectiveId)).ToList();
        st.timeLeft = def.timeLimitSeconds;
        _questStates[questId] = st; // Cập nhật 'sổ theo dõi'

        if (logEvents) Debug.Log($"[QuestManager] Accepted quest: {def.title}");
        Save(); // Lưu lại
        return true;
    }

    /// <summary>
    /// Được gọi bởi UI (hoặc NPC, hoặc autoTurnIn) để TRẢ QUEST (Turn-in).
    /// </summary>
    public bool TurnIn(string questId)
    {
        if (!_questStates.TryGetValue(questId, out var st)) return false;

        // Chỉ trả được khi quest đang ở trạng thái "Completed"
        if (st.status != QuestStatus.Completed) return false;

        // Đổi trạng thái sang "TurnedIn" (đã trả)
        st.status = QuestStatus.TurnedIn;
        _questStates[questId] = st;

        var def = FindQuestDef(questId);
        if (def != null)
        {
            // Phát sự kiện "RewardGrantedEvent"
            // Các hệ thống khác (Inventory, PlayerData) sẽ lắng nghe sự kiện này
            // để thực sự trao thưởng cho người chơi.
            if (def.rewardExperience != 0 || (def.rewardItems != null && def.rewardItems.Length > 0))
            {
                GameEventHub.RewardGranted(def.QuestId, def.rewardExperience, def.rewardItems);
            }

            // (Nâng cao) Kích hoạt quest tiếp theo nếu có
            if (def.nextQuestOnComplete != null)
            {
                var next = def.nextQuestOnComplete;
                // Đưa quest tiếp theo vào trạng thái 'Available'
                if (!_questStates.ContainsKey(next.QuestId))
                    _questStates[next.QuestId] = NewStateFor(next);
                // Tự động nhận quest tiếp theo nếu nó 'autoAccept'
                if (next.autoAcceptOnMapEnter)
                    AcceptQuest(next.QuestId);
            }
        }

        if (logEvents) Debug.Log($"[QuestManager] Turned in quest: {questId}");
        Save(); // Lưu lại
        return true;
    }

    //------------------------------------------------------------------
    // CÁC HÀM TIỆN ÍCH (Helpers)
    //------------------------------------------------------------------

    /// <summary>
    /// Hàm tạo trạng thái runtime mới cho một QuestDefinition.
    /// </summary>
    private QuestRuntimeState NewStateFor(QuestDefinition def)
    {
        return new QuestRuntimeState
        {
            questId = def.QuestId,
            status = QuestStatus.Available, // Mặc định là 'Available'
            timeLeft = def.timeLimitSeconds,
            // Khởi tạo trạng thái cho tất cả objective con (với current = 0)
            objectives = def.objectives.Select(o => ObjectiveRuntimeState.New(o.ObjectiveId)).ToList()
        };
    }

    /// <summary>
    /// Kiểm tra xem một quest (với trạng thái 'st') đã hoàn thành tất cả objective chưa.
    /// </summary>
    private bool IsQuestCompleted(QuestDefinition def, QuestRuntimeState st)
    {
        // Duyệt qua tất cả objective
        for (int i = 0; i < def.objectives.Count; i++)
        {
            // Chỉ cần 1 objective CHƯA hoàn thành...
            if (!def.objectives[i].IsCompleted(st.objectives[i]))
            {
                return false; // ...thì quest chưa hoàn thành
            }
        }
        return true; // Nếu tất cả đều xong, quest hoàn thành
    }

    /// <summary>
    /// Lấy TẤT CẢ các QuestDefinition đang có trong phạm vi (map hiện tại).
    /// </summary>
    private IEnumerable<QuestDefinition> EnumerateAllQuestsInScope()
    {
        foreach (var set in mapQuestSets)
        {
            foreach (var q in set.quests)
            {
                // Quest Global, hoặc quest thuộc map hiện tại
                if (q.scope == QuestScope.Global || string.IsNullOrEmpty(q.mapId) || q.mapId == _currentMapId)
                    yield return q;
            }
        }
    }

    /// <summary>
    /// Tìm một QuestDefinition dựa trên ID.
    /// </summary>
    private QuestDefinition FindQuestDef(string questId)
    {
        return EnumerateAllQuestsInScope().FirstOrDefault(q => q.QuestId == questId);
    }

    /// <summary>
    /// Hàm này DÀNH CHO UI. Lấy danh sách (QuestDefinition, QuestRuntimeState)
    /// của các quest đang hiển thị trong map hiện tại.
    /// </summary>
    public IEnumerable<(QuestDefinition def, QuestRuntimeState st)> GetVisibleQuests()
    {
        foreach (var def in EnumerateAllQuestsInScope())
        {
            if (_questStates.TryGetValue(def.QuestId, out var st))
            {
                // 'yield return' tạo ra một danh sách "lười" (lazy list)
                yield return (def, st);
            }
        }
    }

    //------------------------------------------------------------------
    // LOGIC LƯU/TẢI DỮ LIỆU (Save/Load)
    // Dùng PlayerPrefs và JSON - đơn giản nhưng không bảo mật.
    //------------------------------------------------------------------

    [Serializable]
    private class QuestSaveData
    {
        public string activeMapId;
        public List<QuestRuntimeState> quests = new List<QuestRuntimeState>();
    }

    public void Save()
    {
        try
        {
            // Tạo đối tượng SaveData và gán dữ liệu
            var data = new QuestSaveData
            {
                activeMapId = _currentMapId,
                quests = _questStates.Values.ToList() // Chuyển Dictionary sang List để lưu
            };
            // Chuyển đối tượng thành chuỗi JSON
            var json = JsonUtility.ToJson(data);
            // Lưu chuỗi JSON vào PlayerPrefs
            PlayerPrefs.SetString("QUESTS_SAVE_V1", json);
            PlayerPrefs.Save(); // (Tùy chọn) Lưu ngay lập tức
            if (logEvents) Debug.Log("[QuestManager] Saved quests.");
        }
        catch (Exception e)
        {
            Debug.LogWarning("[QuestManager] Save failed: " + e.Message);
        }
    }

    public void Load()
    {
        try
        {
            if (!PlayerPrefs.HasKey("QUESTS_SAVE_V1")) return; // Không có file save

            // Lấy chuỗi JSON từ PlayerPrefs
            var json = PlayerPrefs.GetString("QUESTS_SAVE_V1");
            // Chuyển JSON ngược lại thành đối tượng
            var data = JsonUtility.FromJson<QuestSaveData>(json);

            _currentMapId = data.activeMapId;
            _questStates.Clear(); // Xóa 'sổ theo dõi' cũ
            // Đổ dữ liệu từ List (trong file save) vào Dictionary
            foreach (var q in data.quests)
            {
                _questStates[q.questId] = q;
            }

            if (logEvents) Debug.Log("[QuestManager] Loaded quests.");
        }
        catch (Exception e)
        {
            Debug.LogWarning("[QuestManager] Load failed: " + e.Message);
        }
    }

    // Tự động lưu khi thoát game hoặc pause
    private void OnApplicationQuit() => Save();
    private void OnApplicationPause(bool pause) { if (pause) Save(); }
}
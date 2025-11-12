using Assets.Scripts.DTO;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class LoadDataManager : MonoBehaviour
{
    public static LoadDataManager Instance { get; private set; }
    public static FirebaseUser firebaseUser;
    public static User userInGame;

    public PlayerBase playerScript;
    public RecylableInventoryManager inventoryManager;
    public MapManager mapManager;

    private FirebaseDBManager firebaseDBManager;
   // public ItemData defaultItemData;

    private void Awake()
    {
        // ▼▼▼ THÊM KHỐI CODE NÀY (Logic Singleton) ▼▼▼
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Bỏ comment dòng này nếu bạn muốn nó tồn tại qua các Scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        

        firebaseDBManager = FirebaseDBManager.Instance ?? FindObjectOfType<FirebaseDBManager>();
        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    private IEnumerator Start()
    {
        // 🕐 Đợi FirebaseDBManager khởi tạo hoàn tất
        while (FirebaseDBManager.Instance == null)
        {
            Debug.Log("⏳ Đợi FirebaseDBManager...");
            yield return null;
        }

        firebaseDBManager = FirebaseDBManager.Instance;

        // 🕐 Đợi Firebase Database sẵn sàng
        yield return new WaitUntil(() => FirebaseDatabase.DefaultInstance != null);

        if (firebaseUser == null)
        {
            Debug.LogError("❌ Firebase user null! Người dùng chưa đăng nhập.");
            yield break;
        }

        Debug.Log("✅ Firebase sẵn sàng, bắt đầu load dữ liệu người chơi...");
        GetUserInGame();
    }

    public void GetUserInGame()
    {
        Debug.Log($"🧩 firebaseDBManager = {(firebaseDBManager == null ? "❌ null" : "✅ ok")}");
        Debug.Log($"🧩 firebaseUser = {(firebaseUser == null ? "❌ null" : firebaseUser.UserId)}");

        if (firebaseDBManager == null || firebaseUser == null)
        {
            Debug.LogError("⚠️ Firebase chưa sẵn sàng để load data!");
            return;
        }

        firebaseDBManager.ReadDB(firebaseUser.UserId, json =>
        {
            Debug.Log($"📦 json từ DB = {(string.IsNullOrEmpty(json) ? "❌ null/empty" : "✅ ok")}");

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("⚠️ Không có dữ liệu user trên Firebase (có thể user mới).");
                return;
            }

            try
            {
                // ⚙️ Parse JSON về object User
                userInGame = JsonConvert.DeserializeObject<User>(json,
                    new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

                if (userInGame == null)
                {
                    Debug.LogError("❌ userInGame null sau khi deserialize");
                    return;
                }

                Debug.Log($"✅ User loaded: {userInGame.Name}");
                Debug.Log($"🎮 PlayerData null? {(userInGame.playerData == null ? "YES" : "NO")}");
              
                if (userInGame.playerData == null)
                {
                    Debug.LogWarning("⚠️ playerData trống — dùng dữ liệu mặc định.");
                    userInGame.playerData = PlayerDataDTO.FromPlayerData(ScriptableObject.CreateInstance<PlayerData>());
                }
                // 🔁 Convert DTO → PlayerData (ScriptableObject)

                PlayerData player = userInGame.playerData.ToPlayerData();

                

                // 🔧 Cập nhật Player trong game
                if (playerScript != null)
                {
                    playerScript.UpdatePlayerData(player);
                    print($"✅ PlayerData đã được cập nhật: {playerScript.GetPlayerData().ToString()}");
                }
                else
                    Debug.LogWarning("⚠️ playerScript chưa được gán trong Inspector!");
                if (inventoryManager != null)
                {
                    List<ItemData> itemDataList = new List<ItemData>();
                    if (userInGame.itemDataList != null)
                        foreach (ItemDataDTO x in userInGame.itemDataList)
                        {
                            itemDataList.Add(x.ToItemData());
                        }
                    inventoryManager.UpdateDataInventory(itemDataList, userInGame.itemQuantityList);
                    Debug.Log("✅ Inventory đã được tải.");
                }
                else
                    Debug.LogWarning("��️ inventoryManager chưa được gán trong Inspector!");
                if (mapManager != null)
                {
                    mapManager.ChangeCurrentMap(userInGame.currentMapIndex);
                    Debug.Log("✅ Di chuyển đến map đã lưu.");
                }
                else
                    Debug.LogWarning("⚠️ mapManager chưa được gán trong Inspector!");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Lỗi khi parse user: " + e.Message);
            }
        });
    }

    private void OnApplicationQuit()
    {
        SaveUserDataOnQuit();
    }

   



    private void SaveUserDataOnQuit()

    {

        if (firebaseUser == null)

        {

            Debug.LogWarning("⚠️ Không có user để lưu dữ liệu.");

            return;

        }



        if (firebaseDBManager == null)

        {

            Debug.LogWarning("⚠️ FirebaseDBManager chưa sẵn sàng, không thể lưu.");

            return;

        }



        try

        {

            // 🧠 Lấy dữ liệu mới nhất từ Player

            PlayerData playerData = playerScript.GetPlayerData();

            PlayerDataDTO playerDTO = PlayerDataDTO.FromPlayerData(playerData);



            // Cập nhật vào user hiện tại

            List<ItemDataDTO> itemDataDTOList = new List<ItemDataDTO>();

            if (inventoryManager.GetItemDataList() != null)

                foreach (ItemData itemData in inventoryManager.GetItemDataList())

                {

                    itemDataDTOList.Add(ItemDataDTO.FromItemData(itemData));

                }

            userInGame.playerData = playerDTO;

            userInGame.itemDataList = itemDataDTOList;

            userInGame.itemQuantityList = inventoryManager.GetItemQuantityList();

            print("Current Index" + mapManager.currentIndex);

            userInGame.currentMapIndex = mapManager.currentIndex;

            print("Data Player khi lưu " + playerData.ToString());

            // 🔥 Ghi lại lên Firebase

            string json = JsonConvert.SerializeObject(userInGame);

            firebaseDBManager.WriteDB(firebaseUser.UserId, json);



            Debug.Log("✅ Dữ liệu user đã được lưu lên Firebase!");

        }

        catch (Exception e)

        {

            Debug.LogError("❌ Lỗi khi lưu dữ liệu user: " + e.Message);

        }

    }
}

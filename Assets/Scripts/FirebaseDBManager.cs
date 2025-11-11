using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;

public class FirebaseDBManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    public static FirebaseDBManager Instance; // ✅ Singleton option

    private void Awake()
    {
        // ✅ Singleton setup (chỉ 1 FirebaseDBManager tồn tại xuyên suốt)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ✅ Đảm bảo Firebase đã init xong trước khi tạo dbReference
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("✅ Firebase initialized & Database ready!");
            }
            else
            {
                Debug.LogError("❌ Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // 🔹 Ghi dữ liệu (object JSON thật, không bị escape)
    public void WriteDB(string id, string json)
    {
        if (dbReference == null)
        {
            Debug.LogError("❌ dbReference is null! Firebase not ready yet.");
            return;
        }

        dbReference.Child("users").Child(id).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                    Debug.LogError($"❌ Failed to write data for {id}: {task.Exception}");
                else if (task.IsCompleted)
                    Debug.Log($"✅ Data written successfully for {id}");
            });
    }

    // 🔹 Đọc dữ liệu (gọi callback khi xong)
    public void ReadDB(string id, Action<string> onDataLoaded)
    {
        if (dbReference == null)
        {
            Debug.LogError("❌ dbReference is null! Firebase not ready yet.");
            onDataLoaded?.Invoke(null);
            return;
        }

        dbReference.Child("users").Child(id)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"❌ Failed to read data for {id}: {task.Exception}");
                    onDataLoaded?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    string json = snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Debug.Log($"📦 Read data for {id}: {json}");
                        onDataLoaded?.Invoke(json);
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ No data found for {id}");
                        onDataLoaded?.Invoke(null);
                    }
                }
            });
    }
}

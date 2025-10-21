using UnityEngine;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;

public class FirebaseDBManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void Start()
    {
        ReadDB("1");
    }
    public void WriteDB(string id, string message)
    {
        dbReference.Child("users").Child(id).SetValueAsync(message).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                //Debug.Log("Data written successfully.");
            }
            else
            {
                //Debug.LogError("Failed to write data: " + task.Exception);
            }
        }
            );
    }

    public void ReadDB(string id)
    {
        dbReference.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                //Debug.Log("Data read successfully: " + snapshot.Value.ToString());
            }
            else
            {
                //Debug.LogError("Failed to read data: " + task.Exception);
            }
        }
            );
    }
}

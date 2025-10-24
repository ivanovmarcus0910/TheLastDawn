using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class LoadDataManager : MonoBehaviour
{
    public static FirebaseUser firebaseUser;
    public static User userInGame;
    private DatabaseReference dbReference;

    private void Awake()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
       
    }

    public void GetUserInGame()
    {
        dbReference.Child("users").Child(firebaseUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                print("Đã lấy dữ liệu người chơi từ Firebase");
                DataSnapshot snapshot = task.Result;
                userInGame = JsonConvert.DeserializeObject<User>(snapshot.Value.ToString());
            }
            else
            {
                Debug.LogError("Failed to read data: " + task.Exception);
            }
        }
            );
    }    
}

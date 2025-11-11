using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using Assets.Scripts.DTO;
using Newtonsoft.Json;

public class LoginManager : MonoBehaviour
{
    [Header("Register")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;
    public Button buttonRegister;

    [Header("Login")]
    public InputField ipLoginEmail;
    public InputField ipLoginPassword;
    public Button buttonLogin;

    [Header("SwitchForm")]
    public Button buttonMoveToSignIn;
    public Button buttonMoveToRegister;

    public GameObject loginForm;
    public GameObject registerForm;

    private FirebaseAuth auth;
    public FirebaseDBManager firebaseDBManager;
    //item mặc định để tránh null
    public ItemData itemData;
    private IEnumerator Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        while (FirebaseDBManager.Instance == null)
        {
            Debug.Log("⏳ Đợi FirebaseDBManager...");
            yield return null;
        }
        firebaseDBManager = FirebaseDBManager.Instance;

        Debug.Log("✅ Firebase Auth & DB ready!");

        // Gán sự kiện nút
        buttonRegister.onClick.AddListener(RegisterAccount);
        buttonLogin.onClick.AddListener(LoginAccount);
        buttonMoveToRegister.onClick.AddListener(SwitchForm);
        buttonMoveToSignIn.onClick.AddListener(SwitchForm);
    }

    public void RegisterAccount()
    {
        Debug.Log("RegisterAccount called");
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("RegisterAccount was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("RegisterAccount encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            try
            {
                Debug.Log($"✅ New Firebase user: {newUser?.UserId}");

                // Tạo dữ liệu mặc định
                PlayerData defaultData = ScriptableObject.CreateInstance<PlayerData>();
                PlayerDataDTO defaultDataDTO = PlayerDataDTO.FromPlayerData(defaultData);
                List<ItemDataDTO> itemDatas = new List<ItemDataDTO>();
                itemDatas.Add(ItemDataDTO.FromItemData(itemData));
                List<int> itemQuantiy = new List<int>() {1}; 
                User userInGame = new User(email, itemDatas, itemQuantiy, defaultDataDTO, 0);

                // 🔹 Serialize đúng JSON format
                string json = JsonConvert.SerializeObject(userInGame);
                firebaseDBManager.WriteDB(newUser.UserId, json);

                Debug.Log("✅ User data saved to Firebase!");
                SceneManager.LoadScene("StartMenu");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Error creating user data: " + e.Message);
            }
        });
    }

    public void LoginAccount()
    {
        Debug.Log("LoginAccount called");
        string email = ipLoginEmail.text;
        string password = ipLoginPassword.text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("LoginAccount was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("LoginAccount encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log($"✅ Login success: {user.Email} ({user.UserId})");

            SceneManager.LoadScene("StartMenu");
        });
    }

    public void SwitchForm()
    {
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);
    }
}

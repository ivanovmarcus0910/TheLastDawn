using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public class LoginManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    private FirebaseDBManager firebaseDBManager;

    private void Start()
    {

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("Firebase Auth Initialized");

        buttonRegister.onClick.AddListener(RegisterAccount);
        buttonLogin.onClick.AddListener(LoginAccount);
        Debug.Log("Button Setting");

        buttonMoveToRegister.onClick.AddListener(SwitchForm);
        buttonMoveToSignIn.onClick.AddListener(SwitchForm);
        Debug.Log("Switch Button");

        firebaseDBManager = GetComponent<FirebaseDBManager>();
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
            // Firebase user has been created.
            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
               newUser.DisplayName, newUser.UserId);
            User userInGame = new User("", new List<ItemData>(), new List<int>(), ScriptableObject.CreateInstance<PlayerData>(), 0);
            firebaseDBManager.WriteDB("Users/"+newUser.UserId, userInGame.ToString());
            SceneManager.LoadScene("StartMenu");
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
            // Firebase user has been created.
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            SceneManager.LoadScene("StartMenu");
        });
    }
    public void SwitchForm()
    {
        Debug.Log("SwitchForm called");
        loginForm.SetActive(!loginForm.activeSelf);
        registerForm.SetActive(!registerForm.activeSelf);
    }
}

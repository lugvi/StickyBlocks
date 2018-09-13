
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class FirebaseNotificationManager : MonoBehaviour {

  Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
  private string topic = "MainTopic";

  // Log the result of the specified task, returning true if the task
  // completed successfully, false otherwise.
  protected bool LogTaskCompletion(Task task, string operation) {
    bool complete = false;
    if (task.IsCanceled) {
      Debug.LogWarning(operation + " canceled.");
    } else if (task.IsFaulted) {
      Debug.LogWarning(operation + " encounted an error.");
      foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
        string errorCode = "";
        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        if (firebaseEx != null) {
          errorCode = String.Format("Error.{0}: ",
            ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
        }
        Debug.LogWarning(errorCode + exception.ToString());
      }
    } else if (task.IsCompleted) {
      Debug.LogWarning(operation + " completed");
      complete = true;
    }
    return complete;
  }


  // When the app starts, check to make sure that we have
  // the required dependencies to use Firebase, and if not,
  // add them if possible.
  protected virtual void Start() {
    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
      dependencyStatus = task.Result;
      if (dependencyStatus == Firebase.DependencyStatus.Available) {
        InitializeFirebase();
      } else {
        Debug.LogError(
          "Could not resolve all Firebase dependencies: " + dependencyStatus);
      }
    });
  }

  // Setup message event handlers.
  void InitializeFirebase() {
    // Prevent the app from requesting permission to show notifications
    // immediately upon being initialized. Since it the prompt is being
    // suppressed, we must manually display it with a call to
    // RequestPermission() elsewhere.
    Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = false;
    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
    Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWith(task => {
      LogTaskCompletion(task, "SubscribeAsync");
    });
    Debug.LogWarning("Firebase Messaging Initialized");

    // This will display the prompt to request permission to receive
    // notifications if the prompt has not already been displayed before. (If
    // the user already responded to the prompt, thier decision is cached by
    // the OS and can be changed in the OS settings).
    Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWith(task => {
      LogTaskCompletion(task, "RequestPermissionAsync");
    });
  }

  public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) {
    Debug.LogWarning("Received a new message");
    var notification = e.Message.Notification;
    if (notification != null) {
        Debug.LogWarning("title: " + notification.Title);
        Debug.LogWarning("body: " + notification.Body);
    }
    if (e.Message.From.Length > 0)
      Debug.LogWarning("from: " + e.Message.From);
    if (e.Message.Link != null) {
        Debug.LogWarning("link: " + e.Message.Link.ToString());
    }
    if (e.Message.Data.Count > 0) {
      Debug.LogWarning("data:");
      foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
               e.Message.Data) {
        Debug.LogWarning("  " + iter.Key + ": " + iter.Value);
      }
    }
  }

  public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
    Debug.LogWarning("Received Registration Token: " + token.Token);
  }

  // Exit if escape (or back, on mobile) is pressed.
  protected virtual void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Application.Quit();
    }
  }

  // End our messaging session when the program exits.
  public void OnDestroy() {
    Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
    Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
  }
}
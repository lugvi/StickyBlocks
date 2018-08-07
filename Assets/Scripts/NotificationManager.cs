using UnityEngine;
using System.Collections.Generic;

using OneSignalPush.MiniJSON;
using System;

public class NotificationManager : MonoBehaviour {

   private static string extraMessage;

   void Start () {
      extraMessage = null;

      // Enable line below to debug issues with setuping OneSignal. (logLevel, visualLogLevel)
      OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);

      // The only required method you need to call to setup OneSignal to receive push notifications.
      // Call before using any other methods on OneSignal.
      // Should only be called once when your app is loaded.
      // OneSignal.Init(OneSignal_AppId);
      OneSignal.StartInit("0b4c0882-499f-475b-994d-7eab090a3243")
               .HandleNotificationReceived(HandleNotificationReceived)
               .HandleNotificationOpened(HandleNotificationOpened)
               //.InFocusDisplaying(OneSignal.OSInFocusDisplayOption.Notification)
               .EndInit();
      
      OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
      OneSignal.permissionObserver += OneSignal_permissionObserver;
      OneSignal.subscriptionObserver += OneSignal_subscriptionObserver;

      var pushState = OneSignal.GetPermissionSubscriptionState();
      Debug.Log("pushState.subscriptionStatus.subscribed : " + pushState.subscriptionStatus.subscribed);
      Debug.Log("pushState.subscriptionStatus.userId : " + pushState.subscriptionStatus.userId);
   }

   private void OneSignal_subscriptionObserver(OSSubscriptionStateChanges stateChanges) {
      Debug.Log("stateChanges: " + stateChanges);
      Debug.Log("stateChanges.to.userId: " + stateChanges.to.userId);
      Debug.Log("stateChanges.to.subscribed: " + stateChanges.to.subscribed);
   }

   private void OneSignal_permissionObserver(OSPermissionStateChanges stateChanges) {
	  Debug.Log("stateChanges.from.status: " + stateChanges.from.status);
      Debug.Log("stateChanges.to.status: " + stateChanges.to.status);
   }

   // Called when your app is in focus and a notificaiton is recieved.
   // The name of the method can be anything as long as the signature matches.
   // Method must be static or this object should be marked as DontDestroyOnLoad
   private static void HandleNotificationReceived(OSNotification notification) {
      OSNotificationPayload payload = notification.payload;
      string message = payload.body;

      print("GameControllerExample:HandleNotificationReceived: " + message);
      print("displayType: " + notification.displayType);
      extraMessage = "Notification received with text: " + message;

   Dictionary<string, object> additionalData = payload.additionalData;
   if (additionalData == null) 
      Debug.Log ("[HandleNotificationReceived] Additional Data == null");
   else
      Debug.Log("[HandleNotificationReceived] message "+ message +", additionalData: "+ Json.Serialize(additionalData) as string);
   }
   
   // Called when a notification is opened.
   // The name of the method can be anything as long as the signature matches.
   // Method must be static or this object should be marked as DontDestroyOnLoad
   public static void HandleNotificationOpened(OSNotificationOpenedResult result) {
      OSNotificationPayload payload = result.notification.payload;
      string message = payload.body;
      string actionID = result.action.actionID;

      print("GameControllerExample:HandleNotificationOpened: " + message);
      extraMessage = "Notification opened with text: " + message;
      
      Dictionary<string, object> additionalData = payload.additionalData;
      if (additionalData == null) 
         Debug.Log ("[HandleNotificationOpened] Additional Data == null");
      else
         Debug.Log("[HandleNotificationOpened] message "+ message +", additionalData: "+ Json.Serialize(additionalData) as string);

      if (actionID != null) {
            // actionSelected equals the id on the button the user pressed.
            // actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
            extraMessage = "Pressed ButtonId: " + actionID;
         }
   }
}
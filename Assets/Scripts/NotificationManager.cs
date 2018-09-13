using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

public class NotificationManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void ScheduleNotificationForiOSWithMessage (string text)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification ();
			// notification.fireDate = System.DateTime.;
			notification.alertAction = "Alert";
			notification.alertBody = text;
			notification.hasAction = false;
			notification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.GregorianCalendar;
			notification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
			NotificationServices.ScheduleLocalNotification (notification);

			NotificationServices.RegisterForNotifications(
				NotificationType.Alert |
				NotificationType.Badge |
				NotificationType.Sound);
		}        
	}
}


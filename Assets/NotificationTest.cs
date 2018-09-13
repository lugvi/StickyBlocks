using UnityEngine;
using System.Collections;

public class NotificationTest : MonoBehaviour
{
    void Awake()
    {
        LocalNotification.ClearNotifications();
        Repeating();
    }

    public void OneTime()
    {
        LocalNotification.SendNotification(1, 5000, "Title", "Long message text", new Color32(0xff, 0x44, 0x44, 255));
    }

    public void OneTimeBigIcon()
    {
        LocalNotification.SendNotification(1, 5000, "Title", "Long message text with big icon", new Color32(0xff, 0x44, 0x44, 255), true, true, true, "app_icon");
    }

    public void OneTimeWithActions()
    {
        LocalNotification.Action action1 = new LocalNotification.Action("background", "In Background", this);
        action1.Foreground = false;
        LocalNotification.Action action2 = new LocalNotification.Action("foreground", "In Foreground", this);
        LocalNotification.SendNotification(1, 5000, "Title", "Long message text with actions", new Color32(0xff, 0x44, 0x44, 255), true, true, true, null, "boing", "default", action1, action2);
    }

    public void Repeating()
    {
        LocalNotification.Action action1 = new LocalNotification.Action("background", "In Background", this);
        action1.Foreground = false;
        LocalNotification.Action action2 = new LocalNotification.Action("foreground", "In Foreground", this);
        LocalNotification.SendRepeatingNotification(1, 60000 * 60 * 24, 60000 * 60 * 24, "Title", "Long message text", new Color32(0xff, 0x44, 0x44, 255),false,true, true,null,null,"default", action1, action2);
    }

    public void Stop()
    {
        LocalNotification.CancelNotification(1);
    }

    public void OnAction(string identifier)
    {
        Debug.LogWarning("Got action " + identifier);
    }
}

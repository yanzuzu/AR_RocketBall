
using UnityEngine;
using System;
using System.IO;

public class GameUtility
{
      
	public static string GetDeviceUniqueIdentifier()
    {
		
		string deviceId = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
        AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        string ANDROID_ID = settingsSecure.GetStatic<string>("ANDROID_ID");
        AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
        deviceId = settingsSecure.CallStatic<string>("getString", contentResolver, ANDROID_ID);      
#else
		deviceId = SystemInfo.deviceUniqueIdentifier;
#endif

		if (deviceId == "")
        {
            string mac = "00000000000000000000000000000000";
            try
            {
                StreamReader reader = new StreamReader("/sys/class/net/wlan0/address");
                mac = reader.ReadLine();
                reader.Close();
            }
            catch (Exception e) 
			{
				Debug.LogError("Error : " + e);
			}
			deviceId = mac.Replace(":", "");
        }

        return deviceId;
    }

	public static string[] Flags = new string[] { 
		"flag_argentina",
		"flag_australia",
		"flag_belgium",
		"flag_brazil",
		"flag_colombia",
		"flag_costa_rica",
		"flag_croatia",
		"flag_denmark",
		"flag_egypt",
		"flag_england",
		"flag_france",
		"flag_germany",
		"flag_iceland",
		"flag_iran",
		"flag_japan",
		"flag_mexico",
		"flag_morocco",
		"flag_nigeria",
		"flag_panama",
		"flag_nigeria",
		"flag_peru",
		"flag_poland",
		"flag_portugal",
		"flag_russia",
		"flag_saudi_arabia",
		"flag_senegal",
		"flag_serbia",
		"flag_south_korea",
		"flag_spain",
		"flag_sweden",
		"flag_switzerland",
		"flag_tunisia",
		"flag_uruguay"
	};

	public static string[] CarNames = new string[]
	{
        "A",
		"B",
		"C",
		"D",
		"E",
		"F",
		"G",
		"H",
		"I",
		"J",
        "K"
	};

      
	public static long DateTimeToUnixTime(DateTime datetime)
    {
        DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);      
        return (long)(datetime - sTime).TotalSeconds;
    }

	public static DateTime UnixTimeToDateTime(long unixtime)
    {
        DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return sTime.AddSeconds(unixtime);
    }

	public static bool CheckTimestampOverDay(long timestamp)
    {
        if (timestamp == 0)
            return true;
           
		DateTime startDay = UnixTimeToDateTime(timestamp);
		return (DateTime.Now.Day - startDay.Day) != 0;
    }
}

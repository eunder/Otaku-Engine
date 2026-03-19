using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibVLCSharp;
using System;

public class VLC_LibVLC_Singleton : MonoBehaviour
{
    private static VLC_LibVLC_Singleton _instance;
    public static VLC_LibVLC_Singleton Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

		//Setup LibVLC
		if (libVLC == null)
			CreateLibVLC();

    }


	public LibVLC libVLC;
	public bool logToConsole = false; //Log function calls and LibVLC logs to Unity console



	//Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
	void CreateLibVLC()
	{
		Debug.Log("VLCPlayerExample CreateLibVLC");
		//Dispose of the old libVLC if necessary
		if (libVLC != null)
		{
			libVLC.Dispose();
			libVLC = null;
		}

		Core.Initialize(Application.dataPath); //Load VLC dlls
		libVLC = new LibVLC(new string[] { "--sout-transcode-threads=2", "--input-repeat=200",  "--extraintf=http:logger", "--verbose=2", "--file-logging", "--logfile=" + Application.persistentDataPath + "/vlc-log.txt"}); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/
		
		//Setup Error Logging
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		libVLC.Log += (s, e) =>
		{
			//Always use try/catch in LibVLC events.
			//LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
			try
			{
				if (logToConsole)
				{
					Debug.Log(e.FormattedLog);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Exception caught in libVLC.Log: \n" + ex.ToString());
			}

		};
	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SharinganOP
{
    public class DebugUI : MonoBehaviour
    {
        public static DebugUI instance;

        public static string title = "Debug UI";
        static List<LogData> logs = new List<LogData>();
        struct LogData
        {
            public string message;
            public float? time;
            public float startTime;
            public Color? color;

            public override string ToString()
            {
                return message;
            }
        }

        void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"Multiple instances of {this.name} were loaded!");
                Destroy(this);
                return;
            }

            instance = this;
            //StartCoroutine(EndOfFrame());
        }

        Coroutine endOfFrame = null;
        void OnEnable()
        {
            endOfFrame = StartCoroutine(EndOfFrame());
        }

        void OnDisable()
        {
            StopCoroutine(endOfFrame);
        }

        void OnDestroy()
        {
            //Destroy(gameObject);
        }

        IEnumerator EndOfFrame()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                for (int i = logs.Count - 1; i >= 0; i--)
                {
                    bool shouldRemoveLog = !logs[i].time.HasValue;
                    if (logs[i].time.HasValue)
                    {
                        if (logs[i].time.Value <= (Time.time - logs[i].startTime))
                        {
                            shouldRemoveLog = true;
                        }
                    }
                    shouldRemoveLog = true;
                    if (shouldRemoveLog)
                    {
                        logs.RemoveAt(i);
                    }
                }
            }
        }

        void OnGUI()
        {
            int width = 500;
            int height = 20;
            int x = 10;
            int y = 10;

            var originalColor = GUI.color;

            //string titleLog =  $"{title} x{logs.Count}";
            GUI.Label(new Rect(x, y, width, height), title);

            for (int i = 0; i < logs.Count; i++)
            {
                int instY = y + ((i + 1) * height);
                //string log = $"[{i}] = {logs[i]}";

                if (logs[i].color.HasValue)
                {
                    GUI.color = logs[i].color.Value;
                }

                //GUI.Label(new Rect(x, instY, width, height), log);
                GUI.Label(new Rect(x, instY, width, height), logs[i].message);
                GUI.color = originalColor;
            }

            int endY = y + ((logs.Count + 1) * height);
            GUI.Label(new Rect(x, endY, width, height), $"End of {title}");
        }

        public static void ToScreen(string message, float? time = null, Color? color = null)
        {
            var logData = new LogData();
            logData.message = message;
            logData.time = time;
            logData.startTime = Time.time;
            logData.color = color;

            logs.Add(logData);
        }
    }
}

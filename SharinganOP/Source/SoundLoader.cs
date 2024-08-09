using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace SharinganOP
{
    public class SoundLoader : MonoBehaviour
    {
        IEnumerator Start()
        {
            string sharinganClipPath = SharinganOPLoader.folder + "SharinganOP.mp3";
            string url = string.Format("file://{0}", sharinganClipPath);
            SharinganOPLoader.Log($"Try to load music clip {url}");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SharinganOPLoader.sharinganAudioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
                SharinganOPLoader.sharinganAudioSource.clip = SharinganOPLoader.sharinganAudioClip;
                SharinganOPLoader.Log($"Loaded to load clip: '{sharinganClipPath}'");
            }
            else
            {
                UnityEngine.Debug.LogError($"[SharinganOP] Failed to load clip: {www.error}");
            }
        }

        public static AudioType ExtensionToAudioType(string extention)
        {
            switch (extention)
            {
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".mp3":
                    return AudioType.MPEG;
                case ".wav":
                    return AudioType.WAV;
                default:
                    break;
            }

            return AudioType.UNKNOWN;
        }
    }
}

using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

namespace Sharingan
{
    public class SoundLoader : MonoBehaviour
    {
        IEnumerator Start()
        {
            string sharinganClipPath = SharinganLoader.folder + "Sharingan.mp3";
            string url = string.Format("file://{0}", sharinganClipPath);
            //SharinganLoader.Log($"Try to load music clip {url}");

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SharinganLoader.sharinganAudioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
                SharinganLoader.sharinganAudioSource.clip = SharinganLoader.sharinganAudioClip;
                // SharinganLoader.Log($"Loaded to load clip: '{sharinganClipPath}'");
            }
            else
            {
                UnityEngine.Debug.LogError($"[Sharingan] Failed to load clip: {www.error}");
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

using System;
using System.Collections;
using System.IO;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;
using UnityEngine.Networking;

public class AudioConverter : MonoBehaviour
{
    private const string Mp3FileName = "audio.mp3";
    private static string mp3FilePath => Path.Combine(Application.temporaryCachePath, Mp3FileName);

    public static void SaveTextToMp3(AudioData audioData)
    {
        var bytes = Convert.FromBase64String(audioData.audioContent);
        File.WriteAllBytes(mp3FilePath, bytes);
    }

    public void LoadClipFromMp3(Action<AudioClip> onClipLoaded)
    {
        StartCoroutine(LoadClipFromMp3Cor(onClipLoaded));
    }

    private static IEnumerator LoadClipFromMp3Cor(Action<AudioClip> onClipLoaded)
    {
        string filePath = "file://" + mp3FilePath;
        using var webRequest = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG);

        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error loading audio clip: {webRequest.error}");
        }
        else
        {
            var downloadHandler = (DownloadHandlerAudioClip)webRequest.downloadHandler;
            if (downloadHandler.audioClip != null)
            {
                onClipLoaded.Invoke(downloadHandler.audioClip);
            }
            else
            {
                Debug.LogError("Failed to load audio clip, the audio clip is null.");
            }
        }
    }
}

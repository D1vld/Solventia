using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Template360
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField] private string videoUrl;
        [SerializeField] private VideoPlayer videoPlayer;
        private UnityWebRequest request;
        private bool videoIsReady = false;
        private string filePath;

        private enum State
        {
            Idle,
            CheckingVideo,
            DownloadingVideo,
            VideoReady,
            Error
        }

        private State currentState = State.Idle;
        private long contentLength = 0;

        private void Start()
        {
            filePath = Path.Combine(Application.persistentDataPath, "videocache.mp4");
            currentState = State.CheckingVideo;

            request = UnityWebRequest.Head(videoUrl);
            request.SendWebRequest();
        }

        private void Update()
        {
            switch (currentState)
            {
                case State.CheckingVideo:
                    if (request.isDone)
                    {
                        HandleHeadRequest();
                    }
                    break;
                case State.DownloadingVideo:
                    HandleDownloadRequest();
                    break;
                case State.VideoReady:
                    if (videoIsReady)
                    {
                        StreamVideo();
                        currentState = State.Idle;
                    }
                    break;
                case State.Error:
                    // Handle error if necessary
                    break;
            }
        }

        private void HandleHeadRequest()
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                string lengthHeader = request.GetResponseHeader("Content-Length");
                if (lengthHeader != null)
                {
                    contentLength = long.Parse(lengthHeader);
                    Debug.Log($"File size: {contentLength / 1024f} KB");
                }
                else
                {
                    Debug.LogWarning("Content-Length header is not available.");
                }
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                currentState = State.Error;
                return;
            }

            request.Dispose();

            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long localSize = fileInfo.Length;

                if (localSize < contentLength)
                {
                    StartFileDownload();
                }
                else
                {
                    videoIsReady = true;
                    currentState = State.VideoReady;
                }
            }
            else
            {
                StartFileDownload();
            }
        }

        private void StartFileDownload()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            request = UnityWebRequest.Get(videoUrl);
            request.downloadHandler = new DownloadHandlerFile(filePath, true);
            request.SendWebRequest();
            currentState = State.DownloadingVideo;
        }

        private void HandleDownloadRequest()
        {
            if (request.isDone)
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    videoIsReady = true;
                    Debug.Log("Download complete: 100%");
                    currentState = State.VideoReady;
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    currentState = State.Error;
                }
                request.Dispose();
            }
            else
            {
                if (request.downloadedBytes > 0)
                {
                    float progress = (float)request.downloadedBytes / contentLength;
                    GeneralManager.Instance.UI.UpdateProgress($"Download progress: {progress * 100:F2}%");
                }
            }
        }

        private void StreamVideo()
        {
            GeneralManager.Instance.UI.UpdateProgress($"");
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = "file://" + filePath;
            videoPlayer.Play();
        }

        private void StopDownload()
        {
            videoPlayer?.Stop();
            videoPlayer.url = null;
            if(request != null && !request.isDone)
            {
                request.Abort();
            }
        }

        internal void Initialize()
        {
        }

        private void OnApplicationQuit()
        {
            Debug.LogWarning("Stopping download");
            StopDownload();
            videoPlayer?.Stop();
        }
    }
}

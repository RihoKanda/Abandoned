using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Abandoned.Models;

namespace Abandoned.API
{
    /// <summary>
    /// APIクライアント
    /// </summary>
    public class APIClient : MonoBehaviour
    {
        [SerializeField] private string baseURL = "http://localhost:8080";

        private static APIClient instance;
        public static APIClient Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("APIClient");
                    instance = go.AddComponent<APIClient>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// ログイン・ユーザー作成
        /// </summary>
        public void Login(string deviceId, Action<LoginResponse> OnSuccess, Action<string> onError)
        {
            var request = new LoginRequest { device_id = deviceId};
            StartCoroutine(PostRequest("/api/auth/login", request, OnSuccess, onError));
        }

        /// <summary>
        /// ゲーム状態取得
        /// </summary>
        public void GetGameState(int userId, Action<GameStateResponse> onSuccess, Action<string> onError)
        {
            string url = $"/api/game/state?user_id={userId}";
            StartCoroutine(GetRequest(url, onSuccess, onError));
        }

        /// <summary>
        /// 放置開始
        /// </summary>
        public void StartIdle(int userId, Action<IdleStartResponse> onSuccess, Action<string> onError)
        {
            var request = new IdleStartRequest { user_id = userId };
            StartCoroutine(PostRequest("/api/idle/start", request, onSuccess, onError));
        }

        /// <summary>
        /// 放置終了
        /// </summary>
        public void FinishIdle(int userId, Action<IdleFinishResponse> onSuccess, Action<string> onError)
        {
            var request = new IdleFinishRequest { user_id = userId };
            StartCoroutine(PostRequest("/api/idle/finish", request, onSuccess, onError));
        }

        /// <summary>
        /// レベルアップ
        /// </summary>
        public void LevelUp(int userId, Action<LevelUpResponse> onSuccess, Action<string> onError)
        {
            var request = new LevelUpRequest { user_id = userId };
            StartCoroutine(PostRequest("/api/user/levelup", request, onSuccess, onError));
        }

        /// <summary>
        /// 能力強化
        /// </summary>
        public void Upgrade(int userId, UpgradeType upgradeType, Action<UpgradeResponse> onSuccess, Action<string> onError)
        {
            string typeStr = upgradeType switch
            {
                UpgradeType.Attack => "attack",
                UpgradeType.Speed => "speed",
                UpgradeType.HPRegain => "hp_regain",
                _ => "attack"
            };

            var request = new UpgradeRequest
            {
                user_id = userId,
                upgrade_type = typeStr
            };
            StartCoroutine(PostRequest("/api/user/upgrade", request, onSuccess, onError));
        }

        /// <summary>
        /// 進化
        /// </summary>
        public void Evolve(int userId, Action<EvolveResponse> onSuccess, Action<string> onError)
        {
            var request = new EvolveRequest { user_id = userId };
            StartCoroutine(PostRequest("/api/user/evolve", request, onSuccess, onError));
        }

        /// <summary>
        /// GETリクエスト
        /// </summary>
        private IEnumerator GetRequest<T>(string endpoint, Action<T> onSuccess, Action<string> onError)
        {
            string url = baseURL + endpoint;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonUtility.FromJson<APIResponse<T>>(request.downloadHandler.text);
                        if (response.success)
                        {
                            onSuccess?.Invoke(response.data);
                        }
                        else
                        {
                            onError?.Invoke(response.error);
                        }
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"Parse error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Network error: {request.error}");
                }
            }
        }

        /// <summary>
        /// POSTリクエスト
        /// </summary>
        private IEnumerator PostRequest<TRequest, TResponse>(string endpoint, TRequest requestData, 
            Action<TResponse> onSuccess, Action<string> onError)
        {
            string url = baseURL + endpoint;
            string json = JsonUtility.ToJson(requestData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var response = JsonUtility.FromJson<APIResponse<TResponse>>(request.downloadHandler.text);
                        if (response.success)
                        {
                            onSuccess?.Invoke(response.data);
                        }
                        else
                        {
                            onError?.Invoke(response.error);
                        }
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"Parse error: {e.Message}");
                    }
                }
                else
                {
                    onError?.Invoke($"Network error: {request.error}");
                }
            }
        }
    }
}
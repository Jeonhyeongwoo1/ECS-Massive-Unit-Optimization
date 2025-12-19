using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Google;
using MewVivor;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using Newtonsoft.Json;
using MewVivor.Factory;
using MewVivor.Model;
using MewVivor.Util;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Environment = MewVivor.Enum.Environment;

public class WebManager
{
    public string BaseUrl { get; private set; }

    public void Initialize()
    {
        var webSetting = Manager.I.Resource.Load<WebSettings>(nameof(WebSettings));
        switch (webSetting.environment)
        {
            case Environment.DEV:
                BaseUrl = webSetting.devIpAddress;
                break;
            case Environment.LIVE:
                break;
            case Environment.QA:
                BaseUrl = webSetting.qaIpAddress;
                break;
        }

        if (string.IsNullOrEmpty(BaseUrl))
        {
            Debug.LogError("Failed get url :" + webSetting.environment);
        }
    }

    public async UniTask<T> SendRequest<T>(string url, object obj, string method = "POST") where T : ResponseDataBase
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            string message = Manager.I.Data.LocalizationDataDict["Conenct_Internet"].GetValueByLanguage();
            Manager.I.UI.OpenSystemPopup(message);
            var responseData = new ResponseDataBase();
            responseData.statusCode = (int)ServerStatusCodeType.NetworkReachabilityNotReachable;
            return responseData as T;
        }

        return await SendRequestAsync<T>(url, obj, method);
    }

    private async UniTask<T> SendRequestAsync<T>(string url, object obj, string method) where T : ResponseDataBase
    {
        if (string.IsNullOrEmpty(BaseUrl))
        {
            Initialize();
        }

        string sendUrl = $"{BaseUrl}{url}";
        UnityWebRequest uwr = BuildRequest(sendUrl, obj, method);
        string text = string.Empty;
        T responseData = null;

        try
        {
            await uwr.SendWebRequest().ToUniTask();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                text = uwr.downloadHandler.text;
                responseData = JsonConvert.DeserializeObject<T>(text);
                Debug.Log($"{url} / {text}");
                return responseData;
            }
            else
            {
                text = uwr.downloadHandler.text;
                responseData = JsonConvert.DeserializeObject<T>(text);
            }
        }
        catch (Exception e)
        {
            text = uwr.downloadHandler.text;
            responseData = JsonConvert.DeserializeObject<T>(text);
            Debug.LogError($"{nameof(SendRequestAsync)} / error : {e.Message}");
            if (uwr != null) uwr.Dispose();
            if (responseData.message == ServerMessageType.AUTH_TOKEN_REFRESH_FAILED)
            {
                Manager.I.ChangeTitleScene();
                PlayerPrefs.SetString("AccessToken", "");
                PlayerPrefs.SetString("RefreshToken", "");
                TaskHelper.CompleteTcs();
                SocialLogout();
            }
        }

        // 실패 처리
        if (responseData != null && responseData.statusCode != (int)ServerStatusCodeType.Success)
        {
            bool isRefreshed = await TokenRefreshProcessAsync();
            if (isRefreshed)
            {
                // 새 요청 생성
                UnityWebRequest retryUwr = BuildRequest(sendUrl, obj, method);
                try
                {
                    await retryUwr.SendWebRequest().ToUniTask();

                    if (retryUwr.result == UnityWebRequest.Result.Success)
                    {
                        text = retryUwr.downloadHandler.text;
                        responseData = JsonConvert.DeserializeObject<T>(text);
                        Debug.Log($"{url} / {text}");
                        return responseData;
                    }
                    else
                    {
                        text = retryUwr.downloadHandler.text;
                        responseData = JsonConvert.DeserializeObject<T>(text);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(SendRequestAsync)} / retry failed: {e.Message}");
                }
                finally
                {
                    retryUwr.Dispose();
                }
            }
        }
        return responseData ?? default;
    }

    private UnityWebRequest BuildRequest(string url, object obj, string method)
    {
        UnityWebRequest uwr = new UnityWebRequest(url, method);

        if (obj != null)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
            Debug.Log($"SendUrl {url} / json {json}");
        }

        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        string accessToken = ModelFactory.CreateOrGetModel<UserModel>().AccessToken;
        if (!string.IsNullOrEmpty(accessToken))
        {
            uwr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
        }

        return uwr;
    }

    private async UniTask<bool> TokenRefreshProcessAsync()
    {
        if (SceneManager.GetActiveScene().name == SceneType.TitleScene.ToString())
        {
            return false;
        }

        var userModel = ModelFactory.CreateOrGetModel<UserModel>();
        string sendUrl = $"{BaseUrl}/auth/refresh";
        UnityWebRequest uwr = BuildRequest(sendUrl, new AuthRefreshRequestData
        {
            refreshToken = userModel.RefreshToken
        }, MethodType.POST.ToString());
        
        string text = string.Empty;
        AuthRefreshResponseData responseData = null;
        
        try
        {
            await uwr.SendWebRequest().ToUniTask();
            text = uwr.downloadHandler.text;
            responseData = JsonConvert.DeserializeObject<AuthRefreshResponseData>(text);
        }
        catch (Exception e)
        {
            text = uwr.downloadHandler.text;
            responseData = JsonConvert.DeserializeObject<AuthRefreshResponseData>(text);
            Debug.LogError($"{nameof(SendRequestAsync)} / error : {e.Message}");
            if (uwr != null) uwr.Dispose();
            if (responseData.message == ServerMessageType.AUTH_TOKEN_REFRESH_FAILED)
            {
                Manager.I.ChangeTitleScene();
                PlayerPrefs.SetString("AccessToken", "");
                PlayerPrefs.SetString("RefreshToken", "");
                TaskHelper.CompleteTcs();
                SocialLogout();
            }
        }

        if (responseData.statusCode != (int)ServerStatusCodeType.Success)
        {
            if (responseData.message == ServerMessageType.AUTH_INVALID_REFRESH_TOKEN)
            {
                if (SceneManager.GetActiveScene().name != SceneType.TitleScene.ToString())
                {
                    string message = Manager.I.Data.LocalizationDataDict["Duplicate_Logged"].GetValueByLanguage();
                    Manager.I.UI.OpenSystemPopup(message,
                        () => Manager.I.ChangeTitleScene());
                }
            }
            else
            {
                PlayerPrefs.SetString("AccessToken", "");
                PlayerPrefs.SetString("RefreshToken", "");
                Manager.I.UI.OpenSystemPopup(responseData.message.ToString(), () => Manager.I.ChangeTitleScene());
                TaskHelper.CompleteTcs();
                SocialLogout();
            }

            return false;
        }

        userModel.RefreshToken = responseData.data.refreshToken;
        userModel.AccessToken = responseData.data.accessToken;
        PlayerPrefs.SetString("AccessToken", responseData.data.accessToken);
        PlayerPrefs.SetString("RefreshToken", responseData.data.refreshToken);

        return true;
    }

    private void SocialLogout()
    {
        #if UNITY_ANDROID
        GoogleSignIn.DefaultInstance.SignOut();
        #elif UNITY_IOS
        
        #endif
    }
}

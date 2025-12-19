using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
#if UNITY_IOS || UNITY_IPHONE
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth;
using AppleAuth.Native;
#endif
using Cysharp.Threading.Tasks;
using Google;
using MewVivor.Common;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Key;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Presenter;
using MewVivor.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MewVivor.Controller
{    
    [Serializable]
    public struct AuthData
    {
        public string idToken;
        public string authCode;
        public string email;
    }

    public class TitleSceneController : BaseSceneController
    {
        [SerializeField] private Button _guestLoginButton;
        [SerializeField] private Button _googleLoginButton;
        [SerializeField] private Button _appleLoginButton;
        [SerializeField] private Button _gameStartbutton;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _loadingProcessText;
        [SerializeField] private AudioClip _titleAudioClip;
        [SerializeField] private ConfigurationSetting _configurationSetting;
        
        private string AccessToken
        {
            get
            {
                string token = PlayerPrefs.GetString(nameof(AccessToken));
                return token;
                // return AesEncryptionUtil.Decrypt(token);
            }
            set
            {
                var model = ModelFactory.CreateOrGetModel<UserModel>();
                model.AccessToken = value;
                // string token = AesEncryptionUtil.Encrypt(value);
                PlayerPrefs.SetString(nameof(AccessToken), value);
            }
        }

        private string RefreshToken
        {
            get
            {
                string token = PlayerPrefs.GetString(nameof(RefreshToken));
                // return AesEncryptionUtil.Decrypt(token);
                return token;
            }
            set
            {
                var model = ModelFactory.CreateOrGetModel<UserModel>();
                model.RefreshToken = value;
                // string token = AesEncryptionUtil.Encrypt(value);
                PlayerPrefs.SetString(nameof(RefreshToken), value);
            }
        }

        private string LastLoginType
        {
            get
            {
                return PlayerPrefs.GetString(nameof(LastLoginType));
            }
            set
            {
                PlayerPrefs.SetString(nameof(LastLoginType), value);
            }
        }
        
        // private string webClientId = "1033451965493-nnlc0ve136qm8i6h9spu057euih5eqd2.apps.googleusercontent.com";
        private GoogleSignInConfiguration googleSignInConfiguration;
#if UNITY_IOS || UNITY_IPHONE
        private IAppleAuthManager appleAuthManager;
#endif
        
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            
#if UNITY_ANDROID
            googleSignInConfiguration = new GoogleSignInConfiguration
            {
                WebClientId = _configurationSetting.webClientId,
                RequestIdToken = true,
                RequestEmail = true,
                RequestProfile = true
            };

            GoogleSignIn.Configuration = googleSignInConfiguration;
#endif
#if UNITY_IOS || UNITY_IPHONE
            if (AppleAuthManager.IsCurrentPlatformSupported)
            {
                var deserializer = new PayloadDeserializer();
                appleAuthManager = new AppleAuthManager(deserializer);
            }
#endif
        }

        private void Update()
        {
#if UNITY_IOS || UNITY_IPHONE
            appleAuthManager?.Update();
#endif
        }

        private async void Start()
        {
            _googleLoginButton.SafeAddButtonListener(() => OnLoginProcessByLoginTypeAsync(LoginType.GOOGLE).Forget());
            _appleLoginButton.SafeAddButtonListener(()=> OnLoginProcessByLoginTypeAsync(LoginType.APPLE).Forget());
            _guestLoginButton.SafeAddButtonListener(() => OnLoginProcessByLoginTypeAsync(LoginType.GUEST).Forget());

            _gameStartbutton.gameObject.SetActive(false);   
            _guestLoginButton.gameObject.SetActive(false);
            _googleLoginButton.gameObject.SetActive(false);
            _appleLoginButton.gameObject.SetActive(false);
            await Initialize();
        }

        private async UniTask Initialize()
        {
            ModelFactory.Clear();
            PresenterFactory.Clear();

            Manager.I.Audio.Play(Sound.BGM, _titleAudioClip, 1f, 0.3f);
            await Manager.I.Resource.LoadResourceAsync<Object>("PreLoad", (v) =>
            { 
                float value = v * 100;
                _loadingProcessText.text = Manager.I.LanguageType == LanguageType.Eng ? $"Loading..{(int)value}%" : $"로딩중..{(int)value}%";
                _slider.value = v;
            });
            
            _loadingProcessText.gameObject.SetActive(false);
            Manager.I.Initialize();
            AesEncryptionUtil.Initialize();
            // bool isSameVersion = await TryCheckVersion();
            // if (!isSameVersion)
            // {
            //     Debug.LogError($"Not match version");
            //     string message = Manager.I.Data.LocalizationDataDict["App_Update_1"].GetValueByLanguage();
            //     Manager.I.UI.OpenSystemPopup(message, OnOpenAppStore);
            //     return;
            // }

            GetUserData getUserData = new GetUserData();
            UserData userData = new UserData
            {
                id = 1.ToString()
            };

            Inventory inventory = new Inventory();
            inventory.jewel = 0;
            inventory.gold = 0;
            getUserData.inventory = inventory;
            getUserData.userData = userData;

            CreatureData creatureData =Manager.I.Data.CreatureDict[Const.PLAYER_DATA_ID];
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.Initialize(getUserData, creatureData);

            Manager.I.StartGame(GameType.MAIN, 1, false).Forget();
            return;
            Debug.Log($"access token {AccessToken}");
            Debug.Log($"refresh token {RefreshToken}");
            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken))
            {
                ActiveLoginButton();
            }
            else
            {
                if (LastLoginType == LoginType.GUEST.ToString())
                {
                    ActiveLoginButton();
                    return;
                }

                await UserLoginProcessAsync();
            }
        }

        private void OnOpenAppStore()
        {
            string storeURL = "";
#if UNITY_ANDROID
            storeURL = $"https://play.google.com/store/apps/details?id={Application.identifier}";
#elif UNITY_IOS
            storeURL = $"https://apps.apple.com/app/id6748491233";
#endif
            Application.OpenURL(storeURL);
        }

        private void ActiveLoginButton()
        {
#if UNITY_ANDROID
                _googleLoginButton.gameObject.SetActive(true);
#elif UNITY_IOS
                _appleLoginButton.gameObject.SetActive(true);
#endif
                _guestLoginButton.gameObject.SetActive(true);
        }

        public void MoveToLobbyScene()
        {
            Manager.I.Audio.Play(Sound.BGM, SoundKey.BGM_Outgame);
            SceneManager.LoadScene(SceneType.LobbyScene.ToString());
        }

        private IEnumerator Wait(float wait, Action action)
        {
            yield return new WaitForSeconds(wait);
            action?.Invoke();
        }

        #region Version

        
        private async UniTask<bool> TryCheckVersion()
        {
            var response =
                await Manager.I.Web.SendRequest<VersionResponseData>("/version", null, MethodType.GET.ToString());

            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                return false;
            }

            string version;
#if UNITY_EDITOR || UNITY_ANDROID
            version = response.data.androidVersion;
#elif UNITY_IOS
            version = response.data.iosVersion;
#endif
            string currentVersion = Application.version;
            if (!version.Equals(currentVersion))
            {
                return false;
            }

            return true;
        }
        
        #endregion
        
        
        private async UniTask UserLoginProcessAsync()
        {
            TaskHelper.InitTcs();
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.AccessToken = AccessToken;
            userModel.RefreshToken = RefreshToken;
            GetUserResponseDataData response =
                await Manager.I.Web.SendRequest<GetUserResponseDataData>("/user", null,
                    MethodType.GET.ToString());
                
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                if (response.message == ServerMessageType.AUTH_TOKEN_VERIFY_FAILED)
                {
                    var refreshResponseData =
                        await Manager.I.Web.SendRequest<AuthRefreshResponseData>("/auth/refresh",
                            new AuthRefreshRequestData()
                            {
                                refreshToken = RefreshToken
                            }, MethodType.POST.ToString());

                    if (refreshResponseData.statusCode != (int)ServerStatusCodeType.Success)
                    {
                        if (refreshResponseData.message == ServerMessageType.AUTH_INVALID_REFRESH_TOKEN)
                        {
                            AccessToken = null;
                            RefreshToken = null;
                            LastLoginType = null;
                            ActiveLoginButton();
                        }
                        else
                        {
                            Manager.I.UI.OpenSystemPopup(refreshResponseData.message.ToString());
                        }
                        return;
                    }
                    
                    AccessToken = refreshResponseData.data.accessToken;
                    RefreshToken = refreshResponseData.data.refreshToken;
                    response =
                        await Manager.I.Web.SendRequest<GetUserResponseDataData>("/user",  null,
                            MethodType.GET.ToString());
                }
                else
                {
                    Manager.I.UI.OpenSystemPopup(response.message.ToString());
                    return;
                }
            }
                
            TaskHelper.CompleteTcs();
            await OnLoginSuccessAsync(AccessToken, RefreshToken, response.data);
        }

        private async UniTask OnGuestLogin()
        {
            if (!string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken))
            {
                await UserLoginProcessAsync();
            }
            else
            {
                var guestLoginWarningPopupPresenter =
                    PresenterFactory.CreateOrGet<GuestLoginWarningPopupPresenter>();
                guestLoginWarningPopupPresenter.OpenPopup(async () =>
                {
                    var response = await Manager.I.Web.SendRequest<LoginResponseDataData>("/auth/login",
                        new AuthRequestData()
                        {
                            oauthType = LoginType.GUEST.ToString()
                        });

                    if (response.statusCode != (int)ServerStatusCodeType.Success)
                    {
                        Manager.I.UI.OpenPopup<UI_SystemPopup>();
                        return;
                    }

                    AccessToken = response.data.accessToken;
                    RefreshToken = response.data.refreshToken;
                    LastLoginType = LoginType.GUEST.ToString();
                    await OnLoginSuccessAsync(response.data.accessToken, response.data.refreshToken, response.data.user);
                });
            }
        }

#if UNITY_ANDROID
        private void SignInGoogle(Action<AuthData, LoginType> successCallback, Action failCallback)
        {
            GoogleSignIn.DefaultInstance.EnableDebugLogging(true);
            GoogleSignIn.DefaultInstance.SignIn().ContinueWith(async (task) =>
            {
                CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy(); 
                if (task.IsFaulted)
                {
                    using (IEnumerator<Exception> enumerator = task.Exception!.InnerExceptions.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            var error = (GoogleSignIn.SignInException)enumerator.Current;
                            Debug.Log("Got Error: " + error!.Status + " " + error.Message);
                        }
                        else
                        {
                            Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                        }
                    }
                    
                    foreach (var e in task.Exception.InnerExceptions)
                    {
                        var signInError = e as GoogleSignIn.SignInException;
                        Debug.LogError($"SignIn Failed. Status: {signInError.Status}, Message: {signInError.Message} / {signInError.StackTrace}");
                    }
                    
                    await UniTask.SwitchToMainThread(cancellationToken);

                    failCallback?.Invoke();
                }
                else if (task.IsCanceled)
                {
                    await UniTask.SwitchToMainThread(cancellationToken);

                    Debug.LogError($"Login failed {task.Exception}");
                    failCallback?.Invoke();
                }
                else
                {
                    await UniTask.SwitchToMainThread(cancellationToken);

                    GoogleSignInUser user = task.Result;
                    AuthData authData = new AuthData
                    {
                        idToken = user.IdToken,
                        authCode = user.AuthCode,
                        email = user.Email
                    };

                    Debug.Log($"login success token {authData.idToken} / auth code {authData.authCode}");
                    successCallback?.Invoke(authData, LoginType.GOOGLE);
                }
            });
        }
        
        #endif
        
#if UNITY_IOS || UNITY_IPHONE
        public void SignInApple(Action<AuthData, LoginType> successCallback = null, Action failCallback = null)
        {
            var rawNonce = GenerateRandomString(32);
            var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);
            
            //이거 확인해봐야함.
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce);
            Debug.Log("Apple Login");
            appleAuthManager.LoginWithAppleId(
                loginArgs,
                async credential =>
                {
                    CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy(); 
                    try
                    {
                        var appleIdCredential = credential as IAppleIDCredential;
                        var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                        var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
                        AuthData authData = new AuthData();
                        authData.authCode = authorizationCode;
                        authData.idToken = identityToken;
                        Debug.Log("OnSuccess");
                        successCallback?.Invoke(authData, LoginType.APPLE);
                    }
                    catch (AggregateException ex)
                    {
                        Debug.Log("failed :" + ex);
                        // 로그인 실패 토스트 메세지
                        await UniTask.SwitchToMainThread(cancellationToken);
                        failCallback?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("failed :" + ex);
                        // 로그인 실패 토스트 메세지
                        await UniTask.SwitchToMainThread(cancellationToken);
                        failCallback?.Invoke();
                    }
                    finally
                    {
                        // 로더 끄기
                    }
                },
                    error =>
                    {
                        var authorizationErrorCode = error.GetAuthorizationErrorCode();
                        Debug.Log("failed :" + authorizationErrorCode);
                        switch (authorizationErrorCode)
                        {
                            case AuthorizationErrorCode.Canceled:
                                failCallback?.Invoke();
                                break;
                            case AuthorizationErrorCode.Unknown:
                            case AuthorizationErrorCode.InvalidResponse:
                            case AuthorizationErrorCode.NotHandled:
                            case AuthorizationErrorCode.Failed:
                                failCallback?.Invoke();
                                // 로그인 실패 토스트 메세지
                                break;
                        }
                        // 로더 끄기
                    });
        }
        
        private string GenerateRandomString(int length)
        {
            const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
            var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
            var result = string.Empty;
            var remainingLength = length;

            var randomNumberHolder = new byte[1];
            while (remainingLength > 0)
            {
                var randomNumbers = new List<int>(16);
                for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
                {
                    cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                    randomNumbers.Add(randomNumberHolder[0]);
                }

                for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
                {
                    if (remainingLength == 0)
                    {
                        break;
                    }

                    var randomNumber = randomNumbers[randomNumberIndex];
                    if (randomNumber < charset.Length)
                    {
                        result += charset[randomNumber];
                        remainingLength--;
                    }
                }
            }

            return result;
        }

        private string GenerateSHA256NonceFromRawNonce(string rawNonce)
        {
            var sha = new SHA256Managed();
            var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
            var hash = sha.ComputeHash(utf8RawNonce);

            var result = string.Empty;
            for (var i = 0; i < hash.Length; i++)
            {
                result += hash[i].ToString("x2");
            }

            return result;
        }
#endif
        
        private async UniTask OnLoginProcessByLoginTypeAsync(LoginType loginType)
        {
            switch (loginType)
            {
                case LoginType.GOOGLE:
#if UNITY_ANDROID
                    SignInGoogle(OnSuccessSocialLogin, OnFailedSocialLogin);
#endif
                    break;
                case LoginType.APPLE:
#if UNITY_IOS || UNITY_IPHONE
                    SignInApple(OnSuccessSocialLogin, OnFailedSocialLogin);
#endif
                    break;
                case LoginType.GUEST:
                    OnGuestLogin().Forget();
                    break;
            }
        }

        private async void OnSuccessSocialLogin(AuthData authData, LoginType loginType)
        {
            var response = await Manager.I.Web.SendRequest<LoginResponseDataData>("/auth/login", new AuthRequestData()
            {
                idToken = authData.idToken,
                oauthType = loginType.ToString()
            });
            
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Manager.I.UI.OpenPopup<UI_SystemPopup>($"Failed error {response.message}");
                return;
            }

            AccessToken = response.data.accessToken;
            RefreshToken = response.data.refreshToken;
            LastLoginType = loginType == LoginType.APPLE ? LoginType.APPLE.ToString() : LoginType.GOOGLE.ToString();

            await OnLoginSuccessAsync(response.data.accessToken, response.data.refreshToken, response.data.user);
        }

        private async UniTask OnLoginSuccessAsync(string accessToken, string refreshToken, GetUserData getUserData)
        {
            CreatureData creatureData = Manager.I.Data.CreatureDict[Const.PLAYER_DATA_ID];
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.AccessToken = accessToken;
            userModel.RefreshToken = refreshToken;
            userModel.Initialize(getUserData, creatureData);
            
            TaskHelper.InitTcs();
            var huntPassPopupPresenter = PresenterFactory.CreateOrGet<HuntPassPopupPresenter>();
            await huntPassPopupPresenter.RequestHuntPassData();

            var shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            await shopPopupPresenter.GetShopDataAsync();

            var questPopupPresenter = PresenterFactory.CreateOrGet<QuestPopupPresenter>();
            await questPopupPresenter.GetQuestDataAsync();
            await RequestAuthResume();
            
            Manager.I.SelectedStageIndex = userModel.FindLastStageHistoryIndex();
            _gameStartbutton.gameObject.SetActive(true);
            
            TaskHelper.CompleteTcs();
            MoveToLobbyScene();
        }

        private void OnFailedSocialLogin()
        {
            string message = "";
#if UNITY_ANDROID
            message = Manager.I.Data.LocalizationDataDict["Failed_google_Login"].GetValueByLanguage();
#elif UNITY_IOS || UNITY_IPHONE
            message = Manager.I.Data.LocalizationDataDict["Failed_apple_Login"].GetValueByLanguage();
#endif
            Manager.I.UI.OpenSystemPopup(message);
        }
    }
}
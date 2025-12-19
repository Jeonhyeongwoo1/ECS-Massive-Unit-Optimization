using System;
using Cysharp.Threading.Tasks;
using MewVivor.Data;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.InGame.Data;
using MewVivor.InGame.View;
using MewVivor.Key;
using MewVivor.Managers;
using MewVivor.Model;
using MewVivor.Popup;
using MewVivor.Presenter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MewVivor
{
    public class Manager
    {
        public static Manager I => _instance ??= new Manager();
        
        public PoolManager Pool => I._pool ??= new PoolManager();
        public EventManager Event => I._event ??= new EventManager();
        public ResourcesManager Resource => I._resource ??= new ResourcesManager();
        public DataManager Data => I._data ??= new DataManager();
        public GameManager Game => I._game ??= new GameManager();
        public ObjectManager Object => I._object ??= new ObjectManager();
        public UIManager UI => I._ui ??= new UIManager();
        public AudioManager Audio => I._audio ??= new AudioManager();
        public WebManager Web => I._web ??= new WebManager();
        public AdsManager Ads => I._ads ??= new AdsManager();
        public TimeManager Time => I._time ??= new TimeManager();
        public IAPManager IAP => I._iap ??= new IAPManager();
        
        public GameContinueData GameContinueData
        {
            get =>I._gameContinueData ??= new GameContinueData();
            set => I._gameContinueData = value;
        }

        public int SelectedStageIndex
        {
            get => _selectedStageIndex;
            set => _selectedStageIndex = value;
        }

        private int _selectedStageIndex;

        private static Manager _instance;
        private EventManager _event;
        private PoolManager _pool;
        private ResourcesManager _resource;
        private DataManager _data;
        private GameManager _game;
        private ObjectManager _object;
        private UIManager _ui;
        private AudioManager _audio;
        private WebManager _web;
        private AdsManager _ads;
        private TimeManager _time;
        private IAPManager _iap;
        
        private GameContinueData _gameContinueData;
        public bool IsOnBGM
        {
            get => PlayerPrefs.GetInt(nameof(IsOnBGM), 0) == 0;
            set
            {
                PlayerPrefs.SetInt(nameof(IsOnBGM), value ? 0 : 1);
                if (value)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    string sceneName = scene.name;
                    if (sceneName == SceneType.LobbyScene.ToString())
                    {   
                        Audio.Play(Sound.BGM, SoundKey.BGM_Outgame);
                    }
                    else if (sceneName == SceneType.GameScene.ToString())
                    {
                        Audio.Play(Sound.BGM,
                            Game.GameType == GameType.MAIN ? SoundKey.BGM_Ingame : SoundKey.BGM_InfinityIngame, 1f, 0.35f);
                    }
                }
                else
                {
                    Audio.Stop(Sound.BGM);
                }
            }
        }

        public bool IsOnSfx
        {
            get => PlayerPrefs.GetInt(nameof(IsOnSfx), 0) == 0;
            set
            {
                PlayerPrefs.SetInt(nameof(IsOnSfx), value ? 0 : 1);
                if (!value)
                {
                    Audio.AllSfxStop();
                }
            }
        }

        public bool IsOnHaptic
        {
            get => PlayerPrefs.GetInt(nameof(IsOnHaptic), 0) == 0;
            set => PlayerPrefs.SetInt(nameof(IsOnHaptic), value ? 0 : 1);
        }

        public LanguageType LanguageType
        {
            get
            {
                LanguageType languageType = (LanguageType) PlayerPrefs.GetInt(nameof(LanguageType), 0);
                if (languageType == LanguageType.None)
                {
                    switch (Application.systemLanguage)
                    {
                        case SystemLanguage.Korean:
                            PlayerPrefs.SetInt(nameof(Enum.LanguageType), (int) LanguageType.Kor);
                            languageType = LanguageType.Kor;
                            break;
                        default:
                            PlayerPrefs.SetInt(nameof(Enum.LanguageType), (int) LanguageType.Eng);
                            languageType = LanguageType.Eng;
                            break;
                    }
                }

                return languageType;
            }
            set => PlayerPrefs.SetInt(nameof(Enum.LanguageType), (int) value);
        }

        public void Initialize()
        {
            Data.Initialize();
            Game.Initialize();
            Object.Initialize();
            Web.Initialize();
            Ads.Initialize();
            IAP.Initialize();
        }
        
        public async UniTask StartGame(GameType gameType, int selectedStage, bool useServer = true)
        {
            Event.Raise(GameEventType.StartGameForEditor);
            Pool.ClearDict();
            var playerModel = ModelFactory.CreateOrGetModel<PlayerModel>();
            playerModel.Reset();
            var stageModel = ModelFactory.CreateOrGetModel<StageModel>();
            stageModel.Reset();

            if (useServer)
            {
                UserGameStartRequestData requestData = new UserGameStartRequestData
                {
                    stage = gameType == GameType.MAIN ? selectedStage : 0
                };

                var response =
                    await Web.SendRequest<UserGameStartResponseData>($"/user-game/start/{gameType}", requestData);
                if (response.statusCode != (int)ServerStatusCodeType.Success)
                {
                    Debug.LogError($"failed game start {response.message}");
                    switch (response.message)
                    {
                        case ServerMessageType.USER_GAME_NOT_ENOUGH_STAMINA:
                            string stamina = Data.LocalizationDataDict["Not_enough_stamina"].GetValueByLanguage();
                            UI.OpenSystemPopup(stamina);
                            return;
                        case ServerMessageType.USER_GAME_NOT_ENOUGH_INFINITY_TICKET:
                            string ticket = Data.LocalizationDataDict["Not_enough_ticket"].GetValueByLanguage();
                            UI.OpenSystemPopup(ticket);
                            return;
                    }
                
                    return;
                }
            
                playerModel.SetGameSessionId(response.data.gameSessionId);
            }
            
            UI_LoadingPopup.I.ShowAndHideLoadingPopup(true);
            await UniTask.WaitForSeconds(1);
            string sceneName = SceneType.GameScene.ToString();
            await LoadSceneAsync((sceneName));
            await UniTask.WaitForSeconds(1);
            
            Game.StartGame(gameType, selectedStage);
            Audio.Play(Sound.BGM, gameType == GameType.MAIN ? SoundKey.BGM_Ingame : SoundKey.BGM_InfinityIngame, 1, 0.35f);
            var gameSceneUI = UI.ShowUI<UI_GameScene>();
            gameSceneUI.Initialize();
            UI_LoadingPopup.I.ShowAndHideLoadingPopup(false);
        }

        public async UniTask MoveToLobbyScene()
        {
            UI_LoadingPopup.I.ShowAndHideLoadingPopup(true);
            await UniTask.WaitForSeconds(1);
            var response = await Web.SendRequest<GetUserResponseDataData>("/user", null, MethodType.GET.ToString());
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                string message = I.Data.LocalizationDataDict["Unknown_error"].GetValueByLanguage();
                UI.OpenSystemPopup(message);
                ChangeTitleScene();
                return;
            }
            
            CreatureData creatureData = Data.CreatureDict[Const.PLAYER_DATA_ID];
            var userModel = ModelFactory.CreateOrGetModel<UserModel>();
            userModel.Initialize(response.data, creatureData);

            var huntPassPopupPresenter = PresenterFactory.CreateOrGet<HuntPassPopupPresenter>();
            await huntPassPopupPresenter.RequestHuntPassData();

            var shopPopupPresenter = PresenterFactory.CreateOrGet<ShopPopupPresenter>();
            await shopPopupPresenter.GetShopDataAsync();

            var questPopupPresenter = PresenterFactory.CreateOrGet<QuestPopupPresenter>();
            await questPopupPresenter.GetQuestDataAsync();
            SelectedStageIndex = userModel.FindLastStageHistoryIndex();
            
            Pool.ClearDict();
            Audio.StopFadeBGM();
            string sceneName = SceneType.LobbyScene.ToString();
            await LoadSceneAsync((sceneName));
            await UniTask.WaitForSeconds(1);
            UI_LoadingPopup.I.ShowAndHideLoadingPopup(false);
            Audio.Play(Sound.BGM, SoundKey.BGM_Outgame);
        }

        public void ChangeTitleScene()
        {
            string sceneName = SceneType.TitleScene.ToString();
            LoadSceneAsync(sceneName).Forget();
        }

        private async UniTask LoadSceneAsync(string sceneName)
        {
            Pool.ClearDict();
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (!operation.isDone)
            {
                try
                {
                    await UniTask.Yield();
                }
                catch (Exception e)
                {
                    Debug.LogError($"error {e.Message}");
                    
                    return;
                }
            }
        }

        public void ChangeLanguage(LanguageType languageType)
        {
            LanguageType = languageType;
            Event?.Raise(GameEventType.ChangeLanguage);
        }
    }
}
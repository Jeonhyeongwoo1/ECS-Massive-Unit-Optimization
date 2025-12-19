using System;
using System.Collections.Generic;
using MewVivor.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine.Serialization;

namespace MewVivor.Data.Server
{
    //{"statusCode":500,"message":"AUTH_LOGIN_FAILED","optionalMessage":"Login Failed","data":null}

    [Serializable]
    public class ResponseDataBase
    {
        public int statusCode;
        public ServerMessageType message; //에러 코드
        public string optionalMessage; //일반적인 메세지 코드
    }

    [Serializable]
    public class RequestDataBase
    {

    }

    [Serializable]
    public class VersionResponseData : ResponseDataBase
    {
        public VersionData data;
    }

    [Serializable]
    public class VersionData
    {
        public string androidVersion;
        public string iosVersion;
    }

    #region Auth
    
    [Serializable]
    public class LoginResponseDataData : ResponseDataBase
    {
        public LoginResponse data;
    }

    [Serializable]
    public class AuthRequestData : RequestDataBase
    {
        public string idToken;
        public string oauthType; //GOOGLE, APPLE, GUEST
    }

    [Serializable]
    public class LoginResponse
    {
        public string accessToken;
        public string refreshToken;
        public GetUserData user;
    }

    [Serializable]
    public class AuthRefreshRequestData : RequestDataBase
    {
        public string refreshToken;
    }

    [Serializable]
    public class AuthRefreshResponseData : ResponseDataBase
    {
        public TokenData data;
    }

    [Serializable]
    public class TokenData
    {
        public string accessToken;
        public string refreshToken;
    }

    [Serializable]
    public class GetUserData
    {
        public UserData userData;
        public List<UserEquipmentData> userEquipments;
        public Inventory inventory;
        public List<StageHistoryData> stageHistories;
        public InfinityHistoryData infinityHistory;
        public Dictionary<string, ShopSubscriptionInfoData> subscriptionInfo;
        public List<MailData> mails;
    }

    public class UserData
    {
        public string id;
        public string name;
        public long exp;
        public int stamina;
        public EvolutionOrderTypeByServer evolutionOrderType;
        public int evolutionCount;
        public int evolutionAtkCount;
        public int evolutionHpCount;
        public int evolutionMoveCount;
        public int evolutionCriticalCount;
        public int evolutionCriticalDamageCount;
        public int evolutionBoomCount;
        public int evolutionSkillCoolCount;
        public int evolutionBerserkerCount;
        public int evolutionInvincibilityCount;
        public DateTime createdAt;
        public DateTime updatedAt;
        public bool isNameChanged;
    }

    [Serializable]
    public class InfinityHistoryData
    {
        public int maxMonsterKillCount;
        public int survivalTime;
    }

    [Serializable]
    public class ShopSubscriptionInfoData
    {
        public bool isSubscribed;
        public DateTime? endDate;
    }
    
    [Serializable]
    public class StageHistoryData
    {
        public int stage;
        public int survivalTime; //밀리세컨드
        public bool isCleared;
    }

    [Serializable]
    public class UserEquipmentData
    {
        public string userEquipmentId;
        public int baseItemId;
        public EquipmentType baseEquipmentType;
        public bool isEquipped;
        public int level;
        public int usedScrollCount;
        public EquipmentGrade grade;
    }

    [Serializable]
    public class Inventory
    {
        public int weaponScroll;
        public int glovesScroll;
        public int ringScroll;
        public int armorScroll;
        public int beltScroll;
        public int bootsScroll;
        public int silverKey;
        public int goldKey;
        public int gold;
        public int jewel;
        public int infiniteTicket;
        public int reviveCost;
    }

    [Serializable]
    public class AuthResumeResponseData : ResponseDataBase
    {
        public AuthResumeData data;
    }

    public class AuthResumeData
    {
        public int infiniteTicket;
        public int stamina;
    }
    
    #endregion

    #region User

    [Serializable]
    public class GetUserResponseDataData : ResponseDataBase
    {
        public GetUserData data;
    }

    [Serializable]
    public class ChangeUserNameRequestData : RequestDataBase
    {
        public string name;
    }

    [Serializable]
    public class ChangeUserNameResponseData : ResponseDataBase
    {
        public ChangeUserNameData data;
    }

    [Serializable]
    public class ChangeUserNameData
    {
        public UserData userData;
        public Inventory inventory;
    }

    #endregion

    #region User-Game

    [Serializable]
    public class UserGameStartRequestData : RequestDataBase
    {
        public int stage;
    }

    [Serializable]
    public class UserGameStartResponseData : ResponseDataBase
    {
        public GameStartData data;
    }

    [Serializable]
    public class GameStartData
    {
        public string gameSessionId;
    }


    [Serializable]
    public class GameEndResponseData : ResponseDataBase
    {
        
    }
    
    [Serializable]
    public class GameEndRequestData : RequestDataBase
    {
        public string gameSessionId;
        public int clearedWave;
        public int normalMonsterKillCount;
        public int eliteMonsterKillCount;
        public int bossMonsterKillCount;
        public int survivalTime; //밀리 세컨즈
        public Dictionary<string, int> dropItems;
    }

    public class GameRetryRequestData : RequestDataBase
    {
        public string gameSessionId;
        public int clearedWave;
        public int normalMonsterKillCount;
        public int eliteMonsterKillCount;
        public int bossMonsterKillCount;
        public int survivalTime; //밀리 세컨즈
        public Dictionary<string, int> dropItems;
    }
    
    [Serializable]
    public class GameRetryResponseData : ResponseDataBase
    {
        public GameSessionData data;
    }
    
    [Serializable]
    public class GameSessionData
    {
        public string gameSessionId;
    }
    
    #endregion

    #region Sandbox

    [Serializable]
    public class CreateUserItemRequestData : RequestDataBase
    {
        public int baseItemCode;
    }

    [Serializable]
    public class CreateUserItemResponseData : ResponseDataBase
    {

    }

    [Serializable]
    public class AddItemRequestData : RequestDataBase
    {
        public Dictionary<string, int> items;
    }

    [Serializable]
    public class AddItemResponseData : ResponseDataBase
    {
        public Dictionary<string, int> data;
    }

    #endregion

    #region Equipment
    
    [Serializable]
    public class EquipResponseData : ResponseDataBase
    {
        public EquipResponseEquipmentData data;
    }

    public class EquipResponseEquipmentData
    {
        public UserEquipmentData currentEquippedItem;
        public UserEquipmentData previousEquippedItem;
    }

    [Serializable]
    public class EquipmentLevelUpResponseData : ResponseDataBase
    {
        public EquipmentLevelData data;
    }

    [Serializable]
    public class EquipmentLevelData
    {
        public UserEquipmentData updatedUserItem;
        public int gold;
        public int weaponScroll;
        public int glovesScroll;
        public int ringScroll;
        public int armorScroll;
        public int beltScroll;
        public int bootsScroll;   
    }

    [Serializable]
    public class EquipmentLevelDownResponseData : ResponseDataBase
    {
        public EquipmentLevelData data;
    }

    [Serializable]
    public class EquipmentMergeRequestData : RequestDataBase
    {
        public List<string> materialUserItemIds;
    }

    [Serializable]
    public class EquipmentMergeResponseData : ResponseDataBase
    {
        public EquipmentMergeData data;
    }

    [Serializable]
    public class EquipmentMergeAllResponseData : ResponseDataBase
    {
        public EquipmentMergeAllData data;
    }

    [Serializable]
    public class EquipmentMergeData
    {
        public List<UserEquipmentData> userItems;
        public UserEquipmentData resultEquipment;
    }

    [Serializable]
    public class EquipmentMergeAllData : EquipmentMergeData
    {
        public List<UserEquipmentData> addedUserItems;
    }

    
    #endregion

    #region Evolution

    [Serializable]
    public class EvolutionLevelUpRequestData : RequestDataBase
    {
        
    }

    [Serializable]
    public class EvolutionLevelUpResponseData : ResponseDataBase
    {
        public EvolutionLevelUpData data;
    }

    public class EvolutionLevelUpData
    {
        public UserData userData;
        public Inventory inventory;
        public EvolutionOrderType updatedEvolutionStat;
    }
    

    #endregion

    #region Quest

    [Serializable]
    public class GetAchievementResponseData : ResponseDataBase
    {
        public AchievementClearData data;
    }

    [Serializable]
    public class AchievementClearData
    {
        public Inventory inventory;
        public List<AchievementInfoData> achievements;
    }

    [Serializable]
    public class AchievementListData
    {
        public List<AchievementInfoData> list;
    }
    
    [Serializable]
    public class AchievementInfoData
    {
        public string id;
        public string text;
        public int defaultGoal;
        public int increaseGoal;
        public int? maxLevel;
        public List<Reward> rewards;
        public string createdAt;
        public string updatedAt;
        public int userLevel;
        public bool isClearable;
        public int currentValue;
        public int clearValue;
    }
    
    [Serializable]
    public class DailyQuestResponseData : ResponseDataBase
    {
        public DailyQuestListData data;
    }

    [Serializable]
    public class ClaimDailyQuestResponseData : ResponseDataBase
    {
        public ClaimDailyQuestData data;
    }

    [Serializable]
    public class ClaimDailyQuestData
    {
        public List<DailyQuestData> quests;
        public List<DailyQuestStageData> questStages;
        public int totalQuestPoint;
        public Inventory inventory;
    }

    public class DailyQuestListData
    {
        public List<DailyQuestData> quests;
        public List<DailyQuestStageData> questStages;
        public int totalQuestPoint;
        public List<AchievementInfoData> achievements;
    }

    [Serializable]
    public class DailyQuestData
    {
        public int questMissionId;
        public float progress;
        public bool isCleared;
        public bool isClaimed;
    }

    [Serializable]
    public class DailyQuestStageData
    {
        public int questStageId;
        public bool isClaimed;
        public bool isCleared;
    }

    [Serializable]
    public class DailyQuestClaimRequestData : RequestDataBase
    {
        public string questMissionId;
    }
    
    [Serializable]
    public class DailyStageQuestClaimRequestData : RequestDataBase
    {
        public string questStageId;
    }
    
    [Serializable]
    public class Reward
    {
        public int itemId;
        public int amount;
    }

    #endregion

    #region Mail

    [Serializable]
    public class CreateMailRequestData : RequestDataBase
    {
        public string title;
        public string text;
        public string type;
        public Dictionary<string, int> rewards;
        public DateTime expiredAt;
    }
    
    [Serializable]
    public class GetMailResponseData : ResponseDataBase
    {
        public GetMailData data;
    }

    [Serializable]
    public class GetMailData
    {
        public List<MailData> list;
        public bool unReadMailExist;
    }
    
    [Serializable]
    public class MailData
    {
        public string id;
        public string userId;
        public string type;
        public string title;
        public string text;
        public Dictionary<string, int> rewards;
        public DateTime createdAt;
        public DateTime updatedAt;
        public DateTime expiredAt;
        public bool isRead;
        public bool isClaimed;
    }

    [Serializable]
    public class ClaimMailResponseData : ResponseDataBase
    {
        public ClaimMailData data;
    }

    [Serializable]
    public class AllClaimMailResponseData : ResponseDataBase
    {
        public ClaimMailData data;
    }

    [Serializable]
    public class ClaimMailData
    {
        public UserData userData;
        public Inventory inventory;
        public List<MailData> mails;
    }
    
    #endregion

    #region Timer

    [Serializable]
    public class StaminaOrTicketResponseData : ResponseDataBase
    {
        public StaminaOrTicketData data;
    }

    [Serializable]
    public class StaminaOrTicketData
    {
        public UserData userData;
        public Inventory inventory;
    }
    
    #endregion

    #region Shop


    [Serializable]
    public class ShopResponseData : ResponseDataBase
    {
        public ShopData data;
    }

    [Serializable]
    public class ShopData
    {
        public List<ShopItemInfoData> items;
        public Inventory inventory;
        public List<UserEquipmentData> grantedEquipments;
        public GrantedItem grantedItems;
    }
    
    [Serializable]
    public class GrantedItem
    {
        public int weaponScroll;
        public int glovesScroll;
        public int ringScroll;
        public int armorScroll;
        public int beltScroll;
        public int bootsScroll;
        public int silverKey;
        public int goldKey;
        public int gold;
        public int jewel;
        public int infiniteTicket;
        public int reviveCost;
    }

    [Serializable]
    public class ShopItemInfoData
    {
        public string itemId;
        public bool isPurchasable;
        public string reason; // Optional
        public int buyCount;
    }

    [Serializable]
    public class ShopPurchaseRequestData : RequestDataBase
    {
        public string os;
        public string bundleId;
        public string productId;
        public string transactionId; 
        public int quantity;
    }

    #endregion

    #region HuntPass

    [Serializable]
    public class HuntPassResponseData : ResponseDataBase
    {
        public HuntPassData data;
    }
    
    [Serializable]
    public class HuntPassData
    {
        public string id;
        public string userId;
        public string passType;
        public int point; //실제 누적 포인트
        public int lastReceivedLevel; //마지막으로 패스보상 받은 패스 레벨
        public int lastReceivedPremiumLevel;
        public bool isPremium; //프리미엄 패스
        public int remainBoostCount; //남은 부스트 갯수
        public int dailyBoostChargeCount; //일일 부스트 충전 횟수
        public int? normalClaimableLevel;
        public int? premiumClaimableLevel; // 받을 수 있는 보상 레벨
    }

    [Serializable]
    public class HuntPassClaimRequestData : RequestDataBase
    {
        public bool isPremium;
    }

    [Serializable]
    public class HuntPassClaimResponseData : ResponseDataBase
    {
        public HuntPassClaimData data;
    }

    [Serializable]
    public class HuntPassClaimData
    {
        public UserData userData;
        public Inventory inventory;
        public HuntPassData userPassStatus;
    }
    
    #endregion
     
}
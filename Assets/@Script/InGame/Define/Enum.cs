namespace MewVivor.Enum
{
    public enum GameState
    {
        None = -1,
        Ready,
        Start,
        DeadPlayer,
        Done
    }
    
    public enum CreatureStateType
    {
        None,
        Idle,
        Move,
        Skill,
        Dead,
        Stun,
        Knockback, //넉백
    }

    public enum PlayerStateType
    {
        None,
        Normal,
        Invincibility,
    }

    public enum GameType
    {
        //서버에 보내는 api 방식과 통일하기 위해서 모든 문자를 대문자로 통일 
        NONE,
        MAIN,
        INFINITY 
    }

    public enum GameEventType
    {
        None = -1,
        //InGame
        DeadPlayer,
        UseResurrection,
        GameOver,
        SpawnMonster = 100,
        DeadMonster,
        LevelUp,
        UpgradeOrAddNewSkill,
        TakeDamageEliteOrBossMonster,
        ActivateDropItem,
        SpawnedBoss,
        EndWave,
        CompletedStage,
        PurchaseSupportSkill,
        ShowPausePopup,
        //OutGame
        ShowMergePopup,
        ShowMergeResultPopup,
        ChangeLanguage,
        OnShowMenuToggle,
        
        //Editor용
        LearnSkill = 10000,
        GameStart,
        ChangeGameType,
        ChangeStageForEditor,
        StartGameForEditor,
    }

    public enum DropableItemType
    {
        ItemBox,
        Magnet,
        Bomb,
        Potion,
        SkillUp,
        Gold,
        Jewel,
        Gem
    }

    public enum CreatureType
    {
        None,
        Player = 201000,
        Monster
    }

    public enum MonsterType
    {
        None,
        Normal,
        Elite,
        Boss,
        SuicideBomber
    }

    public enum SkillCategoryType
    {
        Normal,
        Ultimate
    }

    public enum DebuffType
    {
        None,
        Stun, //스턴 
        AddDamage, //추가 데미지 효과
        SlowSpeed, //이동속도 감속
        DotDamage, //지속 데미지
    }

    public enum SkillType
    {
        None,
        AttackSkill,
        PassiveSkill
    }
    
    public enum AttackSkillType
    {
        None = 0,
        Skill_10001 = 10001, //할퀴기(냥냥펀치)
        Skill_10011 = 10011, //털실(강철털실)
        Skill_10021 = 10021, //구슬(여우구슬)
        Skill_10031 = 10031, //생선뼈(청새치)
        Skill_10041 = 10041, //통조림(통조림 세트)
        Skill_10051 = 10051, //헤어볼(메가볼)
        Skill_10061 = 10061, //꼬리 채찍(꼬리 스피너)
        Skill_10071 = 10071, //자기장(고중력)
        Skill_10081 = 10081, //쳇바퀴 지뢰(고화력)
        Skill_10091 = 10091, //불타는 츄르(지옥불 츄르)
        Skill_10101 = 10101, //골공송(하악질)
        Skill_10111 = 10111, //고양이발(쿵쿵쿵)
        Skill_10121 = 10121, //번개(번개갑옷)
        Skill_10131 = 10131, //부메랑(고성능 부메랑)
        Skill_10141 = 10141, //캣닢(지식 학습)
    }

    public enum PassiveSkillType
    {
        None,
        Attack = 20101,
        MoveSpeed = 20102,
        ProjectileSpeed = 20103,
        HpRecoveryEfficiency = 20104,
        MaxHP = 20105,
        ExplsionRange = 20106,
        SkillRange = 20107,
        AddGoldAmount = 20108,
        SkillDuration = 20109,
        Defence = 20110,
        AddItemRange = 20111,
        CoolTime = 20112,
        CriticalPercent = 20113,
        CriticalDamage = 20114,
        EXP = 20115
    }

    public enum GemType
    {
        None,
        PurpleGem,
        GreenGem,
        BlueGem,
        RedGem
    }

    public enum SceneType
    {
        TitleScene,
        LobbyScene,
        GameScene,
    }

    public enum MaterialType
    {
        None,
        Gold = 50001,
        Jewel,
        Stamina,
        Exp,
        QuestExp,
        WeaponScroll = 50101,
        GlovesScroll,
        RingScroll,
        BeltScroll,
        ArmorScroll,
        BootsScroll,
        ReviveCost = 50201,
        BronzeKey,
        SilverKey,
        GoldKey,
        RandomScroll = 50301,
        AllRandomEquipmentBox,
        CommonEquipmentBox,
        UncommonEquipmentBox,
        RareEquipmentBox,
        EpicEquipmentBox,
        LegendaryEquipmentBox,
        InfiniteTicket = 50401,
    }

    public enum MaterialGrade
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Epic1,
        Epic2,
        Legendary,
        Legendary1,
        Legendary2,
        Legendary3,
    }
    
    public enum MenuToggleType
    {
        None,
        Battle,
        Equipment,
        Shop,
        Challenge,
        Evolution,
    }

    public enum WaveType
    {
        Normal,
        Boss
    }
    
    public enum WaveClearType
    {
        FirstWaveClear,
        SecondWaveClear,
        ThirdWaveClear
    }
    
    public enum EquipmentType
    {
        None = -1,
        Weapon,
        Gloves,
        Ring,
        Belt,
        Armor,
        Boots,
    }

    public enum EvolutionGrade
    {
        UnCommon,
        Rare,
        Epic
    }

    public enum EvolutionType
    {
        Atk,
        MaxHp,
        MoveSpeed,
        CriticalPercent,
        CriticalDamage,
        Boom,
        SkillCoolDown,
        Berserker,
        invincibility,
    }

    public enum EvolutionOrderTypeByServer
    {
        A,
        B,
        C,
        D,
        E
    }

    public enum EvolutionOrderType
    {
        Evolution_Atk,
        Evolution_Hp,
        Evolution_Move,
        Evolution_Critical,
        Evolution_CriticalDamage,
        Evolution_Boom,
        Evolution_SkillCool,
        Evolution_Berserker,
        Evolution_invincibility,
    }

    public enum EquipmentGrade
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Epic1,
        Epic2,
        Legendary,
        Legendary1,
        Legendary2,
        Legendary3,
        Myth,
    }

    public enum EquipmentSortType
    {
        Level,
        Grade,
    }
    
    public enum MergeType
    {
        None,
        SameItem,
        SameGrade
    }

    public enum EquipAbilityStatType
    {
        Grade,
        Level,
        ATK,
        HP
    }

    public enum OutGameContentButtonType
    {
        Setting,
        Checkout,
        Mission,
        Achievement,
        OfflineReward
    }
    
    public enum MissionType
    {
        Complete, // 완료시
        Daily,
        Weekly,
    }

    public enum MissionTarget // 미션 조건
    {
        DailyComplete, // 데일리 완료
        WeeklyComplete, // 위클리 완료
        StageEnter, // 스테이지 입장
        StageClear, // 스테이지 클리어
        EquipmentLevelUp, // 장비 레벨업
        CommonGachaOpen, // 일반 가챠 오픈 (광고 유도목적)
        AdvancedGachaOpen, // 고급 가챠 오픈 (광고 유도목적)
        OfflineRewardGet, // 오프라인 보상 
        FastOfflineRewardGet, // 빠른 오프라인 보상
        ShopProductBuy, // 상점 상품 구매
        Login, // 로그인
        EquipmentMerge, // 장비 합성
        MonsterAttack, // 몬스터 어택
        MonsterKill, // 몬스터 킬
        EliteMonsterAttack, // 엘리트 어택
        EliteMonsterKill, // 엘리트 킬
        BossKill, // 보스 킬
        DailyShopBuy, // 데일리 상점 상품 구매
        GachaOpen, // 가챠 오픈 (일반, 고급가챠 포함)
        ADWatchIng, // 광고 시청
    }

    public enum SettingType
    {
        BGM,
        SFX,
        Vibration
    }

    public enum StageWaveClearDepth
    {
        First,
        Second,
        Third
    }
    
    public enum Sound
    {
        BGM,
        SFX,
        Max,
    }

    public enum GlobalConfigName
    {
        StageEnterStamina,
        StaminaChargeTime,
        MaxChargedStamina,
        IngameItemBoxCycle,
        IngameItemBoxSpawnRange,
        WeightOwnedSkill,
        WeightNewSkill,
        OnlyAttackSkillUpToCount,
        EvolutionDefaultCost,
        EvolutionCostIncrease,
        EvolutionMaxCost,
        BlueGemExpAmount,
        GreenGemExpAmount,
        PurpleGemExpAmount,
        RedGemExpAmount,
        InfiniteEnterInfiniteTicket, //무한모드 입장시 티켓 사용 갯수
        InfiniteTicketChargeTime,
        MaxChargedInfiniteTicket,
        ReviveCostJewel,
        MonsterCountLimit
    }

    public enum InfiniteModeConfigName
    {
        OneWaveTime,
        NormalEnemyOnceSpawnTime,
        EliteEnemyOnceSpawnTime,
        BossEnemyOnceSpawnTime,
        FirstEnemySpawnCount,
        NormalEnemySpawnUpCount,
        NormalEnemyStatUpRate,
        EliteEnemyStatUpRate,
        BossStatUpRate,
        IncreasedDifficultyCycle,
        IncreasedDifficultyStatValue,
        GreenGemDropRate,
        BlueGemDropRate,
        PurpleGemDropRate,
        RedGemDropRate,
        BossAndEliteClearDropItemId,
        BossAndEliteClearDropItemAmount,
        RewardForNormalEnemyKillCount,
        RewardItemIdForNormalKill,
        RewardItemAmountForNormalKill,
        RewardForEliteEnemyKillCount,
        RewardItemIdForEliteKill,
        RewardItemAmountForEliteKill,
        RewardForBossEnemyKillCount,
        RewardItemIdForBossKill,
        RewardItemAmountForBossKill,
        RandomRewardIdForBossKill,
        RandomRewardMinAmountForBossKill,
        RandomRewardMaxAmountForBossKill,
        RandomRewardProbForBossKill,
        NormalEnemyDefaultAttack,
        NormalEnemyDefaultHP,
        EliteEnemyDefaultAttack,
        EliteEnemyDefaultHP,
        BossEnemyDefaultAttack,
        BossEnemyDefaultHP,
        SkillUpItemDropProb,
    }

    public enum LanguageType
    {
        None,
        Eng,
        Kor
    }

    public enum CreatureStatType
    {
        MaxHP,
        Atk,
        MoveSpeed,
        CriticalPercent,
        HpRecovery,
        CriticalDamagePercent,
        Exp,
        Defense,
        AddGoldAmount,
        MagneticRangePercent,
        SkillCoolTime,
        HpRecoveryEfficiency,
        AuraDamageUpPercent,
        ExplosionSkillSize,
        BulletSkillSize,
        BasicSkillCoolTime,
        CircleSkillSize,
        ItemBoxSpawnCoolTime,
        InvincibleChance,
        ExplosionBomb
    }

    public enum EquipmentSkillType
    {
        Power,
        SkillCoolTime,
        MaxHP,
        ResistDamage,
        Critical,
        CriticalDamage,
        HpRecoveryEfficiency,
        MoveSpeed,
        HpRecovery1s,
        ExplosionSkillSize,
        ItemBoxCool,
        BasicAttackCool,
        AuraDamage,
        AuraDamageUp,
        Revive,
        AddRevive,
        BulletSkillSize,
        CircleSkillSize,
    }
    
    public enum LoginType
    {
        GOOGLE,
        APPLE,
        GUEST
    }

    public enum ServerMessageType
    {
        None,
        OK,

        // Auth 관련 에러
        AUTH_LOGIN_FAILED, //'Login Failed',
        AUTH_INVALID_REFRESH_TOKEN, // 'Invalid Refresh Token',
        AUTH_TOKEN_REFRESH_FAILED, // 'Token Refresh Failed',
        AUTH_ACCESS_TOKEN_NOT_FOUND, // 'Access Token Not Found',
        AUTH_TOKEN_NOT_FOUND, //'Token Not Found',
        AUTH_TOKEN_NOT_FOUND_REDIS, //'Token Not Found - Step1',
        AUTH_TOKEN_NOT_FOUND_DB, //'Token Not Found - Step2',
        AUTH_TOKEN_VERIFY_FAILED, //'Token Verify Failed',
        AUTH_TOKEN_NOT_GENERATED, //'This token is not generated',
        AUTH_USER_NOT_FOUND, //'User Not Found',
        AUTH_GOOGLE_LOGIN_FAILED, // 'Google Login Failed',
        AUTH_APPLE_LOGIN_FAILED, // 'Apple Login Failed',

        // User 관련 에러
        NOT_ENOUGH_GOLD, //'Not Enough Gold',
        NAME_ALREADY_EXISTS, // 'Name Already Exists',
        NAME_LENGTH_INVALID, //'Name Length Invalid',
        NAME_SPECIAL_CHARACTER_INVALID, // 'Name Special Character Invalid',
        NAME_INCLUDES_PROFANE_WORD, //'Name Includes Profane Word',

        // 일반적인 에러
        INVALID_REQUEST, //'Invalid Request',
        UNAUTHORIZED_ACCESS, // 'Unauthorized Access',
        FORBIDDEN_ACCESS, //'Forbidden Access',
        INTERNAL_SERVER_ERROR, // 'Internal Server Error',

        // UserGame 관련 에러
        USER_GAME_NOT_FOUND, //'User Game Not Found',
        USER_GAME_NOT_ENOUGH_STAMINA, // 'User Game Not Enough Stamina',
        USER_GAME_NOT_ENOUGH_INFINITY_TICKET, // 'User Game Not Enough Infinity Ticket',
        USER_GAME_NOT_ENOUGH_JEWEL, //'User Game Not Enough Jewel',
        USER_INVENTORY_NOT_FOUND, //'User Inventory Not Found',
        USER_GAME_ALREADY_ENDED, //'User Game Already Ended',
        INVALID_WAVE_INDEX, //'Invalid Wave Index',
        INVALID_STAGE_INDEX, // 'Invalid Stage Index',

        // Achievement 관련 에러
        ACHIEVEMENT_NOT_FOUND, //'Achievement Not Found',
        ACHIEVEMENT_NOT_CLEARED, // 'Achievement Not Cleared',

        // Quest 관련 에러
        USER_QUEST_NOT_FOUND, //'User Quest Not Found',
        USER_QUEST_PROGRESS_NOT_FOUND, // 'User Quest Progress Not Found',
        USER_QUEST_NOT_COMPLETED, //'User Quest Not Completed',
        USER_QUEST_REWARD_ALREADY_CLAIMED, // 'User Quest Reward Already Claimed',
        QUEST_NOT_FOUND, //'Quest Not Found',
        USER_QUEST_DATA_NOT_FOR_TODAY, // 'User Quest Data Not For Today',
        USER_QUEST_STAGE_REWARD_NOT_FOUND, // 'User Quest Stage Reward Not Found',
        USER_QUEST_STAGE_REWARD_ALREADY_CLAIMED, // 'User Quest Stage Reward Already Claimed',
        QUEST_STAGE_NOT_FOUND, //'Quest Stage Not Found',
        USER_QUEST_STAGE_REWARD_NOT_ENOUGH_POINT, //'User Quest Stage Reward Not Enough Point',

        // Item 관련 에러
        ITEM_NOT_FOUND, //'Item Not Found',
        USER_ITEM_NOT_FOUND, // 'User Item Not Found',

        USER_ITEM_NOT_FOUND_MATERIAL, // 'User Item Not Found - Material',
        MATERIAL_ITEMS_SHOULD_BE_LEVEL_1, // 'Material Items Should be level 1',
        MATERIAL_ITEMS_SHOULD_BE_GRADE_COMMON, // 'Material Items Should be grade common',
        MATERIAL_ITEMS_SHOULD_BE_SAME_GRADE, // 'Material Items Should be same grade',
        MATERIAL_ITEMS_SHOULD_BE_SAME_ITEM, // 'Material Items Should be same item',
        MATERIAL_ITEMS_SHOULD_BE_SAME_TYPE, //'Material Items Should be same type',

        USER_ITEM_NOT_ENOUGH_BASE_ITEM, // 'User Item Not Enough Base Item',
        USER_ITEM_NOT_ENOUGH_MATERIAL_ITEMS, // 'User Item Not Enough Material',

        USER_ITEM_NOT_ENOUGH_GOLD, // 'User Item Not Enough Gold',
        USER_ITEM_NOT_ENOUGH_SCROLL, // 'User Item Not Enough Scroll',

        ITEM_LEVEL_ALREADY_MAX, //'Item Level Already Max',
        ITEM_LEVEL_ALREADY_MIN, //'Item Level Already Min',

        // Mail 관련 에러
        MAIL_NOT_FOUND, // 'Mail Not Found',
        MAIL_NOT_FOUND_USER, // 'Mail Not Found In User',
        MAIL_NOT_FOUND_MAIL, // 'Mail Not Found',
        MAIL_NOT_FOUND_EXPIRED, // 'Mail Not Found - Expired',
        MAIL_ALREADY_CLAIMED, // 'Mail Already Claimed',
        MAIL_NOT_FOUND_TITLE_AND_TEXT, // 'Mail Title and Text are required',

        // Shop 관련 에러
        SHOP_ITEM_NOT_FOUND, // 'Shop Item Not Found',
        SHOP_PURCHASE_LIMIT_EXCEEDED, // 'Shop Purchase Limit Exceeded',
        SHOP_ITEM_NOT_FOUND_COST, //'Shop Item Not Found - Cost',
        SHOP_PURCHASE_INVALID_OS, //'Shop Purchase Invalid OS',
        NOT_ENOUGH_COST, //'Not Enough Cost',
        NOT_ENOUGH_JEWEL, // 'Not Enough Jewel',
        NOT_ENOUGH_ITEM_QUANTITY, //'Not Enough Item Quantity',

        // PeriodicCharge 관련 에러
        INVALID_CHARGE_TYPE, //'Invalid Charge Type',
        USER_CHARGE_STAMINA_MAX, // 'User Charge Stamina Max',
        USER_CHARGE_INFINITY_TICKET_MAX, // 'User Charge Infinity Ticket Max',

        // Apple IAP 관련 에러
        INAPP_PURCHASE_VALIDATION_INVALID_PRODUCT_ID, // 'Invalid Product ID',
        INAPP_PURCHASE_VALIDATION_FAILURE, //'Purchase Validation Failure',
        INAPP_PURCHASE_VALIDATION_NETWORK_ERROR, // 'Purchase Validation Network Error',
        INAPP_PURCHASE_VALIDATION_MISSING_SIGNED_INFO, // 'Signed transaction info is missing',
        INAPP_PURCHASE_VALIDATION_INVALID_JWS_FORMAT, // 'Invalid JWS format',
        INAPP_PURCHASE_VALIDATION_DECODE_FAIL, // 'Failed to decode transaction info',
        INAPP_PURCHASE_VALIDATION_INVALID_BUNDLE_ID, // 'Invalid Bundle ID',
        INAPP_PURCHASE_VALIDATION_REVOKED, // 'Transaction has been revoked',
        INAPP_PURCHASE_VALIDATION_EXPIRED, // 'Transaction has expired',
        INAPP_PURCHASE_VALIDATION_MISSING_ENV_VARS, // 'Missing required environment variables for Apple IAP',
        INAPP_PURCHASE_VALIDATION_INVALID_ENVIRONMENT, // 'Invalid environment (Sandbox/Production mismatch)',
        INAPP_PURCHASE_VALIDATION_INVALID_QUANTITY, // 'Invalid quantity (must be greater than 0)',

        // Google IAP 관련 에러
        GOOGLE_PURCHASE_VALIDATION_FAILURE, //'Google Purchase Validation Failure',
        GOOGLE_PURCHASE_VALIDATION_NETWORK_ERROR, // 'Google Purchase Validation Network Error',
        GOOGLE_PURCHASE_VALIDATION_CANCELED, //'Google Purchase Validation Canceled',
        GOOGLE_PURCHASE_VALIDATION_INVALID_PURCHASE_STATE, // 'Invalid Purchase State',
        GOOGLE_PURCHASE_VALIDATION_MISSING_ENV_VARS, //'Missing required environment variables for Google IAP',
        GOOGLE_PURCHASE_VALIDATION_INVALID_PRODUCT_ID, // 'Invalid Product ID',

        // Pass 관련 에러
        PASS_ALREADY_CLAIMED_LEVEL, //'Pass Already Claimed Level',
        PASS_REWARD_NOT_FOUND, // 'Pass Reward Not Found',
        PASS_NOT_ENOUGH_POINT, // 'Pass Not Enough Point',
        PASS_DAILY_BOOST_COUNT_LIMIT_EXCEEDED, // 'Pass Daily Boost Count Limit Exceeded',
        PASS_TYPE_INVALID, //'Invalid Pass Type',
        PASS_NOT_PREMIUM, // 'Pass Not Premium',
        PASS_NOT_ENOUGH_LEVEL, // 'Pass Not Enough Level',
    }

    public enum ServerStatusCodeType
    {
        Success = 200,
        NetworkReachabilityNotReachable = 1000,
    }

    public enum RetryType
    {
        ADVERTISEMENT,
        JEWEL,
    }

    public enum GameEndType
    {
        LOSE,
        CLEAR,
    }

    public enum AchievementMissionTarget
    {
        PlayGame = 90001,
        InfiniteModeLifeTime = 90002,
        GetEquipment,
        MonsterKill,
        EquipmentLevelUp,
        EquipmentGradeUp,
        PlayerGrowth,
        UseCost,
    }

    public enum MailType
    {
        LevelUp_Mail,
        Vip_Mail,
        Vvip_Mail,
    }

    public enum ChargeType
    {
        STAMINA,
        INFINITY_TICKET
    }

    public enum ChallengeType
    {
        Infinity
    }

    public enum QuestTabType
    {
        DailyQuest,
        Achievement
    }

    public enum QuestType
    {
        Complete,
        Daily,
        Weekly
    }

    public enum QuestMissionTarget
    {
        Login,
        PlayGame,
        MonsterKill,
        EqiupmentLevelUp,
        EqiupmentGradeUp,
        BuyShopProduct,
        BuyGacha,
        ADWatchIng,
    }

    public enum ShopGroupType
    {
        Package,
        Subscribe,
        Gacha,
        Gold,
        Jewel,
        Ticket,
        Pass,
        Scroll
    }

    public enum PayType
    {
        IAP,
        Ads,
        JewelAndSilverKey,
        JewelAndGoldKey,
        Free,
        Jewel,
    }

    public enum BuyLimitType
    {
        None,
        Account,
        Day,
        Daily,
    }

    public enum ShopIdType
    {
        Shop_PackageNewbie,
        Shop_PackageGrowth,
        Shop_PackageCost,
        Shop_VIP,
        Shop_VVIP,
        Shop_NormalGacha_Ads,
        Shop_NormalGacha, 
        Shop_RareGacha,
        Shop_Gold1,
        Shop_Gold2,
        Shop_Gold3,
        Shop_Gold4,
        Shop_Jewel1,
        Shop_Jewel2,
        Shop_Jewel3,
        Shop_Jewel4,
        Shop_Jewel5,
        Shop_Jewel6,
        Shop_Jewel7,
        Shop_InfiniteTicket1,
        Shop_HuntPass,
        Shop_RandomScroll,
        Shop_WeaponScroll
    }

    public enum PassType
    {
        HuntPass,
    }
    
    public enum ShopPurchaseType
    {
        Scroll,
        Gold
    }

    public enum VipType
    {
        Shop_VIP,
        Shop_VVIP
    }

    public enum HuntPassConfigName
    {
        WavePassAdsName,
        WavePassAdsCount,
        WavePassBoostCount,
        WavePassBoostRatio,
        NormalMonsterKillPointCount,
        NormalMonsterKillPointAmount,
        EliteMonsterKillPointCount,
        EliteMonsterKillPointAmount,
        BossMonsterKillPointCount,
        BossMonsterKillPointAmount,   
    }
    
    public enum ShopIAPIdType
    {
        None,
        shop_huntpass,
        shop_jewel_100,
        shop_jewel_10000,
        shop_jewel_3000,
        shop_jewel_500,
        shop_jewel_5000,
        shop_pack_1,
        shop_pack_2,
        shop_pack_3,
        shop_vip,
        shop_vvip
    }

    public enum OS
    {
        EDITOR,
        AOS,
        IOS
    }

    public enum CurrencyType
    {
        Gold,
        Jewel
    }
    
    public enum GachaProbabilityTableType
    {
        EquipmentNormal,
        EquipmentRare,
        Scroll
    }
    
    public enum Environment
    {
        DEV,
        LIVE,
        QA
    }
    
    public enum MethodType
    {
        POST,
        GET,
        PATCH
    }
}

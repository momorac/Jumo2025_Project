/// <summary>레시피 대분류 (차림요리 / 단품요리)</summary>
public enum RecipeCategory
{
    None = 0,
    TableDish = 1,   // 차림요리
    SingleDish = 2   // 단품요리
}

/// <summary>레시피 소분류</summary>
public enum RecipeSubCategory
{
    None = 0,

    // 차림요리 소분류
    Rice = 101,       // 밥
    Soup = 102,       // 국
    SideDish = 103,   // 반찬
    Kimchi = 104,     // 김치

    // 단품요리 소분류
    StewBowl = 201,   // 국밥/찌개
    Jeon = 202,    // 전
    Dish = 203        // 요리
}

/// <summary>개별 레시피 타입</summary>
public enum RecipeType
{
    None = 0,

    // ===== 차림요리 - 김치 (1000번대) =====
    CabbageKimchi = 1001,     // 배추김치
    Kkakdugi = 1002,          // 깍두기
    ScallionKimchi = 1003,  // 파김치
    Dongchimi = 1004,         // 동치미
    NabakKimchi = 1005,       // 나박김치

    // ===== 차림요리 - 밥 (1100번대) =====
    MixedGrainRice = 1101,    // 잡곡밥
    WhiteRice = 1102,         // 쌀밥
    SweetRice = 1103,         // 찹쌀밥
    BarleyRice = 1104,        // 보리밥

    // ===== 차림요리 - 국 (1200번대) =====
    RadishSoup = 1201,        // 무국
    EggSoup = 1202,           // 계란국
    KimchiSoup = 1203,        // 김치국
    PorkSoup = 1204,          // 돼지고기국

    // ===== 차림요리 - 반찬 (1300번대) =====
    RadishSalad = 1301,       // 무생채
    FriedEgg = 1302,          // 계란부침
    Jangjorim = 1303,         // 장조림
    ChiveSalad = 1304,     // 부추무침
    RadishBraised = 1305,     // 무조림

    // ===== 단품요리 - 국밥/찌개 (2000번대) =====
    KimchiStew = 2001,        // 김치찌개
    SoybeanStew = 2002,  // 된장찌개
    PorkRiceSoup = 2003,      // 돼지국밥
    DongtaeStew = 2004,        // 동태탕
    Yukgaejang = 2005,     // 육개장
    ChickenSoup = 2006,       // 닭백숙
    SoyPasteRiceSoup = 2007,  // 장국밥
    DriedPollocSoup = 2008,   // 북어국
    GalbiSoup = 2009,      // 갈비탕
    SoftTofuStew = 2010,      // 순두부찌개

    // ===== 단품요리 - 전 (2100번대) =====
    KimchiJeon = 2101,     // 김치전
    TofuJeon = 2102,       // 두부전
    FishJeon = 2103,       // 동태전
    MeatJeon = 2104,       // 육전
    GreenOnionJeon = 2105, // 파전
    SeafoodJeon = 2106,    // 해물파전
    ChiveJeon = 2107,      // 부추전
    CabbageJeon = 2108,    // 배추전

    // ===== 단품요리 - 요리 (2200번대) =====
    GrilledPork = 2201,       // 돼지구이
    BoiledPork = 2202,        // 수육
    GalbiJjim = 2203,     // 갈비찜 
    Bulgogi = 2204,           // 불고기
    BraisedFish = 2205,       // 생선조림
    GrilledFish = 2206,       // 생선구이
    ChickenSkewer = 2207,     // 닭꼬치
    TofuTriple = 2208         // 두부삼합
}

/// <summary>조리 설비 타입</summary>
public enum CookingFacilityType
{
    None = 0,
    Pot = 1,        // 솥 - 밥, 국
    JangdokJar = 2, // 장독 - 김치 (물/장작 불필요)
    Agungi = 3,   // 가마솥 - 반찬, 국밥/찌개
    Brazier = 4     // 화로 - 전, 요리
}

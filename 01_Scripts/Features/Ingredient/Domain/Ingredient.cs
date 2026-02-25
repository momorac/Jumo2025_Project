/// <summary>재료 대분류 카테고리</summary>
public enum IngredientCategory
{
    None = 0,
    Grain = 1,      // 곡식
    Vegetable = 2,  // 채소
    Meat = 3,       // 육류
    Seasoning = 4,  // 부재료
    Intermediate = 5 // 중간재료 (김치류)
}

/// <summary>개별 재료 타입</summary>
public enum IngredientType
{
    None = 0,

    // 곡식 (Grain) - 100번대
    MixedGrain = 101,   // 잡곡
    Barley = 102,       // 보리
    Rice = 103,         // 쌀
    SweetRice = 104,   // 찹쌀

    // 채소 (Vegetable) - 200번대
    Cabbage = 201,      // 배추
    Radish = 202,       // 무
    GreenOnion = 203,   // 파
    Chive = 204,        // 부추
    Pepper = 205,       // 고추 

    // 육류 (Meat) - 300번대
    Chicken = 301,      // 닭
    Egg = 302,          // 계란
    Fish = 303,         // 생선
    Pork = 304,         // 돼지

    // 부재료 (Seasoning) - 400번대
    Tofu = 401,         // 두부
    Flour = 402,        // 밀가루
    Salt = 403,         // 소금
    Paste = 404,        // 장

    // 중간재료 (Intermediate) - 500번대 (레시피 결과물이자 재료)
    KimchiCabbage = 501,     // 배추김치
    KimchiRadish = 502,      // 깍두기
    KimchiGreenOnion = 503,  // 파김치
    Dongchimi = 504,         // 동치미
    NabakKimchi = 505        // 나박김치
}

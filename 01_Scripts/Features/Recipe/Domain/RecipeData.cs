using System;
using System.Collections.Generic;

/// <summary>레시피 런타임 상태 데이터 (저장 대상)</summary>
[Serializable]
public class RecipeData
{
    /// <summary>해금된 레시피 목록</summary>
    public HashSet<RecipeType> UnlockedRecipes = new HashSet<RecipeType>();

    /// <summary>버퍼 자원 재고 (밥/김치 등)</summary>
    public Dictionary<RecipeType, int> BufferStock = new Dictionary<RecipeType, int>();

    public RecipeData()
    {
    }
}

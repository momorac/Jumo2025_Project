using System;
using System.Collections.Generic;

/// <summary>재료 런타임 상태 데이터 (저장 대상)</summary>
[Serializable]
public class IngredientData
{
    /// <summary>해금된 재료 목록</summary>
    public HashSet<IngredientType> UnlockedIngredients = new HashSet<IngredientType>();

    /// <summary>재료별 보유량</summary>
    public Dictionary<IngredientType, int> Inventory = new Dictionary<IngredientType, int>();

    public IngredientData()
    {
    }
}

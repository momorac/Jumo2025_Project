using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

/// <summary>
/// 레시피 시스템 테스트용 스크립트 (UniRx 기반)
/// </summary>
public class RecipeServiceTester : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private RecipeType testRecipe = RecipeType.WhiteRice;
    [SerializeField] private int testBufferAmount = 5;

    [Header("UI 연결 (선택)")]
    [SerializeField] private Text resultText;

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private void Start()
    {
        // 키보드 입력 바인딩
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => TestCook())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.T))
            .Subscribe(_ => TestUnlockAll())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Y))
            .Subscribe(_ => PrintCookableRecipes())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.U))
            .Subscribe(_ => PrintBufferStock())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.I))
            .Subscribe(_ => TestConsumeBuffer())
            .AddTo(disposables);

        Log("레시피 테스트 준비 완료\nR: 조리 | T: 전체해금 | Y: 조리가능목록 | U: 버퍼재고 | I: 버퍼소비");
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }

    #region 테스트 메서드

    [ContextMenu("테스트: 레시피 조리")]
    public void TestCook()
    {
        if (!CheckService()) return;

        var def = App.RecipeService.GetDefinition(testRecipe);
        if (def == null)
        {
            Log($"<color=red>레시피 정의 없음: {testRecipe}</color>");
            return;
        }

        // 재료 확인
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[조리 시도] {def.displayName} ({testRecipe})");
        sb.AppendLine("필요 재료:");
        foreach (var req in def.ingredients)
        {
            int have = App.IngredientService.GetAmount(req.ingredient);
            string status = have >= req.amount ? "✓" : "✗";
            sb.AppendLine($"  {status} {req.ingredient}: {have}/{req.amount}");
        }

        bool success = App.RecipeService.Cook(testRecipe);
        sb.AppendLine($"결과: {(success ? "<color=green>성공</color>" : "<color=red>실패</color>")}");

        Log(sb.ToString());
    }

    [ContextMenu("테스트: 전체 레시피 해금")]
    public void TestUnlockAll()
    {
        if (!CheckService()) return;

        int count = 0;
        foreach (RecipeType type in Enum.GetValues(typeof(RecipeType)))
        {
            if (type != RecipeType.None && App.RecipeService.Unlock(type))
                count++;
        }
        Log($"[해금] {count}개 레시피 해금 완료");
    }

    [ContextMenu("조리 가능 레시피 출력")]
    public void PrintCookableRecipes()
    {
        if (!CheckService()) return;

        var cookable = App.RecipeService.GetCookableRecipes();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"=== 조리 가능 레시피 ({cookable.Count}개) ===");

        foreach (RecipeSubCategory subCat in Enum.GetValues(typeof(RecipeSubCategory)))
        {
            if (subCat == RecipeSubCategory.None) continue;

            var recipes = App.RecipeService.GetCookableBySubCategory(subCat);
            if (recipes.Count == 0) continue;

            sb.AppendLine($"\n[{subCat}]");
            foreach (var def in recipes)
            {
                sb.AppendLine($"  {def.displayName}");
            }
        }

        Log(sb.ToString());
    }

    [ContextMenu("버퍼 재고 출력")]
    public void PrintBufferStock()
    {
        if (!CheckService()) return;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== 버퍼 재고 현황 ===");

        // 밥 종류
        sb.AppendLine("\n[밥]");
        foreach (var def in App.RecipeService.GetBySubCategory(RecipeSubCategory.Rice))
        {
            int stock = App.RecipeService.GetBufferStock(def.type);
            if (stock > 0)
                sb.AppendLine($"  {def.displayName}: {stock}");
        }

        // 김치 종류
        sb.AppendLine("\n[김치]");
        foreach (var def in App.RecipeService.GetBySubCategory(RecipeSubCategory.Kimchi))
        {
            int stock = App.RecipeService.GetBufferStock(def.type);
            if (stock > 0)
                sb.AppendLine($"  {def.displayName}: {stock}");
        }

        Log(sb.ToString());
    }

    [ContextMenu("테스트: 버퍼 소비")]
    public void TestConsumeBuffer()
    {
        if (!CheckService()) return;

        int before = App.RecipeService.GetBufferStock(testRecipe);
        bool success = App.RecipeService.ConsumeFromBuffer(testRecipe);
        int after = App.RecipeService.GetBufferStock(testRecipe);

        Log($"[버퍼 소비] {testRecipe} | 결과: {(success ? "성공" : "실패")} | {before} → {after}");
    }

    [ContextMenu("테스트: 재료 추가 (조리용)")]
    public void AddIngredientsForCooking()
    {
        if (!CheckService()) return;

        // 기본 재료 추가
        App.IngredientService.Add(IngredientType.Rice, 10);
        App.IngredientService.Add(IngredientType.Radish, 10);
        App.IngredientService.Add(IngredientType.Salt, 10);
        App.IngredientService.Add(IngredientType.GreenOnion, 10);
        App.IngredientService.Add(IngredientType.Pepper, 10);
        App.IngredientService.Add(IngredientType.Cabbage, 10);
        App.IngredientService.Add(IngredientType.Tofu, 10);
        App.IngredientService.Add(IngredientType.Flour, 10);
        App.IngredientService.Add(IngredientType.Egg, 10);
        App.IngredientService.Add(IngredientType.Pork, 10);

        Log("[치트] 기본 재료 10개씩 추가 완료");
    }

    #endregion

    #region 헬퍼

    private bool CheckService()
    {
        if (App.RecipeService == null)
        {
            Log("<color=red>RecipeService가 초기화되지 않았습니다.</color>");
            return false;
        }
        return true;
    }

    private void Log(string message)
    {
        GameLogger.Log(LogCategory.System, $"[RecipeTester] {message}");

        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    #endregion
}

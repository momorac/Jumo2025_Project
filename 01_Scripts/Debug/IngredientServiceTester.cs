using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

/// <summary>
/// 재료 시스템 테스트용 스크립트 (UniRx 기반)
/// Inspector에서 버튼 또는 키보드로 테스트 가능
/// </summary>
public class IngredientServiceTester : MonoBehaviour
{
    [Header("테스트 설정")]
    [SerializeField] private IngredientType testType = IngredientType.Rice;
    [SerializeField] private int testAmount = 10;

    [Header("UI 연결 (선택)")]
    [SerializeField] private Button addButton;
    [SerializeField] private Button consumeButton;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button unlockAllButton;
    [SerializeField] private Text resultText;

    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private void Start()
    {
        // UI 버튼 바인딩 (선택적)
        if (addButton != null)
        {
            addButton.OnClickAsObservable()
                .Subscribe(_ => TestAdd())
                .AddTo(disposables);
        }

        if (consumeButton != null)
        {
            consumeButton.OnClickAsObservable()
                .Subscribe(_ => TestConsume())
                .AddTo(disposables);
        }

        if (purchaseButton != null)
        {
            purchaseButton.OnClickAsObservable()
                .Subscribe(_ => TestPurchase())
                .AddTo(disposables);
        }

        if (unlockAllButton != null)
        {
            unlockAllButton.OnClickAsObservable()
                .Subscribe(_ => TestUnlockAll())
                .AddTo(disposables);
        }

        // 키보드 입력 바인딩
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F1))
            .Subscribe(_ => TestAdd())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F2))
            .Subscribe(_ => TestConsume())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F3))
            .Subscribe(_ => TestPurchase())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F4))
            .Subscribe(_ => TestUnlockAll())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F5))
            .Subscribe(_ => TestAddAllIngredients())
            .AddTo(disposables);

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F6))
            .Subscribe(_ => PrintAllInventory())
            .AddTo(disposables);

        Log("테스트 준비 완료\nF1: 추가 | F2: 소비 | F3: 구매 | F4: 전체해금 | F5: 전체추가 | F6: 인벤토리출력");
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }

    #region 테스트 메서드

    [ContextMenu("테스트: 재료 추가")]
    public void TestAdd()
    {
        if (!CheckService()) return;

        App.IngredientService.Add(testType, testAmount);
        int current = App.IngredientService.GetAmount(testType);
        Log($"[추가] {testType} +{testAmount} → 현재: {current}");
    }

    [ContextMenu("테스트: 재료 소비")]
    public void TestConsume()
    {
        if (!CheckService()) return;

        int before = App.IngredientService.GetAmount(testType);
        bool success = App.IngredientService.Consume(testType, testAmount);
        int after = App.IngredientService.GetAmount(testType);

        Log($"[소비] {testType} -{testAmount} | 결과: {(success ? "성공" : "실패")} | {before} → {after}");
    }

    [ContextMenu("테스트: 재료 구매")]
    public void TestPurchase()
    {
        if (!CheckService()) return;

        int goldBefore = App.EconomyService.GetMoney();
        bool success = App.IngredientService.Purchase(testType, testAmount);
        int goldAfter = App.EconomyService.GetMoney();
        int amount = App.IngredientService.GetAmount(testType);

        Log($"[구매] {testType} x{testAmount} | 결과: {(success ? "성공" : "실패")} | 골드: {goldBefore} → {goldAfter} | 보유량: {amount}");
    }

    [ContextMenu("테스트: 전체 재료 해금")]
    public void TestUnlockAll()
    {
        if (!CheckService()) return;

        int count = 0;
        foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
        {
            if (type != IngredientType.None && App.IngredientService.Unlock(type))
                count++;
        }
        Log($"[해금] {count}개 재료 해금 완료");
    }

    [ContextMenu("테스트: 전체 재료 추가")]
    public void TestAddAllIngredients()
    {
        if (!CheckService()) return;

        foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
        {
            if (type != IngredientType.None)
            {
                App.IngredientService.Add(type, 99);
            }
        }
        Log("[치트] 모든 재료 99개씩 추가 완료");
    }

    [ContextMenu("인벤토리 출력")]
    public void PrintAllInventory()
    {
        if (!CheckService()) return;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== 인벤토리 현황 ===");

        foreach (IngredientCategory category in Enum.GetValues(typeof(IngredientCategory)))
        {
            if (category == IngredientCategory.None) continue;

            sb.AppendLine($"\n[{category}]");
            foreach (IngredientType type in Enum.GetValues(typeof(IngredientType)))
            {
                if (type == IngredientType.None) continue;

                int amount = App.IngredientService.GetAmount(type);
                bool unlocked = App.IngredientService.IsUnlocked(type);

                if (amount > 0 || unlocked)
                {
                    string lockStatus = unlocked ? "" : " (잠김)";
                    sb.AppendLine($"  {type}: {amount}{lockStatus}");
                }
            }
        }

        Log(sb.ToString());
    }

    #endregion

    #region 헬퍼

    private bool CheckService()
    {
        if (App.IngredientService == null)
        {
            Log("<color=red>IngredientService가 초기화되지 않았습니다.</color>");
            return false;
        }
        return true;
    }

    private void Log(string message)
    {
        GameLogger.Log(LogCategory.System, $"[IngredientTester] {message}");

        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    #endregion
}

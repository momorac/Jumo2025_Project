#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// CSV 파일을 RecipeRegistry ScriptableObject로 변환하는 에디터 도구
/// </summary>
public class RecipeCSVImporter : EditorWindow
{
    private string csvFilePath = "";
    private RecipeRegistry targetRegistry;
    private string importLog = "";
    private Vector2 scrollPos;
    private int selectedEncoding = 0;
    private static readonly string[] EncodingOptions = { "UTF-8", "UTF-8 (BOM)", "EUC-KR (CP949)", "Unicode (UTF-16)" };
    private static readonly Encoding[] Encodings = {
        new UTF8Encoding(false),
        new UTF8Encoding(true),
        Encoding.GetEncoding(949),  // EUC-KR / CP949
        Encoding.Unicode
    };

    // 한글 재료명 → IngredientType 매핑
    private static readonly Dictionary<string, IngredientType> IngredientNameMap = new Dictionary<string, IngredientType>
    {
        // 곡식
        { "잡곡", IngredientType.MixedGrain },
        { "보리", IngredientType.Barley },
        { "쌀", IngredientType.Rice },
        { "찹쌀", IngredientType.SweetRice },

        // 채소
        { "배추", IngredientType.Cabbage },
        { "무", IngredientType.Radish },
        { "파", IngredientType.GreenOnion },
        { "부추", IngredientType.Chive },
        { "고추", IngredientType.Pepper },

        // 육류
        { "닭", IngredientType.Chicken },
        { "계란", IngredientType.Egg },
        { "생선", IngredientType.Fish },
        { "돼지", IngredientType.Pork },

        // 부재료
        { "두부", IngredientType.Tofu },
        { "밀가루", IngredientType.Flour },
        { "소금", IngredientType.Salt },
        { "장", IngredientType.Paste },

        // 중간재료 (김치)
        { "김치", IngredientType.KimchiCabbage },  // 기본 김치는 배추김치로
    };

    // 한글 레시피명 → RecipeType 매핑
    private static readonly Dictionary<string, RecipeType> RecipeNameMap = new Dictionary<string, RecipeType>
    {
        // 김치
        { "배추김치", RecipeType.CabbageKimchi },
        { "깍두기", RecipeType.Kkakdugi },
        { "파김치", RecipeType.ScallionKimchi },
        { "동치미", RecipeType.Dongchimi },
        { "나박김치", RecipeType.NabakKimchi },

        // 밥
        { "잡곡밥", RecipeType.MixedGrainRice },
        { "쌀밥", RecipeType.WhiteRice },
        { "찹쌀밥", RecipeType.SweetRice },
        { "보리밥", RecipeType.BarleyRice },

        // 국
        { "무국", RecipeType.RadishSoup },
        { "계란국", RecipeType.EggSoup },
        { "김치국", RecipeType.KimchiSoup },
        { "돼지고기국", RecipeType.PorkSoup },

        // 반찬
        { "무생채", RecipeType.RadishSalad },
        { "계란부침", RecipeType.FriedEgg },
        { "장조림", RecipeType.Jangjorim },
        { "부추무침", RecipeType.ChiveSalad },
        { "무조림", RecipeType.RadishBraised },

        // 국밥/찌개
        { "김치찌개", RecipeType.KimchiStew },
        { "된장찌개", RecipeType.SoybeanStew },
        { "돼지국밥", RecipeType.PorkRiceSoup },
        { "동태탕", RecipeType.DongtaeStew },
        { "육개장", RecipeType.Yukgaejang },
        { "닭백숙", RecipeType.ChickenSoup },
        { "장국밥", RecipeType.SoyPasteRiceSoup },
        { "북어국", RecipeType.DriedPollocSoup },
        { "갈비탕", RecipeType.GalbiSoup },
        { "순두부찌개", RecipeType.SoftTofuStew },

        // 전
        { "김치전", RecipeType.KimchiJeon },
        { "두부전", RecipeType.TofuJeon },
        { "동태전", RecipeType.FishJeon },
        { "육전", RecipeType.MeatJeon },
        { "파전", RecipeType.GreenOnionJeon },
        { "해물파전", RecipeType.SeafoodJeon },
        { "부추전", RecipeType.ChiveJeon },
        { "배추전", RecipeType.CabbageJeon },

        // 요리
        { "돼지구이", RecipeType.GrilledPork },
        { "수육", RecipeType.BoiledPork },
        { "갈비찜", RecipeType.GalbiJjim },
        { "불고기", RecipeType.Bulgogi },
        { "생선조림", RecipeType.BraisedFish },
        { "생선구이", RecipeType.GrilledFish },
        { "닭꼬치", RecipeType.ChickenSkewer },
        { "두부삼합", RecipeType.TofuTriple },
    };

    // 소분류 → RecipeSubCategory 매핑
    private static readonly Dictionary<string, RecipeSubCategory> SubCategoryMap = new Dictionary<string, RecipeSubCategory>
    {
        { "밥", RecipeSubCategory.Rice },
        { "국", RecipeSubCategory.Soup },
        { "반찬", RecipeSubCategory.SideDish },
        { "김치", RecipeSubCategory.Kimchi },
        { "국밥", RecipeSubCategory.StewBowl },
        { "국밥/찌개", RecipeSubCategory.StewBowl },
        { "찌개", RecipeSubCategory.StewBowl },
        { "전", RecipeSubCategory.Jeon },
        { "요리", RecipeSubCategory.Dish },
    };

    // 대분류 → RecipeCategory 매핑
    private static readonly Dictionary<string, RecipeCategory> CategoryMap = new Dictionary<string, RecipeCategory>
    {
        { "차림요리", RecipeCategory.TableDish },
        { "차림상", RecipeCategory.TableDish },
        { "단품요리", RecipeCategory.SingleDish },
        { "단품", RecipeCategory.SingleDish },
    };

    // 소분류 → CookingFacilityType 기본값 매핑
    private static readonly Dictionary<RecipeSubCategory, List<CookingFacilityType>> DefaultFacilitiesMap = new Dictionary<RecipeSubCategory, List<CookingFacilityType>>
    {
        { RecipeSubCategory.Rice,     new List<CookingFacilityType> { CookingFacilityType.Pot } },
        { RecipeSubCategory.Soup,     new List<CookingFacilityType> { CookingFacilityType.Pot } },
        { RecipeSubCategory.Kimchi,   new List<CookingFacilityType> { CookingFacilityType.JangdokJar } },
        { RecipeSubCategory.SideDish, new List<CookingFacilityType> { CookingFacilityType.Cauldron } },
        { RecipeSubCategory.StewBowl, new List<CookingFacilityType> { CookingFacilityType.Cauldron } },
        { RecipeSubCategory.Jeon,     new List<CookingFacilityType> { CookingFacilityType.Brazier } },
        { RecipeSubCategory.Dish,     new List<CookingFacilityType> { CookingFacilityType.Brazier } },
    };

    // 한글 조리시설명 → CookingFacilityType 매핑
    private static readonly Dictionary<string, CookingFacilityType> FacilityNameMap = new Dictionary<string, CookingFacilityType>
    {
        { "솥",    CookingFacilityType.Pot },
        { "가마솥", CookingFacilityType.Cauldron },
        { "화로",   CookingFacilityType.Brazier },
        { "장독대", CookingFacilityType.JangdokJar },
        { "Pot",        CookingFacilityType.Pot },
        { "Cauldron",   CookingFacilityType.Cauldron },
        { "Brazier",    CookingFacilityType.Brazier },
        { "JangdokJar", CookingFacilityType.JangdokJar },
    };

    // 한글 재료명 → IngredientType 매핑 (중간재료 포함 - outputIngredient용)
    private static readonly Dictionary<string, IngredientType> OutputIngredientMap = new Dictionary<string, IngredientType>
    {
        { "배추김치", IngredientType.KimchiCabbage },
        { "깍두기", IngredientType.KimchiRadish },
        { "파김치", IngredientType.KimchiGreenOnion },
        { "동치미", IngredientType.Dongchimi },
        { "나박김치", IngredientType.NabakKimchi },
        { "없음", IngredientType.None },
        { "None", IngredientType.None },
        { "", IngredientType.None },
    };

    // 김치 레시피 → 생성되는 IngredientType 매핑
    private static readonly Dictionary<RecipeType, IngredientType> KimchiOutputMap = new Dictionary<RecipeType, IngredientType>
    {
        { RecipeType.CabbageKimchi, IngredientType.KimchiCabbage },
        { RecipeType.Kkakdugi, IngredientType.KimchiRadish },
        { RecipeType.ScallionKimchi, IngredientType.KimchiGreenOnion },
        { RecipeType.Dongchimi, IngredientType.Dongchimi },
        { RecipeType.NabakKimchi, IngredientType.NabakKimchi },
    };

    [MenuItem("Tools/Recipe CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<RecipeCSVImporter>("Recipe CSV Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV → RecipeRegistry 변환 도구", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // CSV 파일 선택
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("CSV 파일", GUILayout.Width(80));
        csvFilePath = EditorGUILayout.TextField(csvFilePath);
        if (GUILayout.Button("찾아보기", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFilePanel("CSV 파일 선택", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                csvFilePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        // 인코딩 선택
        selectedEncoding = EditorGUILayout.Popup("인코딩", selectedEncoding, EncodingOptions);

        targetRegistry = (RecipeRegistry)EditorGUILayout.ObjectField("대상 Registry", targetRegistry, typeof(RecipeRegistry), false);

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "CSV 형식 (10개 컬럼):\n" +
            "대분류,소분류,이름,재료,조리시설,조리시간,가격,버퍼여부,버퍼수량,생성재료\n\n" +
            "재료 형식: \"재료명숫자, 재료명숫자\" (숫자 생략시 1개)\n" +
            "조리시설: 슬래시(/)로 여러 개 지정 가능\n" +
            "  예) 솥  /  솥/가마솥  /  화로/가마솥\n" +
            "  종류: 솥, 가마솥, 화로, 장독대\n\n" +
            "예시:\n" +
            "차림요리,김치,배추김치,\"고추1,배추2,소금1\",장독대,10,1000,O,10,배추김치\n" +
            "단품요리,국밥,돼지국밥,\"돼지2,무1,파1\",솥/가마솥,6,8000,X,1,없음\n\n" +
            "버퍼여부: O/X 또는 TRUE/FALSE\n" +
            "한글이 깨지면 인코딩을 EUC-KR로 변경해보세요.",
            MessageType.Info);

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(csvFilePath) || targetRegistry == null))
        {
            if (GUILayout.Button("CSV 가져오기", GUILayout.Height(30)))
            {
                ImportCSV();
            }
        }

        if (GUILayout.Button("새 Registry 생성", GUILayout.Height(25)))
        {
            CreateNewRegistry();
        }

        EditorGUILayout.Space();

        if (!string.IsNullOrEmpty(importLog))
        {
            GUILayout.Label("Import Log:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            EditorGUILayout.TextArea(importLog, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    private void ImportCSV()
    {
        importLog = "";
        var log = new System.Text.StringBuilder();

        try
        {
            // 선택된 인코딩으로 파일 읽기
            Encoding encoding = Encodings[selectedEncoding];
            log.AppendLine($"인코딩: {EncodingOptions[selectedEncoding]}");
            log.AppendLine($"파일: {csvFilePath}");
            log.AppendLine("");

            string fileContent = File.ReadAllText(csvFilePath, encoding);
            string[] lines = fileContent.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

            var recipes = new List<RecipeDefinition>();
            int successCount = 0;
            int failCount = 0;

            // 헤더 스킵
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var result = ParseCSVLine(line, i + 1, log);
                if (result != null)
                {
                    recipes.Add(result);
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }

            // SerializedObject를 통해 entries 필드에 접근
            SerializedObject serializedRegistry = new SerializedObject(targetRegistry);
            SerializedProperty entriesProperty = serializedRegistry.FindProperty("entries");

            // 기존 데이터 클리어 여부 확인
            if (entriesProperty.arraySize > 0)
            {
                if (EditorUtility.DisplayDialog("기존 데이터 처리",
                    $"Registry에 이미 {entriesProperty.arraySize}개의 레시피가 있습니다.\n\n기존 데이터를 어떻게 처리할까요?",
                    "덮어쓰기 (기존 삭제)", "추가 (기존 유지)"))
                {
                    entriesProperty.ClearArray();
                }
            }

            // 새 레시피 추가
            foreach (var recipe in recipes)
            {
                int index = entriesProperty.arraySize;
                entriesProperty.InsertArrayElementAtIndex(index);
                SerializedProperty element = entriesProperty.GetArrayElementAtIndex(index);

                // 각 필드 설정
                element.FindPropertyRelative("type").enumValueIndex = GetEnumIndex<RecipeType>(recipe.type);
                element.FindPropertyRelative("category").enumValueIndex = GetEnumIndex<RecipeCategory>(recipe.category);
                element.FindPropertyRelative("subCategory").enumValueIndex = GetEnumIndex<RecipeSubCategory>(recipe.subCategory);
                element.FindPropertyRelative("displayName").stringValue = recipe.displayName;
                // 조리시설 목록 설정
                SerializedProperty facilitiesProp = element.FindPropertyRelative("requiredFacilities");
                facilitiesProp.ClearArray();
                foreach (var facility in recipe.requiredFacilities)
                {
                    int fIdx = facilitiesProp.arraySize;
                    facilitiesProp.InsertArrayElementAtIndex(fIdx);
                    facilitiesProp.GetArrayElementAtIndex(fIdx).enumValueIndex = GetEnumIndex<CookingFacilityType>(facility);
                }
                element.FindPropertyRelative("cookingTime").floatValue = recipe.cookingTime;
                element.FindPropertyRelative("basePrice").intValue = recipe.basePrice;
                element.FindPropertyRelative("isBufferResource").boolValue = recipe.isBufferResource;
                element.FindPropertyRelative("bufferOutputAmount").intValue = recipe.bufferOutputAmount;
                element.FindPropertyRelative("outputIngredient").enumValueIndex = GetEnumIndex<IngredientType>(recipe.outputIngredient);

                // 재료 목록 설정
                SerializedProperty ingredientsProp = element.FindPropertyRelative("ingredients");
                ingredientsProp.ClearArray();
                foreach (var ing in recipe.ingredients)
                {
                    int ingIndex = ingredientsProp.arraySize;
                    ingredientsProp.InsertArrayElementAtIndex(ingIndex);
                    SerializedProperty ingElement = ingredientsProp.GetArrayElementAtIndex(ingIndex);
                    ingElement.FindPropertyRelative("ingredient").enumValueIndex = GetEnumIndex<IngredientType>(ing.ingredient);
                    ingElement.FindPropertyRelative("amount").intValue = ing.amount;
                }
            }

            serializedRegistry.ApplyModifiedProperties();
            EditorUtility.SetDirty(targetRegistry);
            AssetDatabase.SaveAssets();

            log.AppendLine($"\n=== 완료 ===");
            log.AppendLine($"성공: {successCount}개");
            log.AppendLine($"실패: {failCount}개");
            log.AppendLine($"총 레시피: {entriesProperty.arraySize}개");

            Debug.Log($"[RecipeCSVImporter] CSV Import complete: {successCount} success, {failCount} failed");
        }
        catch (System.Exception e)
        {
            log.AppendLine($"Error: {e.Message}");
            Debug.LogError($"CSV Import 실패: {e}");
        }

        importLog = log.ToString();
    }

    private RecipeDefinition ParseCSVLine(string line, int lineNum, System.Text.StringBuilder log)
    {
        // CSV 파싱 (따옴표 내 쉼표 처리)
        var columns = ParseCSVColumns(line);
        if (columns.Count < 10)
        {
            log.AppendLine($"[Line {lineNum}] 컬럼 부족 (10개 필요, {columns.Count}개 발견): {line}");
            return null;
        }

        string categoryStr = columns[0].Trim();
        string subCategoryStr = columns[1].Trim();
        string name = columns[2].Trim();
        string ingredientsStr = columns[3].Trim();
        string facilityStr = columns[4].Trim();
        string cookingTimeStr = columns[5].Trim();
        string priceStr = columns[6].Trim();
        string isBufferStr = columns[7].Trim();
        string bufferAmountStr = columns[8].Trim();
        string outputIngredientStr = columns[9].Trim();

        // RecipeType 매핑
        if (!RecipeNameMap.TryGetValue(name, out RecipeType recipeType))
        {
            log.AppendLine($"[Line {lineNum}] 알 수 없는 레시피: {name}");
            return null;
        }

        // Category 매핑
        if (!CategoryMap.TryGetValue(categoryStr, out RecipeCategory category))
        {
            log.AppendLine($"[Line {lineNum}] 알 수 없는 대분류: {categoryStr}");
            return null;
        }

        // SubCategory 매핑
        if (!SubCategoryMap.TryGetValue(subCategoryStr, out RecipeSubCategory subCategory))
        {
            log.AppendLine($"[Line {lineNum}] 알 수 없는 소분류: {subCategoryStr}");
            return null;
        }

        // 재료 파싱
        var ingredients = ParseIngredients(ingredientsStr, lineNum, log);

        // 조리시설 파싱 (슬래시로 구분해 여러 개 지정 가능)
        var facilities = ParseFacilities(facilityStr, subCategory, lineNum, log);

        // 조리시간 파싱
        float cookingTime = GetDefaultCookingTime(subCategory);
        if (float.TryParse(cookingTimeStr, out float parsedTime))
        {
            cookingTime = parsedTime;
        }

        // 가격 파싱
        int price = GetDefaultPrice(category, subCategory);
        if (int.TryParse(priceStr, out int parsedPrice))
        {
            price = parsedPrice;
        }

        // 버퍼 자원 여부 파싱
        bool isBuffer = ParseBool(isBufferStr);

        // 버퍼 수량 파싱
        int bufferAmount = 1;
        if (int.TryParse(bufferAmountStr, out int parsedAmount))
        {
            bufferAmount = parsedAmount;
        }

        // 생성 재료 파싱
        IngredientType outputIngredient = IngredientType.None;
        if (!string.IsNullOrEmpty(outputIngredientStr) && OutputIngredientMap.TryGetValue(outputIngredientStr, out var parsedOutput))
        {
            outputIngredient = parsedOutput;
        }

        // RecipeDefinition 생성
        var recipe = new RecipeDefinition
        {
            type = recipeType,
            category = category,
            subCategory = subCategory,
            displayName = name,
            ingredients = ingredients,
            requiredFacilities = facilities,
            cookingTime = cookingTime,
            basePrice = price,
            isBufferResource = isBuffer,
            bufferOutputAmount = bufferAmount,
            outputIngredient = outputIngredient
        };

        string facilitiesStr = string.Join("/", facilities);
        log.AppendLine($"[Line {lineNum}] ✓ {name} ({recipeType}) [{facilitiesStr}]");
        return recipe;
    }

    private List<CookingFacilityType> ParseFacilities(string facilityStr, RecipeSubCategory subCategory, int lineNum, System.Text.StringBuilder log)
    {
        var result = new List<CookingFacilityType>();

        if (!string.IsNullOrEmpty(facilityStr))
        {
            string[] parts = facilityStr.Split('/');
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (FacilityNameMap.TryGetValue(trimmed, out var facility))
                    result.Add(facility);
                else
                    log.AppendLine($"  [Line {lineNum}] 알 수 없는 조리시설: {trimmed}");
            }
        }

        // 파싱된 시설이 없으면 소분류 기본값 사용
        if (result.Count == 0)
        {
            if (DefaultFacilitiesMap.TryGetValue(subCategory, out var defaults))
                result.AddRange(defaults);
            else
                result.Add(CookingFacilityType.Pot);
        }

        return result;
    }

    private bool ParseBool(string value)
    {
        string v = value.Trim().ToUpperInvariant();
        return v == "O" || v == "TRUE" || v == "1" || v == "예" || v == "Y" || v == "YES";
    }

    private List<string> ParseCSVColumns(string line)
    {
        var columns = new List<string>();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                columns.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        columns.Add(current);

        return columns;
    }

    private List<RecipeIngredient> ParseIngredients(string ingredientsStr, int lineNum, System.Text.StringBuilder log)
    {
        var result = new List<RecipeIngredient>();

        // 따옴표 제거 및 분리
        ingredientsStr = ingredientsStr.Trim('"', ' ');
        string[] parts = ingredientsStr.Split(',');

        // 재료명 + 숫자 파싱 정규식 (예: "고추1", "돼지2", "배추")
        var regex = new Regex(@"^(.+?)(\d+)?$");

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            var match = regex.Match(trimmed);
            if (!match.Success)
            {
                log.AppendLine($"  [Line {lineNum}] 파싱 실패: {trimmed}");
                continue;
            }

            string ingredientName = match.Groups[1].Value.Trim();
            int amount = 1;
            if (match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value))
            {
                int.TryParse(match.Groups[2].Value, out amount);
            }

            if (IngredientNameMap.TryGetValue(ingredientName, out IngredientType ingredientType))
            {
                result.Add(new RecipeIngredient
                {
                    ingredient = ingredientType,
                    amount = amount
                });
            }
            else
            {
                log.AppendLine($"  [Line {lineNum}] 알 수 없는 재료: {ingredientName}");
            }
        }

        return result;
    }

    private float GetDefaultCookingTime(RecipeSubCategory subCategory)
    {
        return subCategory switch
        {
            RecipeSubCategory.Rice => 5f,
            RecipeSubCategory.Soup => 3f,
            RecipeSubCategory.Kimchi => 10f,
            RecipeSubCategory.SideDish => 4f,
            RecipeSubCategory.StewBowl => 6f,
            RecipeSubCategory.Jeon => 4f,
            RecipeSubCategory.Dish => 5f,
            _ => 3f
        };
    }

    private int GetDefaultPrice(RecipeCategory category, RecipeSubCategory subCategory)
    {
        if (category == RecipeCategory.SingleDish)
        {
            return subCategory switch
            {
                RecipeSubCategory.StewBowl => 8000,
                RecipeSubCategory.Jeon => 6000,
                RecipeSubCategory.Dish => 10000,
                _ => 5000
            };
        }
        return subCategory switch
        {
            RecipeSubCategory.Rice => 2000,
            RecipeSubCategory.Soup => 3000,
            RecipeSubCategory.Kimchi => 1000,
            RecipeSubCategory.SideDish => 2500,
            _ => 2000
        };
    }

    private int GetEnumIndex<T>(T value) where T : System.Enum
    {
        var values = System.Enum.GetValues(typeof(T));
        for (int i = 0; i < values.Length; i++)
        {
            if (values.GetValue(i).Equals(value))
                return i;
        }
        return 0;
    }

    private void CreateNewRegistry()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "새 Recipe Registry 생성",
            "RecipeRegistry",
            "asset",
            "RecipeRegistry 저장 위치 선택");

        if (!string.IsNullOrEmpty(path))
        {
            RecipeRegistry newRegistry = CreateInstance<RecipeRegistry>();
            AssetDatabase.CreateAsset(newRegistry, path);
            AssetDatabase.SaveAssets();
            targetRegistry = newRegistry;
            Debug.Log($"생성됨: {path}");
        }
    }
}
#endif

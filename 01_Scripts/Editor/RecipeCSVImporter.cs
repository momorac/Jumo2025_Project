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

    // 소분류 → CookingFacilityType 매핑
    private static readonly Dictionary<RecipeSubCategory, CookingFacilityType> FacilityMap = new Dictionary<RecipeSubCategory, CookingFacilityType>
    {
        { RecipeSubCategory.Rice, CookingFacilityType.Pot },
        { RecipeSubCategory.Soup, CookingFacilityType.Pot },
        { RecipeSubCategory.Kimchi, CookingFacilityType.JangdokJar },
        { RecipeSubCategory.SideDish, CookingFacilityType.Cauldron },
        { RecipeSubCategory.StewBowl, CookingFacilityType.Cauldron },
        { RecipeSubCategory.Jeon, CookingFacilityType.Brazier },
        { RecipeSubCategory.Dish, CookingFacilityType.Brazier },
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
            "CSV 형식:\n" +
            "대분류,소분류,이름,재료\n" +
            "차림요리,김치,배추김치,\"고추, 배추, 소금\"\n\n" +
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
                element.FindPropertyRelative("type").enumValueIndex = (int)recipe.type;
                element.FindPropertyRelative("category").enumValueIndex = (int)recipe.category;
                element.FindPropertyRelative("subCategory").enumValueIndex = GetEnumIndex<RecipeSubCategory>(recipe.subCategory);
                element.FindPropertyRelative("displayName").stringValue = recipe.displayName;
                element.FindPropertyRelative("requiredFacility").enumValueIndex = (int)recipe.requiredFacility;
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

            Debug.Log($"<color=green>CSV Import 완료: {successCount}개 성공, {failCount}개 실패</color>");
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
        if (columns.Count < 4)
        {
            log.AppendLine($"[Line {lineNum}] 컬럼 부족: {line}");
            return null;
        }

        string categoryStr = columns[0].Trim();
        string subCategoryStr = columns[1].Trim();
        string name = columns[2].Trim();
        string ingredientsStr = columns[3].Trim();

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

        // RecipeDefinition 생성
        var recipe = new RecipeDefinition
        {
            type = recipeType,
            category = category,
            subCategory = subCategory,
            displayName = name,
            ingredients = ingredients,
            requiredFacility = FacilityMap.GetValueOrDefault(subCategory, CookingFacilityType.Pot),
            cookingTime = GetDefaultCookingTime(subCategory),
            basePrice = GetDefaultPrice(category, subCategory),
            isBufferResource = subCategory == RecipeSubCategory.Rice || subCategory == RecipeSubCategory.Kimchi,
            bufferOutputAmount = subCategory == RecipeSubCategory.Rice ? 5 : (subCategory == RecipeSubCategory.Kimchi ? 10 : 1),
            outputIngredient = KimchiOutputMap.GetValueOrDefault(recipeType, IngredientType.None)
        };

        log.AppendLine($"[Line {lineNum}] ✓ {name} ({recipeType})");
        return recipe;
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

        foreach (string part in parts)
        {
            string ingredientName = part.Trim();
            if (string.IsNullOrEmpty(ingredientName)) continue;

            if (IngredientNameMap.TryGetValue(ingredientName, out IngredientType ingredientType))
            {
                result.Add(new RecipeIngredient
                {
                    ingredient = ingredientType,
                    amount = 1
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

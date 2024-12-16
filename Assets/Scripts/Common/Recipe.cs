using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Yogaewonsil.Common.CookMethod;
using static Yogaewonsil.Common.Food;

namespace Yogaewonsil.Common {
    public static class Recipe {

        // 레시피 관리하는 데이터베이스 입니다. 음식이 추가/수정될 경우 `recipeList`를 추가/수정하면 됩니다.
        // (CookMethod, [input Food1, input Food2, ...], output Food) 식으로 정의됩니다.
        // 참고: 레시피 구체화.xls from slack by 상언님
        // https://2024fallswppimo.slack.com/archives/C07J19PU63H/p1731530013617699
        private static readonly (CookMethod mthd, Food[] inFoods, Food outFood)[] recipeList = 
        {
            // 초밥류
            new(밥짓기, new[] { 쌀 }, 밥),
            // 계란초밥
            new(굽기, new[] { 계란 }, 계란말이),
            new(초밥제작, new[] { 계란말이, 식초, 밥 }, 계란초밥),
            // 유부초밥
            new(손질, new[] { 두부 }, 손질된두부),
            new(튀기기, new[] { 손질된두부 }, 유부),
            new(초밥제작, new[] { 유부, 식초, 밥 }, 유부초밥),
            // 광어초밥
            new(손질, new[] { 광어 }, 손질된광어),
            new(초밥제작, new[] { 손질된광어, 식초, 밥 }, 광어초밥),
            // 고등어초밥
            new(손질, new[] { 고등어 }, 손질된고등어),
            new(초밥제작, new[] { 손질된고등어, 식초, 밥 }, 고등어초밥),
            // 소고기초밥
            new(손질, new[] { 소고기 }, 손질된소고기),
            new(손질, new[] { 손질된소고기 }, 소고기조각),
            new(굽기, new[] { 소고기조각 }, 구운소고기조각),
            new(초밥제작, new[] { 구운소고기조각, 식초, 밥 }, 소고기초밥),
            // 참치붉은속살초밥 (일반참치)
            new(손질, new[] { 일반참치 }, 손질된일반참치),
            new(초밥제작, new[] { 손질된일반참치, 식초, 밥 }, 참치붉은속살초밥),
            // 연어초밥 (연어->손질된연어->연어조각->연어초밥)
            new(손질, new[] { 연어 }, 손질된연어),
            new(손질, new[] { 손질된연어 }, 연어조각),
            new(초밥제작, new[] { 연어조각, 식초, 밥 }, 연어초밥),
            // 훈제연어초밥 (연어->손질된->연어조각->구운연어조각->훈제연어초밥)
            new(손질, new[] { 연어 }, 손질된연어),
            new(손질, new[] { 손질된연어 }, 연어조각),
            new(굽기, new[] { 연어조각 }, 구운연어조각),
            new(초밥제작, new[] { 구운연어조각, 식초, 밥 }, 훈제연어초밥),
            // 장어초밥 (장어->손질된장어->구운장어조각->장어초밥)
            new(손질, new[] { 장어 }, 손질된장어),
            new(굽기, new[] { 손질된장어 }, 구운장어조각),
            new(초밥제작, new[] { 구운장어조각, 식초, 밥 }, 장어초밥),
            // 참치대뱃살초밥 (고급참치)
            new(손질, new[] { 고급참치 }, 손질된고급참치),
            new(초밥제작, new[] { 손질된고급참치, 식초, 밥 }, 참치대뱃살초밥),
            // 새우초밥
            new(굽기, new[] { 손질된새우 }, 구운손질된새우),
            new(초밥제작, new[] { 구운손질된새우, 식초, 밥 }, 새우초밥),

            // 라멘류
            // 라멘육수 (물, 돼지고기, 채소)
            new(끓이기, new[] { 물, 돼지고기, 채소 }, 라멘육수),
            // 소유라멘 (라멘육수, 생면, 간장, 계란)
            new(끓이기, new[] { 라멘육수, 생면, 간장, 계란 }, 소유라멘),
            // 미소라멘 (라멘육수, 미소, 생면, 계란)
            new(끓이기, new[] { 라멘육수, 미소, 생면, 계란 }, 미소라멘),
            // 아부라라멘 (면->삶은면, 돼지고기->손질된돼지고기->구운손질된돼지고기)
            // (삶은면, 구운손질된돼지고기, 간장, 계란)
            new(끓이기, new[] { 생면 }, 삶은면),
            new(손질, new[] { 돼지고기 }, 손질된돼지고기),
            new(굽기, new[] { 손질된돼지고기 }, 구운손질된돼지고기),
            new(비가열조리, new[] { 삶은면, 구운손질된돼지고기, 간장, 계란 }, 아부라라멘),
            // 돈코츠라멘 (라멘육수, 생면, 차슈 계란)
            new(끓이기, new[] { 라멘육수, 생면, 차슈, 계란 }, 돈코츠라멘),
            // 츠케멘 (라멘육수, 삶은면, 구운손질된돼지고기, 계란)
            new(비가열조리, new[] { 라멘육수, 삶은면, 구운손질된돼지고기, 계란 }, 츠케멘),

            // 튀김류
            // 새우튀김 ((새우->손질된새우), 튀김가루)
            new(손질, new[] { 새우 }, 손질된새우),
            new(튀기기, new[] { 손질된새우, 튀김가루 }, 새우튀김),
            // 돈가스 (돼지고기->손질된돼지고기->돈가스조각, 밥)
            new(손질, new[] { 돼지고기 }, 손질된돼지고기),
            new(튀기기, new[] { 손질된돼지고기 }, 돈가스조각),
            new(비가열조리, new[] { 돈가스조각, 밥 }, 돈가스),

            // 구이류
            // 일본식함박스테이크 (채소->손질된채소, 손질된돼지고기, 계란)
            new(손질, new[] { 채소 }, 손질된채소),
            new(굽기, new[] { 손질된채소, 손질된돼지고기, 계란 }, 일본식함박스테이크),
            // 차슈 (손질된돼지곡, 간장, 물)
            new(굽기, new[] { 손질된돼지고기, 간장, 물 }, 차슈),
            // 연어스테이크 굽기(손질된연어, 손질된채소)
            new(굽기, new[] { 손질된연어, 손질된채소 }, 연어스테이크),
            // 와규스테이크 굽기(손질된소고기, 손질된채소)
            new(굽기, new[] { 손질된소고기, 손질된채소 }, 와규스테이크),

            // 덮밥류
            // 오니기리 비가열조리(밥, (손질된광어 or 손질된고등어 or ... 생선류))
            new(비가열조리, new[] { 밥, 손질된광어 }, 오니기리),
            new(비가열조리, new[] { 밥, 손질된고등어 }, 오니기리),
            new(비가열조리, new[] { 밥, 연어조각 }, 오니기리),
            new(비가열조리, new[] { 밥, 손질된연어 }, 오니기리),
            new(비가열조리, new[] { 밥, 손질된일반참치 }, 오니기리),
            new(비가열조리, new[] { 밥, 손질된장어 }, 오니기리),
            new(비가열조리, new[] { 밥, 손질된고급참치 }, 오니기리),
            // 에비텐동 비가열조리(밥, 새우튀김 간장)
            new(비가열조리, new[] { 밥, 새우튀김, 간장 }, 에비텐동),
            // 사바동 비가열조리(굽기(손질된고등어, 손질된채소)->구운고등어, 밥)
            new(굽기, new[] { 손질된고등어, 손질된채소 }, 구운고등어),
            new(비가열조리, new[] { 구운고등어, 밥 }, 사바동),
            // 규동 비가열조리(굽기(손질(손질된소고기)->소고기조각)->구운소고기조각), 손질된채소, 계란, 밥)
            new(손질, new[] { 손질된소고기 }, 소고기조각),
            new(굽기, new[] { 소고기조각, 손질된채소 }, 구운소고기와채소),
            new(비가열조리, new[] { 구운소고기와채소, 계란, 밥 }, 규동),
            // 사케동 (연어조각, 간장, 손질된채소, 밥)
            new(비가열조리, new[] { 연어조각, 간장, 손질된채소, 밥 }, 사케동),
            // 차슈동 (차슈, 밥, 손질된채소)
            new(비가열조리, new[] { 차슈, 밥, 손질된채소 }, 차슈동),
            // 연어스테이크덮밥 (연어스테이크, 밥)
            new(비가열조리, new[] { 연어스테이크, 밥 }, 연어스테이크덮밥),
            // 우나기동 (구운장어조각, 밥)
            new(비가열조리, new[] { 구운장어조각, 밥 }, 우나기동),

            // 미소국 끓이기(손질(두부)->손질된두부, 미소, 물)
            new(손질, new[] { 두부 }, 손질된두부),
            new(끓이기, new[] { 손질된두부, 미소, 물 }, 미소국),
            // 차 (물, 채소)
            new(끓이기, new[] { 물, 채소 }, 차),
        };

        // 위의 `recipeList`를 바탕으로 `recipe`를 초기화합니다.
        // 실질적으로 `recipe`에서 레시피를 관리합니다.
        private static readonly Dictionary<CookMethod, Dictionary<ValHashSet<Food>, Food>> recipe = InitializeRecipe();
        private static Dictionary<CookMethod, Dictionary<ValHashSet<Food>, Food>> InitializeRecipe()
        {
            // Uniquefy the recipeList
            var uniqueRecipeList = recipeList
            // Group by CookMethod and input
                .GroupBy(tuple => 
                    (tuple.mthd, new ValHashSet<Food>(tuple.inFoods))
                )
                .Select(group => group.First())
                .ToArray();
            
            // Create and return the dictionary
            return uniqueRecipeList
                .GroupBy(tuple => tuple.mthd)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToDictionary(
                        tuple => new ValHashSet<Food>(tuple.inFoods),
                        tuple => tuple.outFood
                    )
                );
        }

        // 레시피에 존재하는 조리법인지 확인합니다.
        private static bool IsValid(CookMethod cookMethod, ValHashSet<Food> input) {

            return recipe.ContainsKey(cookMethod) && recipe[cookMethod].ContainsKey(input);
        }

        public static bool IsValid(CookMethod cookMethod, IEnumerable<Food> input) {
            return IsValid(cookMethod, new ValHashSet<Food>(input));
        }
        
        // 레시피에 존재하는 조리법을 실행합니다.
        // 조리법이 존재하지 않는다면 `실패요리`를 반환합니다. 
        // 조리법이 존재하면 해당 조리법을 실행한 결과값 (Food) 를 반환합니다.
        private static Food Execute(CookMethod cookMethod, ValHashSet<Food> input) {
            if (!IsValid(cookMethod, input)) {
                return 실패요리;
            }
            return recipe[cookMethod][input];
        }

        public static Food Execute(CookMethod cookMethod, IEnumerable<Food> input) {
            return Execute(cookMethod, new ValHashSet<Food>(input));
        }

        public static IEnumerable<(CookMethod mthd, Food[] inFoods, Food outFood)> GetRecipesForOutput(Food output)
        {
            return recipeList.Where(recipe => recipe.outFood == output);
        }

        // 값을 기준으로 비교할 수 있는 HashSet입니다.
        // Input Food를 비교하기 위해 사용하는 클래스 입니다.
        private class ValHashSet<T> : IEquatable<ValHashSet<T>>, IEnumerable<T> {
            public ValHashSet(HashSet<T> hashSet) {
                this.hashSet = hashSet;
            }

            public ValHashSet(IEnumerable<T> enumerable) {
                hashSet = new HashSet<T>(enumerable);
            }

            public bool Equals(ValHashSet<T> other) {
                if (other == null) {
                    return false;
                }
                return hashSet.SetEquals(other.hashSet);
            }

            public override int GetHashCode() {
                return hashSet.Aggregate(0, (acc, val) => acc ^ val.GetHashCode());
            }

            public IEnumerator<T> GetEnumerator()
            {
                return hashSet.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private readonly HashSet<T> hashSet;
        }
    }  
}

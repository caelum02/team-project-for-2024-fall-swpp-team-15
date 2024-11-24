using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Yogaewonsil.Common;
using static Yogaewonsil.Common.Food;
using static Yogaewonsil.Common.CookMethod;


public class RecipeTest
{

    [Test]
    public void RecipeValidTest()
    {
        // 초밥류 테스트
        Assert.IsTrue(Recipe.IsValid(밥짓기, new[] { 쌀 }));
        Assert.AreEqual(밥, Recipe.Execute(밥짓기, new[] { 쌀 }));

        Assert.IsTrue(Recipe.IsValid(합치기, new[] { 식초, 밥 }));
        Assert.AreEqual(식초밥, Recipe.Execute(합치기, new[] { 식초, 밥 }));

        Assert.IsTrue(Recipe.IsValid(굽기, new[] { 계란 }));
        Assert.AreEqual(계란말이, Recipe.Execute(굽기, new[] { 계란 }));

        Assert.IsTrue(Recipe.IsValid(초밥제작, new[] { 계란말이, 식초밥 }));
        Assert.AreEqual(계란초밥, Recipe.Execute(초밥제작, new[] { 계란말이, 식초밥 }));

        // 라멘류 테스트 (소유라멘, 미소라멘, 아부라라멘, 츠케멘)
        Assert.IsTrue(Recipe.IsValid(끓이기, new[] { 물, 돼지고기, 채소 }));
        Assert.AreEqual(라멘육수, Recipe.Execute(끓이기, new[] { 물, 돼지고기, 채소 }));

        Assert.IsTrue(Recipe.IsValid(끓이기, new[] { 라멘육수, 생면, 간장, 계란 }));
        Assert.AreEqual(소유라멘, Recipe.Execute(끓이기, new[] { 라멘육수, 생면, 간장, 계란 }));

        Assert.IsTrue(Recipe.IsValid(끓이기, new[] { 라멘육수, 미소, 생면, 계란 }));
        Assert.AreEqual(미소라멘, Recipe.Execute(끓이기, new[] { 라멘육수, 미소, 생면, 계란 }));

        // 츠케멘  
        Assert.IsTrue(Recipe.IsValid(비가열조리, new[] { 라멘육수, 삶은면, 구운손질된돼지고기, 계란 }));
        Assert.AreEqual(츠케멘, Recipe.Execute(비가열조리, new[] { 라멘육수, 삶은면, 구운손질된돼지고기, 계란 }));

        // 아부라라멘
        Assert.IsTrue(Recipe.IsValid(비가열조리, new[] { 구운손질된돼지고기, 삶은면, 간장, 계란 }));
        Assert.AreEqual(아부라라멘, Recipe.Execute(비가열조리, new[] { 구운손질된돼지고기, 삶은면, 간장, 계란 }));

        // 튀김류 테스트
        Assert.IsTrue(Recipe.IsValid(손질, new[] { 새우 }));
        Assert.AreEqual(손질된새우, Recipe.Execute(손질, new[] { 새우 }));

        Assert.IsTrue(Recipe.IsValid(튀기기, new[] { 손질된새우, 튀김가루 }));
        Assert.AreEqual(새우튀김, Recipe.Execute(튀기기, new[] { 손질된새우, 튀김가루 }));

        // 구이류 테스트
        Assert.IsTrue(Recipe.IsValid(굽기, new[] { 연어필렛, 손질된채소 }));
        Assert.AreEqual(연어스테이크, Recipe.Execute(굽기, new[] { 연어필렛, 손질된채소 }));

        Assert.IsTrue(Recipe.IsValid(굽기, new[] { 손질된소고기, 손질된채소 }));
        Assert.AreEqual(와규스테이크, Recipe.Execute(굽기, new[] { 손질된소고기, 손질된채소 }));

        // 덮밥류 테스트
        Assert.IsTrue(Recipe.IsValid(비가열조리, new[] { 밥, 손질된광어 }));
        Assert.AreEqual(오니기리, Recipe.Execute(비가열조리, new[] { 밥, 손질된광어 }));

        Assert.IsTrue(Recipe.IsValid(비가열조리, new[] { 밥, 새우튀김, 간장 }));
        Assert.AreEqual(에비텐동, Recipe.Execute(비가열조리, new[] { 밥, 새우튀김, 간장 }));

        // 미소국 테스트
        Assert.IsTrue(Recipe.IsValid(끓이기, new[] { 손질된두부, 미소, 물 }));
        Assert.AreEqual(미소국, Recipe.Execute(끓이기, new[] { 손질된두부, 미소, 물 }));

        // 차 테스트
        Assert.IsTrue(Recipe.IsValid(끓이기, new[] { 물, 채소 }));
        Assert.AreEqual(차, Recipe.Execute(끓이기, new[] { 물, 채소 }));
    }


    [Test]
    public void RecipeInvalidTest()
    {
        // Invalid 초밥류 테스트
        Assert.IsFalse(Recipe.IsValid(밥짓기, new[] { 식초 }));
        Assert.AreEqual(실패요리, Recipe.Execute(밥짓기, new[] { 식초 }));

        Assert.IsFalse(Recipe.IsValid(합치기, new[] { 쌀, 밥 }));
        Assert.AreEqual(실패요리, Recipe.Execute(합치기, new[] { 쌀, 밥 }));

        Assert.IsFalse(Recipe.IsValid(굽기, new[] { 식초 }));
        Assert.AreEqual(실패요리, Recipe.Execute(굽기, new[] { 식초 }));

        Assert.IsFalse(Recipe.IsValid(초밥제작, new[] { 계란말이, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(초밥제작, new[] { 계란말이, 쌀 }));

        // Invalid 라멘류 테스트
        Assert.IsFalse(Recipe.IsValid(끓이기, new[] { 물, 돼지고기, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(끓이기, new[] { 물, 돼지고기, 쌀 }));

        Assert.IsFalse(Recipe.IsValid(끓이기, new[] { 라멘육수, 생면, 간장, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(끓이기, new[] { 라멘육수, 생면, 간장, 쌀 }));

        Assert.IsFalse(Recipe.IsValid(끓이기, new[] { 라멘육수, 미소, 쌀, 계란 }));
        Assert.AreEqual(실패요리, Recipe.Execute(끓이기, new[] { 라멘육수, 미소, 쌀, 계란 }));

        // Invalid 튀김류 테스트
        Assert.IsFalse(Recipe.IsValid(손질, new[] { 밥 }));
        Assert.AreEqual(실패요리, Recipe.Execute(손질, new[] { 밥 }));

        Assert.IsFalse(Recipe.IsValid(튀기기, new[] { 손질된새우, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(튀기기, new[] { 손질된새우, 쌀 }));

        // Invalid 구이류 테스트
        Assert.IsFalse(Recipe.IsValid(굽기, new[] { 연어필렛, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(굽기, new[] { 연어필렛, 쌀 }));

        Assert.IsFalse(Recipe.IsValid(굽기, new[] { 손질된소고기, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(굽기, new[] { 손질된소고기, 쌀 }));

        // Invalid 덮밥류 테스트
        Assert.IsFalse(Recipe.IsValid(비가열조리, new[] { 밥, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(비가열조리, new[] { 밥, 쌀 }));

        Assert.IsFalse(Recipe.IsValid(비가열조리, new[] { 밥, 새우튀김, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(비가열조리, new[] { 밥, 새우튀김, 쌀 }));

        // Invalid 미소국 테스트
        Assert.IsFalse(Recipe.IsValid(끓이기, new[] { 손질된두부, 미소, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(끓이기, new[] { 손질된두부, 미소, 쌀 }));

        // Invalid 차 테스트
        Assert.IsFalse(Recipe.IsValid(끓이기, new[] { 물, 쌀 }));
        Assert.AreEqual(실패요리, Recipe.Execute(끓이기, new[] { 물, 쌀 }));
    }
}

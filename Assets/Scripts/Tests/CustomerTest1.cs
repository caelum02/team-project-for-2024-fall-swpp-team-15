// using NUnit.Framework;
// using UnityEngine;
// using Yogaewonsil.Common;

// public class CustomerTest1
// {
//     private GameObject customerManagerObj;
//     private CustomerManager customerManager;
//     private Table table;

//     [SetUp]
//     public void Setup()
//     {
//         customerManagerObj = new GameObject();
//         customerManager = customerManagerObj.AddComponent<CustomerManager>();
//         table = new GameObject().AddComponent<Table>();
//         table.Occupy();
//     }

//     /// <summary>
//     /// CustomerBase의 테이블 도착 및 착석 테스트
//     /// </summary>
//     [Test]
//     public void CustomerBase_ShouldAssignTable_WhenAvailable()
//     {
//         var customer = new GameObject().AddComponent<NormalCustomer>();
//         customerManager.tables.Add(table);

//         customer.Start();
//         Assert.IsTrue(table.isOccupied, "Table should be occupied.");
//     }

//     /// <summary>
//     /// 인내심 감소 확인 테스트
//     /// </summary>
//     [Test]
//     public void CustomerBase_ShouldDecreasePatience_WhenWaiting()
//     {
//         var customer = new GameObject().AddComponent<NormalCustomer>();
//         float initialPatience = 90f;

//         customer.Start();
//         customer.Update(); // Time.deltaTime 모의 실행
//         Assert.Less(customer.patience, initialPatience, "Patience should decrease over time.");
//     }

//     /// <summary>
//     /// 진상손님의 실패요리 주문 테스트
//     /// </summary>
//     [Test]
//     public void BadGuyCustomer_ShouldMakeFailDishOrder()
//     {
//         var badGuy = new GameObject().AddComponent<BadGuyCustomer>();
//         FoodData result = badGuy.MakeOrder();

//         Assert.AreEqual(Food.실패요리, result.food, "BadGuyCustomer should order 실패요리.");
//     }

//     /// <summary>
//     /// GourmetCustomer 보상 확인 테스트
//     /// </summary>
//     [Test]
//     public void GourmetCustomer_ShouldGiveDoubleReward_WhenSuccess()
//     {
//         var gourmetCustomer = new GameObject().AddComponent<GourmetCustomer>();
//         gourmetCustomer.requiredDish = new FoodData { price = 100, level = 1 };
//         gourmetCustomer.isSuccessTreatment = true;

//         customerManager.UpdateGameStats(200, 50);
//         Assert.AreEqual(200, customerManager.gameManager.money, "GourmetCustomer should give double rewards.");
//     }

//     /// <summary>
//     /// MichelinCustomer 보상 및 별 획득 확인 테스트
//     /// </summary>
//     [Test]
//     public void MichelinCustomer_ShouldGiveRewardAndMichelinStar_WhenSuccess()
//     {
//         var michelinCustomer = new GameObject().AddComponent<MichelinCustomer>();
//         michelinCustomer.requiredDish = new FoodData { price = 100, level = 1 };
//         michelinCustomer.isSuccessTreatment = true;

//         michelinCustomer.PayMoneyAndReputation();

//         // 가짜 GameManager에서 별 획득 여부 확인
//         Assert.IsTrue(customerManager.gameManager.hasMichelinStar, "MichelinCustomer should trigger GetMichelinStar().");
//     }

//     /// <summary>
//     /// Table 음식 배치 테스트
//     /// </summary>
//     [Test]
//     public void Table_ShouldPlaceFood_WhenDishProvided()
//     {
//         var table = new GameObject().AddComponent<Table>();
//         PlayerController.Instance = new GameObject().AddComponent<PlayerController>();
//         PlayerController.Instance.heldFood = Food.빵;

//         table.PutDish();
//         Assert.IsNotNull(table.currentPlateObject, "Food should be placed on the table.");
//     }
// }

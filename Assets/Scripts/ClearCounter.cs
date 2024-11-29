// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ClearCounter : MonoBehaviour, IFoodObjectParent
// {
//     [SerializeField] private FoodObjectSO foodObjectSO;
//     [SerializeField] private Transform counterTopPoint;

//     private FoodObject foodObject;




//     public void Interact(PlayerController player){
//         Debug.Log("Interact");
//         Debug.Log(foodObject);
//         if(HasFoodObject()){
//             // Counter위에 FoodObject가 있음
//             if(player.HasFoodObject()){
//                 // Player가 FoodObject를 잡고 있음
//                 Debug.Log("Player can't hold object. Already holding Food Object");
//             } 
//             else {
//                 // Player가 FoodObject를 잡고 있지 않음
//                 GetFoodObject().SetFoodObjectParent(player); // Counter의 FoodObject를 Player에게 넘겨준다
//             }
//         }
//         else {
//             // Counter위에 FoodObject가 없음
//             if(player.HasFoodObject()){
//                 player.GetFoodObject().SetFoodObjectParent(this); // Player의 FoodObject를 Counter에게 넘겨준다
//             } 
//             else {
//                 // Player가 FoodObject를 잡고 있지 않음
//             }
//         }
        
//         // if(foodObject == null){
//         //     Transform foodObjectTransform = Instantiate(foodObjectSO.prefab, counterTopPoint); //위에 음식이 없으면 음식을 생성하고 더 생성 못하게 만든다
//         //     foodObjectTransform.GetComponent<FoodObject>().SetFoodObjectParent(this);
//         // } else {
//         //     foodObject.SetFoodObjectParent(player);
//         // }
//     }

//     public Transform GetFoodObjectFollowTransform() {
//         return counterTopPoint;
//     }

//     public void SetFoodObject(FoodObject foodObject){
//         this.foodObject = foodObject;
//     }

//     public FoodObject GetFoodObject() {
//         return foodObject;
//     }

//     public void ClearFoodObject() {
//         foodObject = null;
//     }

//     public bool HasFoodObject() {
//         return foodObject != null;
//     }
// }

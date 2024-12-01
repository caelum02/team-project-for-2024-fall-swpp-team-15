// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SelectedCounterVisual : MonoBehaviour
// {
//     [SerializeField] private ClearCounter clearCounter;
//     [SerializeField] private GameObject visualGameObject;

//     private void Start() {
//         //PlayerController.Instance.OnSelectedCounterChanged += PlayerController_OnSelectedCounterChanged;
//     }

//     private void PlayerController_OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e)
//     {
//         if(e.selectedCounter == clearCounter) {
//             Show();
//         }
//         else {
//             Hide();
//         }

//     }

//     private void Show() {
//         visualGameObject.SetActive(true);
//         Debug.Log("Showing Visual");
//         // 아직 미완성, 나중에 필요할 때 다시 올 예정
//     }

//     private void Hide() {
//         visualGameObject.SetActive(false);
//         // 아직 미완성, 나중에 필요할 때 다시 올 예정
//     }
// }

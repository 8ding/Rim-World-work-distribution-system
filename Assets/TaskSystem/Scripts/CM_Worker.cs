// /* 
//     ------------------- Code Monkey -------------------
//
//     Thank you for downloading this Code Monkey project
//     I hope you find it useful in your own projects
//     If you have any questions let me know
//     Cheers!
//
//                unitycodemonkey.com
//     --------------------------------------------------
//  */
//  
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
//
// namespace CM_TaskSystem {
//
//     public class CM_Worker : CM_IWorker {
//
//         private static GridPathfindingSystem.GridPathfinding pathfinding;
//
//         public GameObject gameObject;
//         private V_Object unitObject;
//
//         public static CM_Worker Create(Vector3 position) {
//             if (pathfinding == null) {
//                 float nodeSize = 5f;
//                 Vector3 pathfindingOriginRoam = new Vector3(200, 200);
//                 Vector3 pathfindingUpperRight = new Vector3(1000, 1000);
//                 pathfinding = new GridPathfindingSystem.GridPathfinding(pathfindingOriginRoam, pathfindingUpperRight, nodeSize);
//             }
//
//             return new CM_Worker(position);
//         }
//
//         private CM_Worker(Vector3 position) {
//             unitObject = V_GameHandlerLogic_BattleRoyaleTycoon.CreateBasicUnit(position, 30f, SpriteHolder.instance.m_PeasantSpriteSheet, UnitAnimTypeEnum.dBareHands_Walk, UnitAnimTypeEnum.dBareHands_Idle);
//             
//             V_ObjectLogic_PathfindingHandler pathfindingHandler = unitObject.GetLogic<V_ObjectLogic_PathfindingHandler>();
//             pathfindingHandler.SetActivePathfinding(pathfinding);
//
//             gameObject = unitObject.GetLogic<V_IObjectTransform>().GetTransform().gameObject;
//         }
//
//         public void MoveTo(Vector3 position, Action onArrivedAtPosition = null) {
//             unitObject.GetLogic<V_ObjectWalkerAnimated>().MoveTo(position, onArrivedAtPosition);
//         }
//
//         public void PlayAnimationToilet(Action onAnimComplete) {
//             float framerate = UnityEngine.Random.Range(.3f, .7f);
//             unitObject.GetLogic<V_UnitAnimation>().PlayAnimForced(BattleRoyaleTycoon.UnitAnimEnum.dBareHands_Victory, framerate, onAnimComplete);
//         }
//
//         public bool IsMoving() {
//             return unitObject.GetLogic<V_ObjectWalkerAnimated>().IsMoving();
//         }
//
//         public Vector3 GetPosition() {
//             return unitObject.GetPosition();
//         }
//
//     }
//
// }
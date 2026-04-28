using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    [RequireComponent(typeof(AICharacterController))]
    public class AIBootstrap_Old : MonoBehaviour {
        [SerializeField] private Transform targetTemp;
        [Space(10f)]
        [SerializeField] private AIPathConfigAsset pathConfig;

        private AICharacterController controller;

        private void Awake() {
            controller = GetComponent<AICharacterController>();
            var path = new AIPath(pathConfig.Config);

            controller.Install(targetTemp, path);
        }

        private void Start() {
            controller.Init();
        }
    }
}
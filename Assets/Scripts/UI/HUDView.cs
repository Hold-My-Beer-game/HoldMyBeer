using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Single deterministic UI binding layer.
    /// Owns all UI state updates.
    /// </summary>
    internal class HUDView : MonoBehaviour {
        [SerializeField] private UIDocument document;
        
        private HUDState state;
        private bool initialized;
        
        private VisualElement root;

        // UI references cache
        private VisualElement bottle;
        private VisualElement blood;
        private Label goalLabel;
        private Label interactLabel;
        private VisualElement ammoContainer;

        // Ammo cache
        private readonly List<VisualElement> bullets = new();
        
        private const float FLICKER_THRESHOLD = 0.25f;
        private const float FLICKER_FREQ = 10f;
        private const float FLICKER_AMP = 0.2f;

        public void Bind(HUDState newState) {
            if (initialized) { return; }
            initialized = true;
            
            state = newState;
            root = document.rootVisualElement;
            VisualElement hudRoot = root.Q<VisualElement>("HUD");
            if (hudRoot == null) { throw new Exception("HUDRoot is null"); }

            // Cache UI elements once 
            bottle = hudRoot.Q<VisualElement>("Bottle")?? throw new NullReferenceException("Bottle");
            blood = hudRoot.Q<VisualElement>("Blood")?? throw new NullReferenceException("Blood");
            goalLabel = hudRoot.Q<Label>("GoalLabel")?? throw new NullReferenceException("GoalLabel");
            interactLabel = hudRoot.Q<Label>("InteractLabel")?? throw new NullReferenceException("InteractLabel");
            ammoContainer = hudRoot.Q<VisualElement>("AmmoContainer")?? throw new NullReferenceException("AmmoContainer");

            BuildAmmoVisuals();

            state.OnChanged += Refresh;
            Refresh();
        }

        private void OnDisable() {
            if (state != null) { state.OnChanged -= Refresh; }
        }

        private void Update() {
            UpdateVitals();
        }

        // ---------------------
        // FULL UI UPDATE ENTRY
        // ---------------------
        private void Refresh() {
            UpdateText();
            UpdateInteract();
            UpdateAmmo();
        }

        // -------------
        // VISUAL SYSTEM
        // -------------
        private void UpdateVitals() {
            // todo: flicker isn't working
            float flicker = 1f;
            if (state.Drunkness < FLICKER_THRESHOLD) { flicker = 1f + Mathf.Sin(Time.time * FLICKER_FREQ) * FLICKER_AMP; }

            bottle.style.opacity = state.Drunkness * flicker;
            blood.style.opacity = 1f - state.Health;
        }

        // -----------
        // TEXT SYSTEM
        // -----------
        private void UpdateText() {
            goalLabel.text = state.GoalText;
        }

        private void UpdateInteract() {
            interactLabel.text = state.InteractText;
            interactLabel.style.display = state.InteractVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // -------------------------
        // AMMO SYSTEM (STATIC POOL)
        // -------------------------
        private void BuildAmmoVisuals() {
            ammoContainer.Clear();
            bullets.Clear();

            for (int i = 0; i < state.Ammo; i++) {
                VisualElement bullet = new VisualElement();
                bullet.AddToClassList("bullet");

                bullets.Add(bullet);
                ammoContainer.Add(bullet);
            }
        }

        private void UpdateAmmo() {
            // rebuild only if size changes
            if (bullets.Count != state.Ammo) { BuildAmmoVisuals(); }

            for (int i = 0; i < bullets.Count; i++) {
                bool empty = i >= state.Ammo;
                bullets[i].EnableInClassList("empty", empty);
            }
        }
    }
}
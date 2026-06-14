using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace HoldMyBeer.UI {
    [System.Serializable]
    public struct ScreenRef {
        public UIScreen id;
        public string elementName;
    }

    /// <summary>
    /// Maps enum -> VisualElement via inspector.
    /// </summary>
    public class UIScreenRegistry : MonoBehaviour {
        [SerializeField] private UIDocument document;
        [SerializeField] private List<ScreenRef> screens;

        private Dictionary<UIScreen, VisualElement> map;

        public Dictionary<UIScreen, VisualElement> Map => map;
        public VisualElement Root => document.rootVisualElement;

        public void Init() {
            map = new Dictionary<UIScreen, VisualElement>();

            VisualElement root = document.rootVisualElement;

            foreach (ScreenRef s in screens) {
                VisualElement el = root.Q<VisualElement>(s.elementName);

                if (el == null) {
                    Debug.LogError($"UIScreenRegistry: '{s.elementName}' not found");
                    continue;
                }
                map[s.id] = el;
            }
        }
    }
}
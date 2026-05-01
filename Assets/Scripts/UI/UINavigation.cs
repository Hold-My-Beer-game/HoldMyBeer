using System.Collections.Generic;
using UnityEngine.UIElements;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Controls which screen is visible.
    /// Stack-based for Back() support
    /// </summary>
    public class UINavigation {
        private readonly Dictionary<UIScreen, VisualElement> screens;
        private readonly Stack<UIScreen> history = new();

        private UIScreen current;

        public UINavigation(Dictionary<UIScreen, VisualElement> screens, UIScreen start) {
            this.screens = screens;
            Show(start, record: false);
        }

        public void Open(UIScreen screen) {
            if (!screens.ContainsKey(screen))
                return;

            Show(screen, true);
        }

        public void Back() {
            if (history.Count == 0)
                return;

            Show(history.Pop(), false);
        }

        private void Show(UIScreen screen, bool record) {
            foreach (KeyValuePair<UIScreen, VisualElement> kv in screens) {
                if (kv.Value == null) continue;

                kv.Value.style.display =
                    kv.Key == screen ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (record)
                history.Push(current);

            current = screen;
        }
    }
}
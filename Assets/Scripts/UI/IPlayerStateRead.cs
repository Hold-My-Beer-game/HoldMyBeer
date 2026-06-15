using System;

namespace HoldMyBeer.UI {
    public interface IPlayerStateRead {
        public event Action<float> OnHealthChange;
        public event Action OnDeath;
        public event Action<float> OnAlcoholChange;
        public event Action<string> OnGoalChange;
        /// <summary>
        /// Raised when Player can interact.
        /// <list type="bullet">
        ///<item>bool: true if player can interact; otherwise false</item>
        ///<item>string: Interact message</item> 
        /// </list>>
        /// </summary>
        public event Action<bool, string> OnInteract;
        public event Action<int> OnAmmoChange;
        public event Action<int> OnLoadChange;
    }
}

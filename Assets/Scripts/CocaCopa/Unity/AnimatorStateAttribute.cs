using UnityEngine;

namespace CocaCopa.Unity {
    public sealed class AnimatorStateAttribute : PropertyAttribute {
        public string AnimatorFieldName { get; }
        public bool HasAnimatorFieldName => !string.IsNullOrWhiteSpace(AnimatorFieldName);

        public bool UseFullPath { get; }

        public AnimatorStateAttribute(bool useFullPath = true) {
            AnimatorFieldName = null;
            UseFullPath = useFullPath;
        }

        public AnimatorStateAttribute(string animatorFieldName, bool useFullPath = true) {
            AnimatorFieldName = animatorFieldName;
            UseFullPath = useFullPath;
        }
    }
}
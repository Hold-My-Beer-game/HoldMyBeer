using UnityEngine;

namespace CocaCopa.Unity {
    public sealed class AnimatorParameterAttribute : PropertyAttribute {
        public string AnimatorFieldName { get; }
        public bool HasAnimatorFieldName => !string.IsNullOrWhiteSpace(AnimatorFieldName);

        public bool FilterByType { get; }
        public AnimatorControllerParameterType ParameterType { get; }

        public AnimatorParameterAttribute() {
            AnimatorFieldName = null;
            FilterByType = false;
            ParameterType = default;
        }

        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType) {
            AnimatorFieldName = null;
            FilterByType = true;
            ParameterType = parameterType;
        }

        public AnimatorParameterAttribute(string animatorFieldName) {
            AnimatorFieldName = animatorFieldName;
            FilterByType = false;
            ParameterType = default;
        }

        public AnimatorParameterAttribute(string animatorFieldName, AnimatorControllerParameterType parameterType) {
            AnimatorFieldName = animatorFieldName;
            FilterByType = true;
            ParameterType = parameterType;
        }
    }
}
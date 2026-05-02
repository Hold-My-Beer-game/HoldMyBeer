using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal interface IScreamAffected {
        void React(Vector3 screamPos, Vector3 targetPos);
    }
}
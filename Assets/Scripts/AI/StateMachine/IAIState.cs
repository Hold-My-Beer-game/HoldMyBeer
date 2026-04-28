using System;

namespace HoldMyBeer.AI {
    public interface IAIState {
        string Id { get; }
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
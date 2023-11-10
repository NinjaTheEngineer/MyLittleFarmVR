using System;
using UnityEngine;

namespace NinjaTools {
    public abstract class BaseState<EState> where EState : Enum {
        public BaseState(EState key) {
            StateKey = key;
        }

        public StateManager<EState> StateMachine { get; private set; }
        public EState StateKey { get; private set; }
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void UpdateState();
        public abstract EState GetNextState();
        public abstract void OnTriggerEnter(Collider other);
        public abstract void OnTriggerStay(Collider other);
        public abstract void OnTriggerExit(Collider other);
    }
}
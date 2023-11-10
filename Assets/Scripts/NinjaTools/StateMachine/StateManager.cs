using System;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTools {
    public abstract class StateManager<EState> : NinjaMonoBehaviour where EState : Enum {
        protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
        protected BaseState<EState> CurrentState;
        protected bool IsTransitioningState = false;
        void Start() {
            var logId = "Start";
            logd(logId," Starting from stateManager");
            CurrentState.EnterState();
        }
        public virtual void Update() {
            var logId = "Update";
            EState nextStateKey = CurrentState.GetNextState();

            if (IsTransitioningState) {
                logd(logId, "Currently Transitioning to State => Continuing", true);
                return;
            }

            if(nextStateKey.Equals(CurrentState.StateKey)) {
                CurrentState.UpdateState();
            } else {
                TransitionToState(nextStateKey);
            }
        }

        public void TransitionToState(EState stateKey) {
            var logId = "TransitionToState";
            IsTransitioningState = true;
            CurrentState.ExitState();
            logd(logId, "CurrentState="+CurrentState.StateKey+" TransitionTo="+stateKey);
            CurrentState = States[stateKey];
            CurrentState.EnterState();
            OnStateChange(stateKey);
            IsTransitioningState = false;
        }
        void OnTriggerEnter(Collider other){
            CurrentState.OnTriggerEnter(other);
         }
        void OnTriggerStay(Collider other) {
            CurrentState.OnTriggerStay(other);
        }
        void OnTriggerExit(Collider other) {
            CurrentState.OnTriggerExit(other);
        }
        public abstract void OnStateChange(EState state);
    }
}

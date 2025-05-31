using System;
using System.Collections.Generic;
using UnityEngine;

namespace MadKoala.StopMotion
{
    public class StopMotionController
    {
        private Animator _animator;
        private int _framesPerSecond;
        private float _frameTime;
        private float _accumulatedTime;
        private float _lastFrameTime;
        private int _layers;
        private Dictionary<int, AnimatorState> states = new Dictionary<int, AnimatorState>();

        public void Set(Animator animator, int framesPerSecond)
        {
            _animator = animator;
            _layers = _animator.layerCount;
            _framesPerSecond = framesPerSecond;
            _animator.speed = 0;
            _framesPerSecond = Math.Max(_framesPerSecond, 1);
            _frameTime = 1f/_framesPerSecond;
            _lastFrameTime = Time.time;
            UpdateStateInfo();
        }

        public void ChangeFrameRate(int framesPerSecond)
        {
            _framesPerSecond = framesPerSecond;
            _framesPerSecond = Math.Min(_framesPerSecond, 1);
            _frameTime = 1f / _framesPerSecond;
        }

        public void Update(float dt)
        {
            UpdateStateInfo();
        
            float deltaTime = Time.time - _lastFrameTime;
            // Advance frames based on target FPS
            if (deltaTime >= _frameTime)
            {
                AdvanceFrame();
                _lastFrameTime = Time.time;
            }
        }
        
        void UpdateStateInfo()
        {
            for (int layerIndex = 0; layerIndex < _layers; layerIndex++)
            {
                var hash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                var stateInfo = _animator.GetCurrentAnimatorStateInfo(layerIndex);
                states.TryGetValue(layerIndex, out var currentState);
                if (currentState == null || currentState.CurrentStateHash != hash)
                {
                    states[layerIndex] = new AnimatorState()
                    {
                        CurrentStateHash = stateInfo.fullPathHash,
                        StateLength = stateInfo.length,
                        StateStepByFrame = stateInfo.length / _framesPerSecond,
                        AnimationProgress = stateInfo.normalizedTime % 1 // Keep between 0-1
                    };
                }
            }
        }

        void AdvanceFrame()
        {
            foreach (var pair in states)
            {
                var state = pair.Value;
                // Calculate progress through animation (looping)
                state.AnimationProgress += state.StateStepByFrame / state.StateLength;
                //state.AnimationProgress += _frameTime / state.StateLength;
                state.AnimationProgress %= 1f; // Wrap around for looping animations
                // Force animator to specific frame
                _animator.Play(state.CurrentStateHash, pair.Key, state.AnimationProgress);
            }
            _animator.Update(0f); // Force immediate update
        }
    }

    public class AnimatorState
    {
        public int CurrentStateHash;
        public float StateLength;
        public float StateStepByFrame;
        public float AnimationProgress;
    }
}
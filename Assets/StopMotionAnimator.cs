using System;
using UnityEngine;

namespace MadKoala.StopMotion
{
    public class StopMotionAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private int _framesPerSecond;
        private StopMotionController _controller;

        private int _currentFrameRate;
        
        private Animator animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _currentFrameRate = _framesPerSecond;
            _controller = new StopMotionController();
            _controller.Set(animator,_currentFrameRate);
        }

        // Update is called once per frame
        void Update()
        {
            CheckChanges();
            _controller.Update(Time.deltaTime);
        }
        
        private void CheckChanges()
        {
            if(_controller == null) return;

            if (_currentFrameRate != _framesPerSecond)
            {
                _currentFrameRate = _framesPerSecond;
                _controller.ChangeFrameRate(_currentFrameRate);
            }
        }
    }
}

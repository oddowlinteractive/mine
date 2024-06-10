using System;
using UnityEngine;

namespace Managers
{
    public class TickManager : MonoBehaviour
    {

        public static Action<Int64> OnTick;

        private const float TickTimerMax = 0.2f;
        private Int64 _tick;
        private float _tickTimer;
    
        void Awake()
        {
            _tick = 0;
        }

        // Update is called once per frame
        void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= TickTimerMax)
            {
                //_tickTimer -= TickTimerMax;
                _tickTimer = 0;
                _tick++;
                OnTick?.Invoke(_tick);
            }
        }
    }
}

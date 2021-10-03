using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.UI
{
    public class TimeEventController : MonoBehaviour
    {
        [ReadOnly]
        public float currentTime;

        public List<TimeEvent> TimeEvents = new List<TimeEvent>();

        public TimeEvent repeatConstantly = new TimeEvent();
        public float cachedRepeatConstantlyTime; 


        public void Awake()
        {
            cachedRepeatConstantlyTime = repeatConstantly.time; 
        }

        private void FixedUpdate()
        {
            currentTime += Time.fixedDeltaTime;

            repeatConstantly.time -= Time.deltaTime; 
            if(repeatConstantly.time <0)
            {
                repeatConstantly.e.Invoke();
                repeatConstantly.time = cachedRepeatConstantlyTime; 
            }


            for (var i = 0; i < TimeEvents.Count; i++)
            {
                if (TimeEvents[i].time < currentTime)
                {
                    TimeEvents[i].e.Invoke();
                    TimeEvents.Remove(TimeEvents[i]);
                    break; 
                }

               
            }
        }
    }

    [System.Serializable]
    public struct TimeEvent
    {
        public float time; 
        public UnityEvent e; 
    }
}
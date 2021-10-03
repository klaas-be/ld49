using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts.UI
{
    public class TimeEventController : MonoBehaviour
    {
        public float currentTime;

        public List<TimeEvent> TimeEvents = new List<TimeEvent>();

        private void Awake()
        {
            List<Time>
        }

        private void Update()
        {
            currentTime += Time.deltaTime; 
            
            
        }
    }

    public struct TimeEvent
    {
        public float time; 
        UnityEvent e; 
    }
}
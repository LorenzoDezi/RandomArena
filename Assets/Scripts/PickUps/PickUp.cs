using FPSDemo.Scripts.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.Scripts.PickUps
{

    public class PickUpEventArgs
    {
        private int pickupManagerIndexTracker;
        public int PickUpManagerIndexTracker
        {
            get { return pickupManagerIndexTracker; }

            set { pickupManagerIndexTracker = value; }
        }

        public PickUpEventArgs(int indexTracker)
        {
            this.pickupManagerIndexTracker = indexTracker;
        }
    }

    public class PickUp : MonoBehaviour
    {

        Transform childrenTransform;

        [HideInInspector]
        public int ManagerIndexTracker;

        public delegate void PickUpEventHandler(object sender, PickUpEventArgs args);

        event PickUpEventHandler PickUpEvent;


        void Start()
        {

            childrenTransform = this.GetComponentInChildren<Transform>();
            if(PickUpManager.manager != null)
                this.PickUpEvent += PickUpManager.manager.OnPickUp;
        }

        void Update()
        {

            childrenTransform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime * 2f);
        }

        public void RaisePickUpEvent()
        {
            PickUpEvent.Invoke(new object(), new PickUpEventArgs(this.ManagerIndexTracker));
        }
    }

    
}



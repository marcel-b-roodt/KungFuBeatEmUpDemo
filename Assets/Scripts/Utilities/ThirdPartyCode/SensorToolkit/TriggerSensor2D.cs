using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SensorToolkit
{
    /*
     * A sensor that detects colliders that cause the sensors OnTriggerEnter function to be called. This means
     * that the list of detected objects is updated outside of the sensor being pulsed. Pulsing the sensor may
     * still be required though for refreshing the line of sight tests if they are enabled.
     *
     * This sensor guards against cases where a collider causes OnTriggerEnter to be called, but no corresponding
     * OnTriggerExit event occurs. This can happen if a collider is disabled and then re-enabled outside of the
     * sensors range. The sensor expects all detected colliders to regularly create OnTriggerStay events, and if they
     * don't then the collider is timed out and the detection is lost.
     */
    public class TriggerSensor2D : BaseAreaSensor
    {
        // Should the sensor be pulsed automatically at fixed intervals or should it be pulsed manually. For the
        // trigger sensor this is only relevant to refreshing the line of sight tests as the colliders are detected
        // outside of the pulse method.
        public enum UpdateMode { FixedInterval, Manual }
        public UpdateMode LineOfSightUpdateMode;

        // If set to pulse automatically this is the interval in seconds between each automatic pulse.
        public float CheckLineOfSightInterval = 1f;

        // A callback that is called each time the list of detected objects is changed. This is used by the editor
        // extensions and you shouldn't need to listen to it.
        public delegate void SensorUpdateHandler();
        public event SensorUpdateHandler OnSensorUpdate;

        HashSet<GameObject> previousDetectedObjects;
        Dictionary<Collider2D, int> triggerStayLag;
        SensorMode oldDetectionMode;
        bool oldRequiresLineOfSight;
        List<Collider2D> collidersToRemove;
        List<Collider2D> collidersToIncrement;

        // Pulses the sensor, causing it to refresh its line of sight tests.
        public override void Pulse()
        {
            if (isActiveAndEnabled) testSensor();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            previousDetectedObjects = new HashSet<GameObject>();
            triggerStayLag = new Dictionary<Collider2D, int>();
            oldDetectionMode = DetectionMode;
            oldRequiresLineOfSight = RequiresLineOfSight;
            collidersToRemove = new List<Collider2D>();
            collidersToIncrement = new List<Collider2D>();

            if (!checkForTriggers())
            {
                Debug.LogWarning("Trigger Sensor cannot detect anything if there are no triggers on the same GameObject.", gameObject);
            }
            if (GetComponent<Rigidbody2D>() == null)
            {
                Debug.LogWarning("In order to detect GameObjects properly the TriggerSensor must itself have a RigidBody. Recommend adding a kinematic RigidBody.");
            }
            else if (GetComponent<Rigidbody2D>().sleepMode != RigidbodySleepMode2D.NeverSleep)
            {
                Debug.LogWarning("The rigidbody which owns the trigger collider should have its 'Sleeping Mode' parameter set to 'Never Sleep'");
            }

            StartCoroutine(LineOfSightRoutine());
        }

        void Update()
        {
            // If one of these properties is changed at runtime then the list of DetectedObjects will be changed immediately. This code ensures
            // that the relevant sensor events are fired.
            if (oldDetectionMode != DetectionMode || oldRequiresLineOfSight != RequiresLineOfSight)
            {
                sensorDetectionEvents();
                oldDetectionMode = DetectionMode;
                oldRequiresLineOfSight = RequiresLineOfSight;
            }

            // Increment triggerStayLag for each detected collider, if it is increased above a threshold this means its
            // no longer calling OnTriggerStay and should be removed from the list of detected colliders.
            collidersToRemove.Clear();
            collidersToIncrement.Clear();
            var colliderStayLagEnumerator = triggerStayLag.Keys.GetEnumerator();
            while (colliderStayLagEnumerator.MoveNext())
            {
                var c = colliderStayLagEnumerator.Current;
                int currentCount;
                triggerStayLag.TryGetValue(c, out currentCount);
                if (currentCount >= 10)
                {
                    collidersToRemove.Add(c);
                }
                else
                {
                    collidersToIncrement.Add(c);
                }
            }

            for (int i = 0; i < collidersToRemove.Count; i++)
            {
                removeCollider(collidersToRemove[i]);
            }
            for (int i = 0; i < collidersToIncrement.Count; i++)
            {
                var c = collidersToIncrement[i];
                int currentCount;
                triggerStayLag.TryGetValue(c, out currentCount);
                triggerStayLag[c] = currentCount + 1;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            addCollider(other);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (!triggerStayLag.ContainsKey(other)) { addCollider(other); }
            triggerStayLag[other] = 0;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            removeCollider(other);
        }

        new void addCollider(Collider2D other)
        {
            var newDetected = base.addCollider(other);
            triggerStayLag[other] = 0;
            if (newDetected != null)
            {
                OnDetected.Invoke(newDetected);
                previousDetectedObjects.Add(newDetected);
            }
            if (OnSensorUpdate != null) OnSensorUpdate();
        }

        new void removeCollider(Collider2D other)
        {
            triggerStayLag.Remove(other);
            var detectionLost = base.removeCollider(other);
            if (detectionLost != null)
            {
                OnLostDetection.Invoke(detectionLost);
                previousDetectedObjects.Remove(detectionLost);
            }
            if (OnSensorUpdate != null) OnSensorUpdate();
        }

        IEnumerator LineOfSightRoutine()
        {
            while (true)
            {
                if (!RequiresLineOfSight || LineOfSightUpdateMode == UpdateMode.Manual) yield return null;

                testSensor();

                if (CheckLineOfSightInterval > 0f)
                    yield return new WaitForSeconds(CheckLineOfSightInterval);
                else
                    yield return null;
            }
        }

        void testSensor()
        {
            refreshLineOfSight();
            sensorDetectionEvents();
            if (OnSensorUpdate != null) OnSensorUpdate();
        }

        void sensorDetectionEvents()
        {
            var detectedEnumerator = DetectedObjects.GetEnumerator();

            while (detectedEnumerator.MoveNext())
            {
                var go = detectedEnumerator.Current;
                if (previousDetectedObjects.Contains(go))
                {
                    previousDetectedObjects.Remove(go);
                }
                else
                {
                    // This is a newly detected object
                    OnDetected.Invoke(go);
                }
            }

            // Any object still in previousDetectedObjects is no longer detected
            var previousDetectedEnumerator = previousDetectedObjects.GetEnumerator();
            while (previousDetectedEnumerator.MoveNext())
            {
                var go = previousDetectedEnumerator.Current;
                OnLostDetection.Invoke(go);
            }

            previousDetectedObjects.Clear();
            detectedEnumerator = DetectedObjects.GetEnumerator();
            while (detectedEnumerator.MoveNext())
            {
                previousDetectedObjects.Add(detectedEnumerator.Current);
            }
        }

        bool checkForTriggers()
        {
            var hasRB = GetComponent<Rigidbody2D>() != null;
            if (hasRB)
            {
                foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
                {
                    if (c.enabled && c.isTrigger) return true;
                }
            }
            else
            {
                foreach (Collider2D c in GetComponents<Collider2D>())
                {
                    if (c.enabled && c.isTrigger) return true;
                }
            }
            return false;
        }
    }
}

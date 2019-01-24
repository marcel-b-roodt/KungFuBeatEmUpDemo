using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace SensorToolkit
{
    /*
     * Detects GameObjects along a ray, it's defined by it's length, which physics layers it detects Objects on and which physics layers obstructs
     * its path. The ray sensor can be queried for the RayCastHit objects associated with each object it detects, so that it's possible to get the
     * point of contact, surface normal etc. As well as this the ray sensor can be queried for the collider that blocked it's path.
     *
     * If the DetectsOnLayers layermask is a subset of the ObstructedByLayers layermask then the ray sensor will use the RayCast method as an
     * optmization. Otherwise it will use the RayCastAll method.
     */
    [ExecuteInEditMode]
	public class RaySensor2D : Sensor
    {
        // Specified whether the ray sensor will pulse automatically each frame or will be updated manually by having its Pulse() method called when needed.
        public enum UpdateMode { EachFrame, Manual }

        // The length of the ray sensor detection range in world units.
        public float Length = 5f;

        // A layermask for colliders that will block the ray sensors path.
        public LayerMask ObstructedByLayers;

        // A layermask for colliders that are detected by the ray sensor.
        public LayerMask DetectsOnLayers;

        // In Collider mode this sensor will show each GameObject with colliders detected by the sensor. In RigidBody
        // mode it will only show the attached RigidBodies to the colliders that are detected.
        public SensorMode DetectionMode;

        // What direction does the ray sensor detect in.
        public Vector2 Direction = Vector2.up;

        // Is the Direction parameter in world space or local space.
        public bool WorldSpace = false;

        // Should the sensor pulse each frame automatically or will it be pulsed manually.
        public UpdateMode SensorUpdateMode;

        // Returns a list of all detected GameObjects in no particular order.
        public override IEnumerable<GameObject> DetectedObjects
        {
            get
            {
                var detectedEnumerator = detectedObjects.GetEnumerator();
                while (detectedEnumerator.MoveNext())
                {
                    var go = detectedEnumerator.Current;
                    if (go != null && go.activeInHierarchy)
                    {
                        yield return go;
                    }
                }
            }
        }

        // Returns a list of all detected GameObjects in order of distance from the sensor. This distance is given by the RaycastHit.dist for each GameObject.
        public override IEnumerable<GameObject> DetectedObjectsOrderedByDistance { get { return DetectedObjects; } }

        // Returns a list of all RaycastHit objects, each one is associated with a GameObject in the detected objects list.
        public IList<RaycastHit2D> DetectedObjectRayHits { get { return new List<RaycastHit2D>(detectedObjectHits.Values); } }

        // Returns the Collider that obstructed the ray sensors path, or null if it wasn't obstructed.
        public Collider2D ObstructedBy { get { return obstructionRayHit.collider; } }

        // Returns the RaycastHit data for the collider that obstructed the rays path.
        public RaycastHit2D ObstructionRayHit { get { return obstructionRayHit; } }

        // Returns true if the ray sensor is being obstructed and false otherwise
        public bool IsObstructed { get { return isObstructed && ObstructedBy != null; } }

        // Event fired at the time the sensor is obstructed when before it was unobstructed
        [SerializeField]
        public UnityEvent OnObstruction;

        // Event fired at the time the sensor is unobstructed when before it was obstructed
        [SerializeField]
        public UnityEvent OnClear;

        // Event fired each time the sensor is pulsed. This is used by the editor extension and you shouldn't have to subscribe to it yourself.
        public delegate void SensorUpdateHandler();
        public event SensorUpdateHandler OnSensorUpdate;

        Vector2 direction { get { return WorldSpace ? Direction.normalized : (Vector2)transform.TransformDirection(Direction.normalized); } }
        RayDistanceComparer2D distanceComparer;

        bool isObstructed = false;
        RaycastHit2D obstructionRayHit;
        Dictionary<GameObject, RaycastHit2D> detectedObjectHits;
        HashSet<GameObject> previousDetectedObjects;
        List<GameObject> detectedObjects;

        // Returns true if the passed GameObject appears in the sensors list of detected gameobjects
        public override bool IsDetected(GameObject go)
        {
            return detectedObjectHits.ContainsKey(go);
        }

        // Pulse the ray sensor
        public override void Pulse()
        {
            if (isActiveAndEnabled) testRay();
        }

        // detectedGameObject should be a GameObject that is detected by the sensor. In this case it will return
        // the Raycasthit data associated with this object.
        public RaycastHit2D GetRayHit(GameObject detectedGameObject)
        {
            RaycastHit2D val;
            if (!detectedObjectHits.TryGetValue(detectedGameObject, out val))
            {
                Debug.LogWarning("Tried to get the RaycastHit for a GameObject that isn't detected by RaySensor.");
            }
            return val;
        }

        void OnEnable()
        {
            detectedObjects = new List<GameObject>();
            distanceComparer = new RayDistanceComparer2D();
            detectedObjectHits = new Dictionary<GameObject, RaycastHit2D>();
            previousDetectedObjects = new HashSet<GameObject>();
            clearDetectedObjects();
        }

        void Update()
        {
            if (Application.isPlaying && SensorUpdateMode == UpdateMode.EachFrame) testRay();
        }

        bool layerMaskIsSubsetOf(LayerMask lm, LayerMask subsetOf)
        {
            return ((lm | subsetOf) & (~subsetOf)) == 0;
        }

        void testRay()
        {
            clearDetectedObjects();
            if (layerMaskIsSubsetOf(DetectsOnLayers, ObstructedByLayers) && (IgnoreList == null || IgnoreList.Length == 0))
            {
                testRaySingle();
            }
            else
            {
                testRayMulti();
            }

            obstructionEvents();
            detectionEvents();

            if (OnSensorUpdate != null) OnSensorUpdate();
        }

        void obstructionEvents()
        {
            if (isObstructed && obstructionRayHit.collider == null)
            {
                isObstructed = false;
                OnClear.Invoke();
            }
            else if (!isObstructed && obstructionRayHit.collider != null)
            {
                isObstructed = true;
                OnObstruction.Invoke();
            }
        }

        void detectionEvents()
        {
            // Any GameObjects still in previousDetectedObjects are no longer detected
            var lostDetectionEnumerator = previousDetectedObjects.GetEnumerator();
            while (lostDetectionEnumerator.MoveNext())
            {
                OnLostDetection.Invoke(lostDetectionEnumerator.Current);
            }

            previousDetectedObjects.Clear();
            for (int i = 0; i < detectedObjects.Count; i++)
            {
                previousDetectedObjects.Add(detectedObjects[i]);
            }
        }

        void testRaySingle()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Length, ObstructedByLayers);
            if (hit.collider != null)
            {
                if ((1 << hit.collider.gameObject.layer & DetectsOnLayers) != 0)
                {
                    addRayHit(hit);
                }
                obstructionRayHit = hit;
            }
        }

        void testRayMulti()
        {
            LayerMask combinedLayers = DetectsOnLayers | ObstructedByLayers;
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, Length, combinedLayers);
            System.Array.Sort(hits, distanceComparer);

            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if ((1 << hit.collider.gameObject.layer & DetectsOnLayers) != 0)
                {
                    addRayHit(hit);
                }
                if ((1 << hit.collider.gameObject.layer & ObstructedByLayers) != 0)
                {
                    // Potentially blocks the ray, just make sure it isn't in the ignore list
                    if (shouldIgnore(hit.collider.gameObject)
                        || hit.rigidbody != null
                        && shouldIgnore(hit.rigidbody.gameObject))
                    {
                        // Obstructing collider or its rigid body is in the ignore list
                        continue;
                    }
                    else
                    {
                        obstructionRayHit = hit;
                        break;
                    }
                }
            }
        }

        void addRayHit(RaycastHit2D hit)
        {
            GameObject go;
            if (DetectionMode == SensorMode.RigidBodies)
            {
                if (hit.rigidbody == null) return;
                go = hit.rigidbody.gameObject;
            }
            else
            {
                go = hit.collider.gameObject;
            }
            if (!detectedObjectHits.ContainsKey(go) && !shouldIgnore(go))
            {
                detectedObjectHits.Add(go, hit);
                detectedObjects.Add(go);
                if (!previousDetectedObjects.Contains(go))
                {
                    OnDetected.Invoke(go);
                }
                else
                {
                    previousDetectedObjects.Remove(go);
                }
            }
        }

        void clearDetectedObjects()
        {
            obstructionRayHit = new RaycastHit2D();
            detectedObjectHits.Clear();
            detectedObjects.Clear();
        }

        void reset()
        {
            clearDetectedObjects();
            isObstructed = false;
        }

        class RayDistanceComparer2D : IComparer<RaycastHit2D>
        {
            public int Compare(RaycastHit2D x, RaycastHit2D y)
            {
                if (x.distance < y.distance) { return -1; }
                else if (x.distance > y.distance) { return 1; }
                else { return 0; }
            }
        }

        protected static readonly Color GizmoColor = new Color(51 / 255f, 255 / 255f, 255 / 255f);
        protected static readonly Color GizmoBlockedColor = Color.red;
        public void OnDrawGizmosSelected()
        {
            if (!isActiveAndEnabled) return;

            if (IsObstructed)
            {
                Gizmos.color = GizmoBlockedColor;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * obstructionRayHit.distance);
            }
            else
            {
                Gizmos.color = GizmoColor;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * Length);
            }

            Gizmos.color = GizmoColor;
            foreach (RaycastHit2D hit in DetectedObjectRayHits)
            {
                Gizmos.DrawIcon(hit.point, "SensorToolkit/eye.png", true);
            }
        }
    }
}

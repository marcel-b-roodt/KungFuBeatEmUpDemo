using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SensorToolkit
{
    /*
     * Common functionality for sensors that detect colliders within a volume. Sensors that implement this class should
     * detect colliders and pass those colliders to this base implementation through the addCollider protected method. When
     * colliders are no longer detected then a corresponding call to removeCollider should be made. This class manages
     * the list of detected objects by processing the list of detected colliders.
     *
     * Includes optional support for line of sight testing, when enabled a GameObject will only appear as detected if
     * it passes a line of sight test. To calculate line of sight the sensor casts a number of rays towards the target
     * object. The ratio of these test rays that reach the object unobstructed is it's computed visibility. If the visibility
     * ratio is greater then a user specified amount then the object appears in the detected list. These target points can
     * be specified by adding a LOSTargets component to the object, or else they are randomly generated. See the documentation 
     * at www.micosmo.com/sensortoolkit/#rangesensor for more info.
     * 
     * If line of sight testing is enabled then it will need to be continually refreshed by calls to the refreshLineOfSight()
     * protected method. Ideally this should happen during a pulse.
     */
    [ExecuteInEditMode]
    public abstract class BaseVolumeSensor : Sensor
    {
        // In Collider mode this sensor will show each GameObject with colliders detected by the sensor. In RigidBody
        // mode it will only show the attached RigidBodies to the colliders that are detected.
        public SensorMode DetectionMode;

        // GameObjects are only detected if they pass a line of sight test. The refreshLineOfSight method must be called
        // regularly to update the set of objects in line of sight.
        public bool RequiresLineOfSight;

        // Line of Sight rays will be tested against this layer mask.
        public LayerMask BlocksLineOfSight;

        // If true will only perform line of sight tests on objects with a LOSTargets component, if false then
        // the sensor will auto-generate test points for object which don't have this component.
        public bool TestLOSTargetsOnly;

        // Number of test points the Sensor will generate on objects that don't have a LOSTargets component.
        [Range(1, 20)]
        public int NumberOfRays = 1;

        // Minimum visibility an object must be for it to be detected.
        [Range(0f, 1f)]
        public float MinimumVisibility = 0.5f;

        // Displays the results of line of sight tests during OnDrawGizmosSelected for objects in this set.
        public HashSet<GameObject> ShowRayCastDebug;

        // Returns a list of all GameObjects detected by this sensor.
        public override IEnumerable<GameObject> DetectedObjects
        {
            get
            {
                if (RequiresLineOfSight)
                {
                    var objectVisibilityEnumerator = objectVisibility.Keys.GetEnumerator();
                    while (objectVisibilityEnumerator.MoveNext())
                    {
                        var go = objectVisibilityEnumerator.Current;
                        if (go != null && go.activeInHierarchy && !shouldIgnore(go) && objectVisibility[go] >= MinimumVisibility)
                        {
                            yield return go;
                        }
                    }
                }
                else if (DetectionMode == SensorMode.RigidBodies)
                {
                    var rigidBodyCollidersEnumerator = rigidBodyColliders.Keys.GetEnumerator();
                    while (rigidBodyCollidersEnumerator.MoveNext())
                    {
                        var go = rigidBodyCollidersEnumerator.Current;
                        if (go != null && go.activeInHierarchy && !shouldIgnore(go))
                        {
                            yield return go;
                        }
                    }
                }
                else
                {
                    var gameObjectCollidersEnumerator = gameObjectColliders.Keys.GetEnumerator();
                    while (gameObjectCollidersEnumerator.MoveNext())
                    {
                        var go = gameObjectCollidersEnumerator.Current;
                        if (go != null && go.activeInHierarchy && !shouldIgnore(go))
                        {
                            yield return go;
                        }
                    }
                }
            }
        }

        // Returns a list of all GameObjects detected by sensor and ordered by their distance from the sensor.
        // Bit of a hack here, don't get nested enumerators from this property.
        public override IEnumerable<GameObject> DetectedObjectsOrderedByDistance
        {
            get
            {
                gameObjectList.Clear();
                gameObjectList.AddRange(DetectedObjects);
                distanceComparer.Point = transform.position;
                gameObjectList.Sort(distanceComparer);
                return gameObjectList;
            }
        }

        // Returns a map of every sensed object and their computed visibility. This includes objects whose
        // visibility is too low to be detected.
        public Dictionary<GameObject, float> ObjectVisibilities
        {
            get
            {
                return objectVisibility;
            }
        }

        // Returns the visibility between 0-1 of the specified object. A 0 means its not visible at all while
        // a 1 means it is entirely visible. Visibility only makes sense in the context of line of sight tests,
        // it is the ratio of rays towards the target object that are clear of obstructions.
        public override float GetVisibility(GameObject go)
        {
            if (!RequiresLineOfSight) return base.GetVisibility(go);
            else if (objectVisibility.ContainsKey(go)) return objectVisibility[go];
            else return 0f;
        }

        // Returns a list of transforms on the given object that passed line of sight tests. Will only return
        // results for objects that have a LOSTargets component.
        public List<Transform> GetVisibleTransforms(GameObject go)
        {
            RayCastTargets targets;
            if (go != null && rayCastTargets.TryGetValue(go, out targets))
            {
                return targets.GetVisibleTransforms();
            }
            else
            {
                return new List<Transform>();
            }
        }

        // Returns a list of positions on a given object that passed line of sight tests.
        public List<Vector3> GetVisiblePositions(GameObject go)
        {
            RayCastTargets targets;
            if (go != null && rayCastTargets.TryGetValue(go, out targets))
            {
                return targets.GetVisibleTargetPositions();
            }
            else
            {
                return new List<Vector3>();
            }
        }

        // Maps a RigidBody GameObject to a list of it's colliders that have been detected. These colliders
        // may be attached to children GameObjects.
        Dictionary<GameObject, List<Collider>> gameObjectColliders = new Dictionary<GameObject, List<Collider>>();

        // Maps a GameObject to a list of it's colliders that have been detected.
        Dictionary<GameObject, List<Collider>> rigidBodyColliders = new Dictionary<GameObject, List<Collider>>();

        // Maps a GameObject to a list of raycast target positions for calculating line of sight
        Dictionary<GameObject, RayCastTargets> rayCastTargets = new Dictionary<GameObject, RayCastTargets>();

        // Maps a detected object to its computed visibility from line of sight tests
        Dictionary<GameObject, float> objectVisibility = new Dictionary<GameObject, float>();

        // A list of results from all the raycast tests
        List<RayCastResult> raycastResults = new List<RayCastResult>();

        // List of temporary values for modifying collections
        List<GameObject> gameObjectList = new List<GameObject>();

        DistanceFromPointComparer distanceComparer = new DistanceFromPointComparer();

        protected static ListCache<Collider> colliderListCache = new ListCache<Collider>();
        protected static ListCache<Vector3> vector3ListCache = new ListCache<Vector3>();
        class RayCastTargetsCache : ObjectCache<RayCastTargets>
        {
            public override void Dispose(RayCastTargets obj)
            {
                obj.dispose();
                base.Dispose(obj);
            }
        }
        static RayCastTargetsCache rayCastTargetsCache = new RayCastTargetsCache();

        struct RayCastResult
        {
            public GameObject go;
            public Vector3 testPoint;
            public Vector3 obstructionPoint;
            public bool isObstructed;
        }

        class RayCastTargets
        {
            GameObject go;
            IList<Transform> targetTransforms;
            List<Vector3> targetPoints;
            List<Vector3> returnPoints;
            List<bool> isTargetVisible;

            public RayCastTargets()
            {
                returnPoints = new List<Vector3>();
                isTargetVisible = new List<bool>();
            }

            public bool IsTransforms()
            {
                return targetTransforms != null;
            }

            public void Set(GameObject go, IList<Transform> targets)
            {
                this.go = go;
                targetTransforms = targets;
                targetPoints = null;
                isTargetVisible.Clear(); for (int i = 0; i < targets.Count; i++) isTargetVisible.Add(false);
            }

            public void Set(GameObject go, List<Vector3> targets)
            {
                this.go = go;
                targetTransforms = null;
                targetPoints = targets;
                isTargetVisible.Clear(); for (int i = 0; i < targets.Count; i++) isTargetVisible.Add(false);
            }

            public List<Transform> GetVisibleTransforms()
            {
                var visibleList = new List<Transform>();
                for (int i = 0; i < isTargetVisible.Count; i++)
                {
                    if (isTargetVisible[i]) visibleList.Add(targetTransforms[i]);
                }
                return visibleList;
            }

            public List<Vector3> GetVisibleTargetPositions()
            {
                var visibleList = new List<Vector3>();
                if (targetTransforms != null)
                {
                    for (int i = 0; i < isTargetVisible.Count; i++)
                    {
                        if (isTargetVisible[i]) visibleList.Add(targetTransforms[i].position);
                    }
                }
                else
                {
                    for (int i = 0; i < isTargetVisible.Count; i++)
                    {
                        if (isTargetVisible[i]) visibleList.Add(go.transform.TransformPoint(targetPoints[i]));
                    }
                }
                return visibleList;
            }

            public void SetIsTargetVisible(int i, bool isVisible)
            {
                isTargetVisible[i] = isVisible;
            }

            public IList<Vector3> getTargetPoints()
            {
                returnPoints.Clear();
                if (targetTransforms != null)
                {
                    for (int i = 0; i < targetTransforms.Count; i++)
                    {
                        returnPoints.Add(targetTransforms[i].position);
                    }
                }
                else
                {
                    var go = this.go;
                    for (int i = 0; i < targetPoints.Count; i++)
                    {
                        returnPoints.Add(go.transform.TransformPoint(targetPoints[i]));
                    }
                }
                return returnPoints;
            }

            public void dispose()
            {
                if (targetPoints != null) { vector3ListCache.Dispose(targetPoints); }
            }
        }

        protected virtual void OnEnable()
        {
            rigidBodyColliders.Clear();
            gameObjectColliders.Clear();
            rayCastTargets.Clear();
            objectVisibility.Clear();
            raycastResults.Clear();
            gameObjectList.Clear();
        }

        protected GameObject addCollider(Collider c)
        {
            GameObject newColliderDetection = null;
            GameObject newRigidBodyDetection = null;

            if (addColliderToMap(c, c.gameObject, gameObjectColliders))
            {
                disposeRayCastTarget(c.gameObject);
                newColliderDetection = c.gameObject;
            }
            if (c.attachedRigidbody != null && addColliderToMap(c, c.attachedRigidbody.gameObject, rigidBodyColliders))
            {
                disposeRayCastTarget(c.attachedRigidbody.gameObject);
                newRigidBodyDetection = c.attachedRigidbody.gameObject;
            }

            var newDetection = DetectionMode == SensorMode.Colliders ? newColliderDetection : newRigidBodyDetection;
            if (shouldIgnore(newDetection))
            {
                return null;
            }
            else if (RequiresLineOfSight && newDetection != null)
            {
                bool prevDetected = objectVisibility.ContainsKey(newDetection) && objectVisibility[newDetection] >= MinimumVisibility;
                var targets = getRayCastTargets(newDetection);
                if (TestLOSTargetsOnly && !targets.IsTransforms()) return null;
                objectVisibility[newDetection] = testObjectVisibility(newDetection, targets);

                if (!prevDetected && objectVisibility[newDetection] >= MinimumVisibility) return newDetection;
                else return null;
            }
            else
            {
                return newDetection;
            }
        }

        protected GameObject removeCollider(Collider c)
        {
            if (c == null)
            {
                clearDestroyedGameObjects();
                return null;
            }

            GameObject colliderDetectionLost = null;
            GameObject rigidBodyDetectionLost = null;

            if (removeColliderFromMap(c, c.gameObject, gameObjectColliders))
            {
                disposeRayCastTarget(c.gameObject);
                colliderDetectionLost = c.gameObject;
            }
            if (c.attachedRigidbody != null && removeColliderFromMap(c, c.attachedRigidbody.gameObject, rigidBodyColliders))
            {
                disposeRayCastTarget(c.attachedRigidbody.gameObject);
                rigidBodyDetectionLost = c.attachedRigidbody.gameObject;
            }

            var detectionLost = DetectionMode == SensorMode.Colliders ? colliderDetectionLost : rigidBodyDetectionLost;
            if (shouldIgnore(detectionLost))
            {
                return null;
            }
            else if (RequiresLineOfSight && detectionLost != null)
            {
                if (objectVisibility.ContainsKey(detectionLost))
                {
                    objectVisibility.Remove(detectionLost);
                    return detectionLost;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return detectionLost;
            }
        }

        protected void clearDestroyedGameObjects()
        {
            gameObjectList.Clear();
            var collidersGameObjectsEnumerator = gameObjectColliders.Keys.GetEnumerator();
            while (collidersGameObjectsEnumerator.MoveNext())
            {
                var go = collidersGameObjectsEnumerator.Current;
                if (go == null) { gameObjectList.Add(go); }
            }
            for (int i = 0; i < gameObjectList.Count; i++)
            {
                gameObjectColliders.Remove(gameObjectList[i]);
            }

            gameObjectList.Clear();
            var rigidBodyGameObjectsEnumerator = rigidBodyColliders.Keys.GetEnumerator();
            while (rigidBodyGameObjectsEnumerator.MoveNext())
            {
                var go = rigidBodyGameObjectsEnumerator.Current;
                if (go == null) { gameObjectList.Add(go); }
            }
            for (int i = 0; i < gameObjectList.Count; i++)
            {
                rigidBodyColliders.Remove(gameObjectList[i]);
            }
        }

        protected void clearColliders()
        {
            var collidersEnumerator = gameObjectColliders.GetEnumerator();
            while (collidersEnumerator.MoveNext())
            {
                var colliderList = collidersEnumerator.Current.Value;
                colliderListCache.Dispose(colliderList);
            }
            gameObjectColliders.Clear();

            collidersEnumerator = rigidBodyColliders.GetEnumerator();
            while (collidersEnumerator.MoveNext())
            {
                var colliderList = collidersEnumerator.Current.Value;
                colliderListCache.Dispose(colliderList);
            }
            rigidBodyColliders.Clear();

            clearLineOfSight();
        }

        protected void clearLineOfSight()
        {
            objectVisibility.Clear();
            raycastResults.Clear();
        }

        protected void refreshLineOfSight()
        {
            objectVisibility.Clear();
            raycastResults.Clear();
            if (DetectionMode == SensorMode.RigidBodies)
            {
                var gosEnumerator = rigidBodyColliders.Keys.GetEnumerator();
                while (gosEnumerator.MoveNext())
                {
                    var go = gosEnumerator.Current;
                    if (go == null) continue;
                    var targets = getRayCastTargets(go);
                    if (TestLOSTargetsOnly && !targets.IsTransforms()) continue;
                    objectVisibility[go] = testObjectVisibility(go, targets);
                }
            }
            else
            {
                var gosEnumerator = gameObjectColliders.Keys.GetEnumerator();
                while (gosEnumerator.MoveNext())
                {
                    var go = gosEnumerator.Current;
                    if (go == null) continue;
                    var targets = getRayCastTargets(go);
                    if (TestLOSTargetsOnly && !targets.IsTransforms()) continue;
                    objectVisibility[go] = testObjectVisibility(go, targets);
                }
            }
        }

        float testObjectVisibility(GameObject go, RayCastTargets targets)
        {
            int nSuccess = 0;
            var rayCastTargets = getRayCastTargets(go);
            IList<Vector3> testPoints = rayCastTargets.getTargetPoints();
            for (int i = 0; i < testPoints.Count; i++)
            {
                var testPoint = testPoints[i];
                Vector3 obstructionPoint;
                var result = new RayCastResult();
                result.go = go;
                result.testPoint = testPoint;
                result.isObstructed = false;
                if (isInLineOfSight(go, testPoint, out obstructionPoint))
                {
                    nSuccess++;
                    rayCastTargets.SetIsTargetVisible(i, true);
                }
                else
                {
                    result.isObstructed = true;
                    result.obstructionPoint = obstructionPoint;
                    rayCastTargets.SetIsTargetVisible(i, false);
                }
                raycastResults.Add(result);
            }

            return nSuccess / (float)testPoints.Count;
        }

        RayCastTargets getRayCastTargets(GameObject go)
        {
            RayCastTargets rts;
            if (rayCastTargets.TryGetValue(go, out rts))
            {
                return rts;
            }
            else
            {
                var losTargets = go.GetComponent<LOSTargets>();
                rts = rayCastTargetsCache.Get();
                if (losTargets != null && losTargets.Targets != null)
                {
                    rts.Set(go, losTargets.Targets);
                    rayCastTargets.Add(go, rts);
                    return rts;
                }
                else
                {
                    rts.Set(go, generateRayCastTargets(go));
                    rayCastTargets.Add(go, rts);
                    return rts;
                }
            }
        }

        List<Vector3> generateRayCastTargets(GameObject go)
        {
            IList<Collider> cs;
            if (DetectionMode == SensorMode.Colliders) cs = gameObjectColliders[go];
            else cs = rigidBodyColliders[go];

            List<Vector3> rts = vector3ListCache.Get();
            if (NumberOfRays == 1)
            {
                rts.Add(getCentreOfColliders(go, cs));
            }
            else
            {
                for (int i = 0; i < NumberOfRays; i++)
                {
                    rts.Add(getRandomPointInColliders(go, cs));
                }
            }
            return rts;
        }

        bool isInLineOfSight(GameObject go, Vector3 testPoint, out Vector3 obstructionPoint)
        {
            obstructionPoint = Vector3.zero;
            var toGoCentre = testPoint - transform.position;

            var ray = new Ray(transform.position, toGoCentre.normalized);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, toGoCentre.magnitude, BlocksLineOfSight))
            {
                // Ray hit something, check that it was the target.
                if (DetectionMode == SensorMode.RigidBodies && hitInfo.rigidbody != null && hitInfo.rigidbody.gameObject == go)
                {
                    return true;
                }
                else if (DetectionMode == SensorMode.Colliders && hitInfo.collider.gameObject == go)
                {
                    return true;
                }
                else
                {
                    obstructionPoint = hitInfo.point;
                    return false;
                }
            }
            else
            {
                // Ray didn't hit anything so assume target is in line of sight
                return true;
            }
        }

        Vector3 getCentreOfColliders(GameObject goRoot, IList<Collider> goColliders)
        {
            Vector3 aggregate = Vector3.zero;
            for (int i = 0; i < goColliders.Count; i++)
            {
                var c = goColliders[i];
                aggregate += c.bounds.center - goRoot.transform.position;
            }
            return aggregate / goColliders.Count;
        }

        Vector3 getRandomPointInColliders(GameObject goRoot, IList<Collider> colliders)
        {
            // Choose a random collider weighted by its volume
            Collider rc = colliders[0];
            var totalVolume = 0f;
            for (int i = 0; i < colliders.Count; i++)
            {
                var c = colliders[i];
                totalVolume += c.bounds.size.x * c.bounds.size.y + c.bounds.size.z;
            }

            var r = Random.Range(0f, 1f);
            for (int i = 0; i < colliders.Count; i++)
            {
                var c = colliders[i];
                rc = c;
                var v = c.bounds.size.x * c.bounds.size.y * c.bounds.size.z;
                r -= v / totalVolume;
                if (r <= 0f) break;
            }

            // Now choose a random point within that random collider and return it
            var rp = new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
            rp.Scale(rc.bounds.size);
            rp += rc.bounds.center - goRoot.transform.position;
            return rp;
        }

        bool addColliderToMap(Collider c, GameObject go, IDictionary<GameObject, List<Collider>> dict)
        {
            var newDetection = false;
            List<Collider> colliderList;
            if (!dict.TryGetValue(go, out colliderList))
            {
                newDetection = true;
                colliderList = colliderListCache.Get();
                dict[go] = colliderList;
            }
            colliderList.Add(c);
            return newDetection;
        }

        bool removeColliderFromMap(Collider c, GameObject go, IDictionary<GameObject, List<Collider>> dict)
        {
            var detectionLost = false;
            List<Collider> colliderList;
            if (dict.TryGetValue(go, out colliderList))
            {
                colliderList.Remove(c);
                if (colliderList.Count == 0)
                {
                    detectionLost = true;
                    dict.Remove(go);
                    colliderListCache.Dispose(colliderList);
                    objectVisibility.Remove(go);
                }
            }
            return detectionLost;
        }

        void disposeRayCastTarget(GameObject forGameObject)
        {
            if (rayCastTargets.ContainsKey(forGameObject))
            {
                rayCastTargetsCache.Dispose(rayCastTargets[forGameObject]);
                rayCastTargets.Remove(forGameObject);
            }
        }

        protected static readonly Color GizmoColor = new Color(51 / 255f, 255 / 255f, 255 / 255f);
        protected static readonly Color GizmoBlockedColor = Color.red;
        public virtual void OnDrawGizmosSelected()
        {
            if (!isActiveAndEnabled) return;

            Gizmos.color = GizmoColor;
            foreach (GameObject go in DetectedObjects)
            {
                Vector3 goCentre = getCentreOfColliders(go, DetectionMode == SensorMode.RigidBodies && rigidBodyColliders.ContainsKey(go)
                    ? rigidBodyColliders[go] : gameObjectColliders[go]) + go.transform.position;
                Gizmos.DrawIcon(goCentre, "SensorToolkit/eye.png", true);
            }

            if (RequiresLineOfSight && ShowRayCastDebug != null)
            {
                foreach (RayCastResult result in raycastResults)
                {
                    if (!ShowRayCastDebug.Contains(result.go)) continue;

                    Gizmos.color = GizmoColor;
                    if (result.isObstructed)
                    {
                        Gizmos.DrawLine(transform.position, result.obstructionPoint);
                        Gizmos.color = GizmoBlockedColor;
                        Gizmos.DrawLine(result.obstructionPoint, result.testPoint);
                        Gizmos.DrawCube(result.testPoint, Vector3.one * 0.1f);
                    }
                    else
                    {
                        Gizmos.DrawLine(transform.position, result.testPoint);
                        Gizmos.DrawCube(result.testPoint, Vector3.one * 0.1f);
                    }
                }
            }
        }
    }
}
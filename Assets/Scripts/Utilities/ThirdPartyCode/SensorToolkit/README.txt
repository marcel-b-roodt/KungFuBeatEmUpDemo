Thank you for purchasing Sensor Toolkit!

The documentation is available online at: http://www.micosmo.com/sensortoolkit
And there is an example scene under the Examples directory.

If you have any questions, feature requests or if you have found a bug then please send me an email at micosmogames@gmail.com

ChangeLog

1.1.3:
- Greatly reduced garbage generated.
- Sensor DetectedObjects and DetectedObjectsOrderedByDistance changed to IEnumerable types, so they can be enumerated without allocations.

1.2.0
- Sensors can now be configured with tag filters.
- Added 'GetVisibleTransforms' and 'GetVisiblePositions' methods to trigger/range sensors, to query the raycast targets on a detected object that are in line of sight.
- range/trigger sensors can be configured to only detect objects with LOSTargets components.
- Added new PlayMaker actions.

1.2.1
- Fixed SensorGetVisibleRaycastTargets playmaker action so that visible Transforms are returned correctly.
- Removed duplicate TagSelectorPropertyDrawer.cs editor file which was causing builds to fail.

1.2.2
- Small bug fix for tag filters

1.2.3
- Fixed issue where RangeSensor wasn't firing DetectionLost events

1.3.0
- Added SteeringRig and SteeringRig2D, along with a few example steering prefabs
- Added 3 new example scenes: Action, Stealth and Space
- Minor bug fixes

1.3.1
- Added LOSTargets to Stealth example
- Minor fix to Physics layers in examples
- Made steering rig gizmos bigger

1.3.2
- FOVCollider and FOVCollider2D now expose their mesh through the FOVMesh property so it can be rendered.
- Added RenderFOVCollider and RenderFOVCollider2D for easily rendering fov colliders
- Fix for occasional error occuring with trigger sensor when gameobjects are rapidly activated and deactivated

1.3.3
- TriggerSensor2D must have a rigid body with sleep mode never sleep.
- New playmaker actions for controlling steering rig.

1.3.4
- Fix bug causing null reference exception

1.3.5
- Added a copy of SpaceDemo with 10x scale to demonstrate how to configure its sensors at a larger scale.
- Fixed a bug causing detection events to not fire when LOS testing was enabled.
- A fix to several SensorGetDetected actions with a bug ocurring when 'Store Component' field is used.
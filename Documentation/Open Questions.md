# Easy Events - Open Questions

## Behaviour
2. `EventManager.TriggerEvent` returns before recording editor history when no listener exists for the event type. Is it intended that events with no subscribers never appear in Event history?
3. Dispatch uses the exact runtime type, so subscribing to a base type or interface never fires. Intended design?
4. The dispatch buffer in `EventListenerCallbackDictionary` is a shared instance field. Is re-entrant triggering (an event fired from inside a callback of the same type) supported? No test covers it.
5. Unsubscription compares `callback.GetHashCode()` only. Is the possibility of collisions between different delegates accepted?
6. Static state is never reset (no domain-reload handling). Is that a concern when Enter Play Mode options disable domain reload?

## Editor tooling
7. `EventDetailsWindow` header shows `_record.GetType().Name` (always the record type) instead of the event type, and lists only public fields, not properties (so record events with positional properties show nothing). Bug or intended?
9. `PACKAGES_DEV` define: where is it expected to be set (only in a dev project with the package embedded under Assets, assumed)? Elsewhere an embedded package may fail to load styles.
10. `EasyEventsEditorBridge` sits in the Runtime assembly under the `EasyEvents.Editor` namespace, guarded by `UNITY_EDITOR`. Deliberate because `EventManager` is internal?

## Tests and samples
11. `Tests/Runtime` has an asmdef but no tests, and references `UnityEditor.TestRunner` without an Editor platform restriction. Placeholder?
12. The sample uses the legacy `Input` API; does it work in projects set to the new Input System only?
13. The 1.0.0 CHANGELOG entry has empty "Changed" and "Fixed" headings. Cosmetic only.

## Answered
- Install URL: the correct URL is `https://github.com/Fixer33/EasyEventsPackage.git`. The README was corrected accordingly.
- History size: now read from EditorPrefs (fixed in source; not yet verified in Unity).

# Easy Events - Package Description

Package `com.fixer33.easy-events`, version 1.0.1, display name "Easy Events", tagline "No more huge EventManager class". Requires Unity 6000.0. Author: Fixer33. License: MIT.

Anything marked (assumed) was inferred, not read directly.

## 1. Purpose and users

A small, static, type-keyed event bus. Instead of one central manager with a method per event, each event is its own type (class, struct or record) implementing `IEvent`. Any object implementing `IEventListener` subscribes to a specific event type with a typed callback. Anyone can fire an event by calling `Trigger()` on an instance.

Users are Unity game developers (the author's own projects, assumed) who want decoupled in-process messaging between gameplay or UI code, with an editor window to inspect recently fired events.

## 2. Architecture and main components

Assemblies:

| Assembly | Folder | Notes |
|---|---|---|
| `com.fixer33.easy-events` (root namespace `EasyEvents`) | `Runtime/` | Runtime code, auto-referenced, no references, no define constraints. |
| `com.fixer33.easy-events.Editor` (`EasyEvents.Editor`) | `Editor/` | Editor-only, references the runtime asmdef by GUID. |
| `com.fixer33.easy-events.Editor.Tests` and `com.fixer33.easy-events.Tests` | `Tests/Editor`, `Tests/Runtime` | Test assemblies. |

Runtime files:
- `IEvent` - marker interface with a default interface method `Trigger()`, which calls `EventManager.TriggerEvent(this)`. `IEventExtensions.Trigger(this IEvent)` is an extension wrapper so `Trigger()` can be called on a concrete struct/class/record variable.
- `IEventListener` - marker interface with internal default methods `StartListeningTo<T>`, `StopListeningTo<T>`, `RemoveAllListeners`. Public access is through `IEventListenerExtensions`. Also declares the delegate `EventListenerCallback<in T>(T eventData) where T : IEvent`.
- `EventManager` - `internal static class`; the actual bus. Holds `Dictionary<Type, EventListenerCallbackDictionary>`. Each inner dictionary maps listener to a list of callbacks (identified by `callback.GetHashCode()`). Dispatch is by the runtime type of the event instance (`eventData.GetType()`), so listeners must subscribe to the exact concrete event type, not a base type or interface.
- `EventHistoryRecord` - record `(DateTime Time, IEvent EventData)`, for editor tooling.
- `EasyEventsEditorBridge` - `public static` class in namespace `EasyEvents.Editor`, but located in the Runtime assembly and wrapped in `#if UNITY_EDITOR`. It exposes the manager's history and update event to the Editor assembly, because `EventManager` is internal.

Dispatch details (read from `EventManager`):
- `TriggerEvent` returns immediately if no listener was ever registered for that event type (no dictionary entry). In that case nothing is added to the editor history either.
- Callbacks are copied into a buffer per listener before being invoked, so a callback may unsubscribe during dispatch.
- Dispatch is synchronous, on the calling thread; ordering between listeners follows `Dictionary` enumeration (no guarantee, assumed); no thread safety.
- Listeners are held strongly; they must unsubscribe (typically in `OnDisable`) or they stay registered.
- Editor only: after dispatch, a record is enqueued into a queue capped at `HistorySize` (EditorPrefs key `Fixer33.EasyEvents.HistorySize`, default 15, minimum 1) and `EditorHistoryUpdated` is raised.

## 3. Public API surface

Namespace `EasyEvents`:
- `interface IEvent` - implement on your event type; `Trigger()`.
- `static class IEventExtensions` - `Trigger(this IEvent)`.
- `interface IEventListener` - implement on subscribers (MonoBehaviour or plain class).
- `static class IEventListenerExtensions` - `StartListeningTo<T>(callback)`, `StopListeningTo<T>(callback)`, `RemoveAllListeners()`.
- `delegate EventListenerCallback<in T>(T eventData)`.
- `record EventHistoryRecord` - public but documented in code as editor-only data.

Namespace `EasyEvents.Editor` (Runtime assembly, editor builds only): `EasyEventsEditorBridge` with the `EditorHistoryUpdated` event and `EditorHistory` property.

Typical use:
```csharp
public record PlayerScored(int Id, int Score) : IEvent;

public class Hud : MonoBehaviour, IEventListener
{
    void OnEnable()  => this.StartListeningTo<PlayerScored>(OnScored);
    void OnDisable() => this.StopListeningTo<PlayerScored>(OnScored);
    void OnScored(PlayerScored e) { }
}

new PlayerScored(1, 100).Trigger();
```

Extension points: none beyond implementing the two interfaces; `EventManager` is internal and not replaceable.

## 4. Dependencies

- Declared in `package.json`: `com.unity.test-framework` 1.4.6 (needed only by the test assemblies).
- Asmdef references: runtime none; editor to runtime (GUID `685e7510a74c7d346a821b4def9158ff`); test assemblies to `UnityEngine.TestRunner`, `UnityEditor.TestRunner`, the runtime assembly, plus precompiled `nunit.framework.dll`.
- Define symbols: no `versionDefines`. Code uses `UNITY_EDITOR`; test asmdefs use the constraint `UNITY_INCLUDE_TESTS`. Editor windows also check `PACKAGES_DEV` to choose the stylesheet load path (section 6).
- Implicit: UI Toolkit (`UnityEngine.UIElements`) in editor windows; recent C# features (records, default interface methods, target-typed `new`). The sample uses the legacy `Input` class (`Input.GetMouseButtonUp`), so it needs the old input system enabled (assumed).
- No other com.fixer33.* package is required.

## 5. Editor tools

Menu item **Tools > Fixer33 > Event history** opens `EventHistoryWindow`, titled "Event history". It lists the last 15 fired events (newest first) as `EventPreviewElement` rows with time (`HH:mm:ss`), event type name and a **Details** button. The window refreshes through `EasyEventsEditorBridge.EditorHistoryUpdated`. **Details** opens `EventDetailsWindow` (shown modally), which lists the event's public instance fields as `name: value` using reflection. Styles live in `Editor/GUI/Styles/EventHistoryWindow.uss` and `EventDetailsWindow.uss`.

Limitations seen in code: only public fields are shown (properties are not, so record events with positional properties show nothing); the details header label uses `_record.GetType().Name`, which is the history record's own type ("EventHistoryRecord"), not the event type (the window title does use the event type).

## 6. Samples and tests

Sample "Creating custom events and subscribing" (`Samples~/Creating custom events and subscribing`, imported through Package Manager): `ExampleEvent1` (struct with `RandomNumber`), `RightMouseButtonClick` (parameterless record), `ExampleBehaviour1` (fires `ExampleEvent1` on a timer, default 3 s, and `RightMouseButtonClick` on right mouse release), `ExampleBehaviour2` (listener that logs both), and `Sample.unity`. Namespace `Sample`.

Tests: `Tests/Editor/Base.cs` (NUnit, class `Base`) has three tests: `CallingEvent`, `UnSubscribingFromOneEvent`, `UnSubscribingFromAllEvents`. `Tests/Runtime` contains only an asmdef, no test code. Run from Window > General > Test Runner.

Stylesheet path: windows load `Packages/com.fixer33.easy-events/Editor/GUI/Styles/...`, and prefix `Assets/` when `PACKAGES_DEV` is defined (package embedded under Assets). If the define is absent in such a setup the stylesheet silently does not load (the code null-checks it).

## 7. Consumption

Install from a Git URL via Package Manager. The correct URL (confirmed by the author) is `https://github.com/Fixer33/EasyEventsPackage.git`; the README has been corrected to match. Consumers reference assembly `com.fixer33.easy-events` (auto-referenced, so asmdef-less code sees it; asmdef projects must add the reference).

## 8. Current state

- Finished and functional at 1.0.1. CHANGELOG: 1.0.1 (2025-03-17) fixed an exception on unsubscribing; 1.0.0 (2025-02-19) first version.
- History size is read from EditorPrefs (`Fixer33.EasyEvents.HistorySize`, default 15), as the author wanted. Fixed in source; not yet verified in Unity. There is no GUI control yet.
- Unused: test helper `UnSubscribeFromTwoEvents` is never called; `Tests/Runtime` has no tests.
- Possible rough edges (read, not run): the dispatch buffer is a shared field, so a nested `Trigger()` of the same event type from inside a callback could interfere (assumed); unsubscription matches by delegate hash code only; `RemoveAllListeners` leaves empty entries in the dictionary.

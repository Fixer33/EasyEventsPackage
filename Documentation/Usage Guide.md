# Easy Events - Usage Guide

Package `com.fixer33.easy-events` 1.0.1 ("No more huge EventManager class"). Unity 6000.0+. MIT. Anything marked (assumed) was inferred rather than read directly in the code.

## 1. Installation

### Via com.fixer33.package-installer (preferred)

1. Install `com.fixer33.package-installer` first (see its own README).
2. Open **Window > Fixer33 > Package Installer**.
3. Find **Easy Events** (`com.fixer33.easy-events`) in the **Tools** group and install it. The installer's `packages-list.json` entry for it is:
   - PackageName: `Easy Events`
   - PackageId: `com.fixer33.easy-events`
   - PackageUrl: `https://github.com/Fixer33/EasyEventsPackage.git`

### Via Git URL

**Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/Fixer33/EasyEventsPackage.git
```

The only declared dependency is `com.unity.test-framework` 1.4.6 (needed only by the package's own test assemblies). No other com.fixer33.* package is required.

### Optional sample

Package Manager > Easy Events > Samples > "Creating custom events and subscribing" > Import.

## 2. Minimal setup

There is nothing to create in a scene and no asset to configure. The runtime assembly `com.fixer33.easy-events` (namespace `EasyEvents`) is auto-referenced, so scripts without an asmdef see it immediately. If your code lives in its own asmdef, add a reference to `com.fixer33.easy-events` there.

Add `using EasyEvents;` and you are ready.

## 3. Using the API step by step

### Step 1 - define an event type

An event is any class, struct or record that implements `IEvent`.

```csharp
using EasyEvents;

public struct ExampleEvent1 : IEvent      // from the sample
{
    public int RandomNumber;
}

public record RightMouseButtonClick() : IEvent;   // parameterless event, from the sample
```

### Step 2 - make a listener and subscribe

A listener is any object implementing `IEventListener` (a MonoBehaviour or a plain class). `StartListeningTo<T>` / `StopListeningTo<T>` are extension methods on `IEventListener`, so call them with `this.`:

```csharp
using EasyEvents;
using UnityEngine;

public class ExampleBehaviour2 : MonoBehaviour, IEventListener
{
    private void OnEnable()
    {
        this.StartListeningTo<ExampleEvent1>(OnExampleEvent1);
        this.StartListeningTo<RightMouseButtonClick>(OnRightMouseButtonClick);
    }

    private void OnDisable()
    {
        this.StopListeningTo<ExampleEvent1>(OnExampleEvent1);
        this.StopListeningTo<RightMouseButtonClick>(OnRightMouseButtonClick);
    }

    private void OnExampleEvent1(ExampleEvent1 eventData) =>
        Debug.Log("Random number: " + eventData.RandomNumber);

    private void OnRightMouseButtonClick(RightMouseButtonClick eventData) =>
        Debug.Log("RMB clicked");
}
```

The callback type is `EventListenerCallback<in T>(T eventData) where T : IEvent`.

### Step 3 - trigger the event

Call `Trigger()` on an instance (extension `IEventExtensions.Trigger`, which forwards to the event dispatcher):

```csharp
var evt = new ExampleEvent1 { RandomNumber = 42 };
evt.Trigger();

new RightMouseButtonClick().Trigger();
```

All matching callbacks run synchronously, on the calling thread, before `Trigger()` returns.

### Step 4 - unsubscribe

- One callback: `this.StopListeningTo<T>(callback)` with the same method you subscribed.
- Everything the listener registered: `this.RemoveAllListeners()`.

```csharp
private void OnDestroy() => this.RemoveAllListeners();
```

## 4. Editor windows and menu paths

**Tools > Fixer33 > Event history** opens the window titled "Event history" (`EasyEvents.Editor.GUI.EventHistoryWindow`):

- Header "Called event history", then a scroll list of the last 15 triggered events, newest first.
- Each row (`EventPreviewElement`) shows `[HH:mm:ss] EventTypeName` and a **Details** button.
- **Details** opens `EventDetailsWindow` (modal), titled "`<EventType>` details", listing the event's public instance fields as `name: value` plus the time.
- The list refreshes automatically when an event is recorded (through `EasyEventsEditorBridge.EditorHistoryUpdated`). When the window is opened it only fills itself immediately if the editor is in Play Mode; otherwise it stays empty until the next event is recorded.

Styles are loaded from `Packages/com.fixer33.easy-events/Editor/GUI/Styles/EventHistoryWindow.uss` and `EventDetailsWindow.uss`.

## 5. Configuration and scripting defines

There is no settings asset and no public configuration.

| Item | Where | Effect |
|---|---|---|
| `UNITY_EDITOR` | Runtime code | History recording, `EditorHistoryUpdated` and `EasyEventsEditorBridge` exist only in the editor; none of it is compiled into builds. |
| History size | EditorPrefs key `Fixer33.EasyEvents.HistorySize` (Editor only) | Default `15`, minimum 1. Set it with `EditorPrefs.SetInt`; there is no GUI control yet. Fixed in source; not yet verified in Unity. |
| `PACKAGES_DEV` | Editor windows | When defined, the window stylesheet path gets an `Assets/` prefix (for the package embedded under `Assets/`). Define it only in that situation (assumed). If the package is embedded under `Assets/` and the define is missing, the stylesheet is not found and the windows show unstyled (the code null-checks it, so no error). |
| `UNITY_INCLUDE_TESTS` | Test asmdefs | Standard Unity constraint for the package's tests. |

The package defines no `versionDefines`.

## 6. Extension points and a worked example

The package has no plugin system. `EventManager` is `internal` and cannot be replaced. The extension points are:

1. Your own event types (`IEvent`).
2. Your own listeners (`IEventListener`).
3. In the editor, `EasyEvents.Editor.EasyEventsEditorBridge` exposes `EditorHistory` (`IEnumerable<EventHistoryRecord>`) and the `EditorHistoryUpdated` event, so you can build your own editor tooling.

### Worked example: log every recorded event to a custom editor tool

Put this under an Editor-only folder/asmdef that references `com.fixer33.easy-events` (the bridge lives in the Runtime assembly, under `#if UNITY_EDITOR`):

```csharp
#if UNITY_EDITOR
using System.Linq;
using EasyEvents.Editor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EventLogger
{
    static EventLogger()
    {
        EasyEventsEditorBridge.EditorHistoryUpdated -= OnUpdated;
        EasyEventsEditorBridge.EditorHistoryUpdated += OnUpdated;
    }

    private static void OnUpdated()
    {
        var last = EasyEventsEditorBridge.EditorHistory.LastOrDefault();
        if (last != null)
            Debug.Log($"[{last.Time:HH:mm:ss}] {last.EventData.GetType().Name}");
    }
}
#endif
```

`EventHistoryRecord` exposes `Time` and `EventData`. Remember the history only contains events that had at least one listener (see pitfalls).

## 7. Common pitfalls (current behaviour)

- **Events with no subscribers are not recorded.** `TriggerEvent` returns early when no listener was ever registered for that exact event type, so such events never appear in Event history.
- **Exact-type dispatch.** Callbacks are keyed by `typeof(T)` at subscribe time and matched against `eventData.GetType()` at trigger time. Subscribing to a base class or to an interface never fires for derived events.
- **Always unsubscribe.** Listeners are held strongly in a static dictionary. Unsubscribe in `OnDisable`/`OnDestroy` or call `RemoveAllListeners()`. A destroyed MonoBehaviour that never unsubscribed stays registered and its callback is still invoked (assumed from the code; nothing checks for destroyed objects).
- **`RemoveAllListeners()` leaves empty entries** in the internal dictionaries (harmless, but not cleaned up).
- **Unsubscription matches by delegate hash code only.** Pass the same method (group) you subscribed with. Avoid anonymous lambdas for subscriptions you intend to remove, since you cannot easily pass an identical delegate later.
- **Same callback subscribed twice** is stored twice and runs twice; one `StopListeningTo` removes every entry with that hash.
- **Re-entrant triggers.** The dispatch buffer is a per-event-type shared field. Triggering the same event type from inside one of its callbacks may interfere with the outer dispatch (assumed; no test covers it). Unsubscribing during dispatch is safe because callbacks are copied first.
- **Order and threads.** Listener order is not guaranteed (follows dictionary enumeration, assumed). There is no thread safety; trigger from the main thread. An exception in one callback propagates out of `Trigger()` and skips the remaining callbacks (assumed from the absence of a try/catch).
- **Struct events are boxed** when triggered (they are passed as `IEvent`), so frequent struct events still allocate.
- **No domain-reload handling.** Static state is never reset. With "Enter Play Mode" options that disable domain reload, registrations from a previous play session may persist (assumed; not covered by tests).
- **Details window shows only public fields.** Properties are not listed, so record events with positional (property-based) parameters show no data. Use public fields if you want values in the Details window. Also, the large header label in the Details window shows the history record's own type name (`EventHistoryRecord`) instead of the event type; the window title is correct.
- **Sample uses the legacy `Input` class** (`Input.GetMouseButtonUp`), so it needs the old Input Manager enabled (assumed to fail with the new Input System only).
- **History is editor-only.** `EventHistoryRecord` and the bridge do not exist in builds.

## 8. Working with other com.fixer33.* packages

No other package is required, and the code references none. The package can be installed alongside any of them. Packages in the same family, by name and Git URL:

- `com.fixer33.package-installer` - https://github.com/Fixer33/PackageInstaller.git (URL as given in that package's README; the README URLs of this package family are not always identical to the real remotes, so verify if it fails)

No integration with other packages exists in the code.

## 9. Remaining open questions

Still-open questions from the package's Open Questions that affect usage:

- **Q2** - Events with no subscribers are never shown in Event history. Intended?
- **Q3** - Dispatch uses the exact runtime type, so subscribing to a base type or interface never fires. Intended design?
- **Q4** - Is re-entrant triggering (same event type fired from inside its own callback) supported?
- **Q5** - Unsubscription compares only `callback.GetHashCode()`; are collisions accepted?
- **Q6** - Static state is never reset; is that a concern with domain reload disabled?
- **Q7** - Details window header shows the record type, and only public fields (not properties) are listed. Bug or intended?
- **Q9** - Where is `PACKAGES_DEV` expected to be defined?
- **Q12** - Does the sample work in projects using only the new Input System?

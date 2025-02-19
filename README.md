# Easy Events

![Unity](https://img.shields.io/badge/Unity-UPM%20Package-blue)
![GitHub](https://img.shields.io/github/license/Fixer33/EasyEventsPackage)

Easy Events is a lightweight and flexible event system for Unity, designed to simplify event creation, subscription, and invocation. It provides an easy-to-use API for handling events efficiently in both runtime and editor environments.

## Features

- Simple event creation using `IEvent` interface.
- Easy subscription and unsubscription with `IEventListener`.
- Built-in UI window for debugging, showing the last 15 triggered events and their parameters.
- Sample usage available in Unity's **Samples** section.
- Compatible with Unity's **UPM (Unity Package Manager)**.

## Installation

### Using UPM (Unity Package Manager)

1. Open Unity and go to **Window > Package Manager**.
2. Click the **+** button and select **Add package from git URL**.
3. Enter the repository URL:
   ```
   https://github.com/Fixer33/EasyEvents.git
   ```
4. Click **Add** and wait for Unity to install the package.

## Usage

### Creating an Event
To create an event, define a class, struct, or record that implements `IEvent`. Add public parameters to pass data when the event is triggered.

```csharp
public record PlayerScoredEvent(int PlayerId, int Score) : IEvent
{
    public int PlayerId { get; private set; } = PlayerId;
    public int Score { get; private set; } = Score;
}
```

### Subscribing to an Event
A listener class must implement `IEventListener`. You can subscribe using:

```csharp
public class ScoreListener : MonoBehaviour, IEventListener
{
    private void OnEnable()
    {
        this.StartListeningTo<PlayerScoredEvent>(OnPlayerScored);
    }
    
    private void OnDisable()
    {
        this.StopListeningTo<PlayerScoredEvent>(OnPlayerScored);
    }
    
    private void OnPlayerScored(PlayerScoredEvent e)
    {
        Debug.Log($"Player {e.PlayerId} scored {e.Score} points!");
    }
}
```

### Triggering an Event
Create an instance of the event and call `.Trigger()` to invoke it:

```csharp
new PlayerScoredEvent(1, 100).Trigger();
```

## Debugging Events in Editor
Easy Events provides a UI window to monitor event logs. Open it via:

**Tools > Fixer33 > Event history**

This window displays the last 15 triggered events along with their parameters.

## Samples
**Samples** section includes example scene demonstrating how to use Easy Events. Check them out to get started quickly.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

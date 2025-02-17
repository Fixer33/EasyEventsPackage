namespace EasyEvents
{
    public interface IEventListener
    {
        internal void StartListeningTo<T>(EventListenerCallback<T> callback) where T : IEvent
        {
            EventManager.AddListenerFor<T>(this, callback);
        }
        
        internal void StopListeningTo<T>(EventListenerCallback<T> callback) where T : IEvent
        {
            EventManager.RemoveListenerFor<T>(this, callback);
        }
        
        internal void RemoveAllListeners()
        {
            EventManager.RemoveListenerCompletely(this);
        }
    }

    public delegate void EventListenerCallback<in T>(T eventData) where T : IEvent;

    // ReSharper disable once InconsistentNaming
    public static class IEventListenerExtensions
    {
        public static void StartListeningTo<T>(this IEventListener eventListener, EventListenerCallback<T> callback) where T : IEvent
        {
            eventListener.StartListeningTo<T>(callback);
        }
        
        public static void StopListeningTo<T>(this IEventListener eventListener, EventListenerCallback<T> callback) where T : IEvent
        {
            eventListener.StopListeningTo<T>(callback);
        }
        
        public static void RemoveAllListeners(this IEventListener eventListener)
        {
            eventListener.RemoveAllListeners();
        }
    }
}
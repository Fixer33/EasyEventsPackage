namespace EasyEvents
{
    public interface IEvent
    {
        public void Trigger()
        {
            EventManager.TriggerEvent(this);
        }
    }

    public static class IEventExtensions
    {
        public static void Trigger(this IEvent @event)
        {
            @event.Trigger();
        }
    }
}
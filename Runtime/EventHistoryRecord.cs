using System;

namespace EasyEvents
{
    /// <summary>
    /// Used for editor scripts. Not used in the builds
    /// </summary>
    /// <param name="Time"></param>
    /// <param name="EventData"></param>
    public record EventHistoryRecord(DateTime Time, IEvent EventData)
    {
        public DateTime Time { get; private set; } = Time;
        public IEvent EventData { get; private set; } = EventData;
    }
}
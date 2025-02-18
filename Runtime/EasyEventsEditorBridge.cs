#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace EasyEvents.Editor
{
    public static class EasyEventsEditorBridge
    {
        public static event Action EditorHistoryUpdated = delegate { };
        
        public static IEnumerable<EventHistoryRecord> EditorHistory => EventManager.EditorHistory;

        static EasyEventsEditorBridge()
        {
            EventManager.EditorHistoryUpdated -= OnHistoryUpdated;
            EventManager.EditorHistoryUpdated += OnHistoryUpdated;
        }

        private static void OnHistoryUpdated()
        {
            EditorHistoryUpdated?.Invoke();
        }
    }
}
#endif
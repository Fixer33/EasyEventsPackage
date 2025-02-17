using System;
using System.Collections.Generic;

namespace EasyEvents
{
    internal static class EventManager
    {
        private static readonly Dictionary<Type, EventListenerCallbackDictionary> Dictionary = new();
        
        public static void AddListenerFor<T>(IEventListener listener, EventListenerCallback<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (Dictionary.ContainsKey(type) == false)
                Dictionary.Add(type, new EventListenerCallbackDictionary());
            
            Dictionary[type].AddCallbackData(listener, callback);
        }
        
        public static void RemoveListenerFor<T>(IEventListener listener, EventListenerCallback<T> callback) where T : IEvent
        {
            var type = typeof(T);
            if (Dictionary.ContainsKey(type) == false)
                return;
            
            Dictionary[type].RemoveCallbackData(listener, callback);
        }

        public static void RemoveListenerCompletely(IEventListener listener)
        {
            foreach (var dictRecord in Dictionary)
            {
                dictRecord.Value.RemoveAllListenerCallbacks(listener);
            }
        }

        public static void TriggerEvent<T>(T eventData) where T : IEvent
        {
            var type = eventData.GetType();
            if (Dictionary.ContainsKey(type) == false)
                return;
            
            Dictionary[type].TriggerCallbacks(eventData);
        }

        private class EventListenerCallbackDictionary
        {
            private readonly Dictionary<IEventListener, List<CallbackData>> _callbackDictionary;

            internal EventListenerCallbackDictionary()
            {
                _callbackDictionary = new();
            }

            public void AddCallbackData<T>(IEventListener listener, EventListenerCallback<T> callback) where T : IEvent
            {
                _callbackDictionary.TryAdd(listener, new());
                _callbackDictionary[listener].Add(new CallbackData(callback.GetHashCode(), rawData =>
                {
                    callback?.Invoke((T)rawData);
                }));
            }

            public void RemoveCallbackData<T>(IEventListener listener, EventListenerCallback<T> callback) where T : IEvent
            {
                _callbackDictionary.TryAdd(listener, new());
                _callbackDictionary[listener].RemoveAll(i => i.CallbackHash == callback.GetHashCode());
            }

            public void RemoveAllListenerCallbacks(IEventListener listener)
            {
                if (_callbackDictionary.ContainsKey(listener) == false)
                    return;
                
                _callbackDictionary[listener].Clear();
            }

            public void TriggerCallbacks(IEvent eventData)
            {
                foreach (var callbackRecord in _callbackDictionary)
                {
                    if (callbackRecord.Value == null)
                        continue;

                    // ReSharper disable once PossibleNullReferenceException
                    foreach (var callbackData in callbackRecord.Value)
                    {
                        callbackData.Callback?.Invoke(eventData);
                    }
                }
            }
        }
        
        private record CallbackData(int CallbackHash, EventListenerCallback<IEvent> Callback)
        {
            public int CallbackHash { get; private set; } = CallbackHash;
            public EventListenerCallback<IEvent> Callback { get; private set; } = Callback;
        }
    }
}
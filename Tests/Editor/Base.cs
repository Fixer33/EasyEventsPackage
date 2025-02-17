using System;
using NUnit.Framework;

namespace EasyEvents.EditorTests.Tests.Editor
{
    public class Base
    {
        [Test]
        public void CallingEvent()
        {
            var listener = new TestListener();
            var testEvent = new TestEvent();
            bool callbackWorks = false;
            
            listener.FirstEventCallback = () =>
            {
                callbackWorks = true;
            };
            listener.SubscribeToOneEvent();

            testEvent.Trigger();
            
            Assert.IsTrue(callbackWorks);
        }
        
        [Test]
        public void UnSubscribingFromOneEvent()
        {
            var listener = new TestListener();
            var testEvent = new TestEvent();
            bool callback1Called = false;
            bool callback2Called = false;
            listener.FirstEventCallback = () =>
            {
                callback1Called = true;
            };
            listener.SecondEventCallback = () =>
            {
                callback2Called = true;
            };
            
            listener.SubscribeToTwoEvents();
            listener.UnSubscribeFromOneEvent();
            testEvent.Trigger();
            
            Assert.IsTrue(callback1Called == false && callback2Called);
        }
        
        [Test]
        public void UnSubscribingFromAllEvents()
        {
            var listener = new TestListener();
            var testEvent = new TestEvent();
            bool callback1Called = false;
            bool callback2Called = false;
            listener.FirstEventCallback = () =>
            {
                callback1Called = true;
            };
            listener.SecondEventCallback = () =>
            {
                callback2Called = true;
            };
            
            listener.SubscribeToTwoEvents();
            listener.RemoveAllListeners();
            testEvent.Trigger();
            
            Assert.IsTrue(callback1Called == false && callback2Called == false);
        }
        
        
        private struct TestEvent : IEvent
        {
        
        }
        
        private class TestListener : IEventListener
        {
            public Action FirstEventCallback;
            public Action SecondEventCallback;
        
            public void SubscribeToOneEvent()
            {
                this.StartListeningTo<TestEvent>(Callback);
            }
        
            public void SubscribeToTwoEvents()
            {
                this.StartListeningTo<TestEvent>(Callback);
                this.StartListeningTo<TestEvent>(Callback2);
            }

            public void UnSubscribeFromOneEvent()
            {
                this.StopListeningTo<TestEvent>(Callback);
            }
        
            public void UnSubscribeFromTwoEvents()
            {
                this.StopListeningTo<TestEvent>(Callback);
                this.StopListeningTo<TestEvent>(Callback2);
            }

            private void Callback(TestEvent eventData)
            {
                FirstEventCallback?.Invoke();
            }

            private void Callback2(TestEvent eventData)
            {
                SecondEventCallback?.Invoke();
            }
        }
    }
}

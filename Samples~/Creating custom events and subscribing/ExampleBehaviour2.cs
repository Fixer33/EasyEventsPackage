using EasyEvents;
using UnityEngine;

namespace Sample
{
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

        private void OnExampleEvent1(ExampleEvent1 eventData)
        {
            Debug.Log("Example event 1 callback with random number: " + eventData.RandomNumber);
        }

        private void OnRightMouseButtonClick(RightMouseButtonClick eventData)
        {
            Debug.Log("RMB clicked");
        }
    }
}
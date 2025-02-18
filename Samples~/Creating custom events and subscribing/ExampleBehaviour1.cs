using EasyEvents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sample
{
    public class ExampleBehaviour1 : MonoBehaviour
    {
        [SerializeField] private float _exampleEventCallTime = 3;
        private readonly RightMouseButtonClick _rmbClick = new();
        private ExampleEvent1 _exampleEvent1;
        private float _timer;

        private void Start()
        {
            _timer = _exampleEventCallTime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _timer = _exampleEventCallTime;
                _exampleEvent1.RandomNumber = Random.Range(0, 100);
                _exampleEvent1.Trigger();
            }
            
            if (Input.GetMouseButtonUp(1))
                _rmbClick.Trigger();
        }
    }
}
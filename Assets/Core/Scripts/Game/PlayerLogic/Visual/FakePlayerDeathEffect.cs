using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Game.PlayerLogic.Visual
{
    public sealed class FakePlayerDeathEffect : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> rigidbodies = new();
        [SerializeField] private float maxForce = 10f;

        public void StartEffect(Vector3 transformForward)
        {
            Destroy(gameObject, 3);
            StartCoroutine(AddForceWithDecay(transformForward));
        }

        private IEnumerator AddForceWithDecay(Vector3 transformForward)
        {
            yield return new WaitForEndOfFrame();

            int count = rigidbodies.Count;

            for (int i = 0; i < count; i++)
            {
                float decayFactor = 1f - (float)i / (count - 1);
                float currentForce = maxForce * decayFactor;

                rigidbodies[i].AddForce(transformForward.normalized * currentForce, ForceMode.Impulse);

                Vector3 randomTorque = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

                float torqueMagnitude = currentForce * 0.5f;
                rigidbodies[i].AddTorque(randomTorque * torqueMagnitude, ForceMode.Impulse);

                if (i <= 0) continue;
                float sideOffset = Random.Range(-8f, 8f) * 0.5f;
                Vector3 offsetDirection = Vector3.right * sideOffset;

                rigidbodies[i].AddForce(offsetDirection, ForceMode.Impulse);
            }
        }
    }
}
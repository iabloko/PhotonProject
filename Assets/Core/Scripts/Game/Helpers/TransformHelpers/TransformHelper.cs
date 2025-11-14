using UnityEngine;

namespace Sandbox.Project.Scripts.Helpers.TransformHelpers
{
    public static class TransformHelper
    {
        public static void Decay(this Transform origin, Vector3 a, Vector3 b, float decay, float dt)
        {
            origin.localPosition = b + (a - b) * Mathf.Exp(-decay * dt);
        }
    }
}
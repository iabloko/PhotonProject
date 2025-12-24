using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.NickName
{
    public interface INickNameFadeEffect
    {
        public void Initialization(Camera c);
        public void RegisterNickName(TMP_Text nickName);
        public void UnregisterNickName(TMP_Text nickName);
        public void FixedUpdateNetwork();
    }
    
    public sealed class NickNameFadeEffect : INickNameFadeEffect, IDisposable
    {
        private List<TMP_Text> _names;

        private const float FADE_START = 25f;
        private const float FADE_END = 50f;

        private const int FADE_STEPS = 10;
        private const float SPHERE_RADIUS = 0.1f;

        private CullingGroup _cullingGroup;
        private BoundingSphere[] _spheres;

        private float[] _distances;

        public NickNameFadeEffect()
        {
            _names = new List<TMP_Text>();
        }

        void INickNameFadeEffect.Initialization(Camera c)
        {
            if (c == null) return;
            if (_cullingGroup != null) return;

            CreateCullingGroup(c);
            BuildDistanceBands();

            if (_names.Count > 0)
                RebuildSpheres_NoAlloc();
        }

        void INickNameFadeEffect.RegisterNickName(TMP_Text nickName)
        {
            if (!nickName) return;

            CompactNullNicknames();

            if (_names.Contains(nickName)) return;

            _names.Add(nickName);

            if (_cullingGroup != null)
                RebuildSpheres_NoAlloc();
        }

        void INickNameFadeEffect.UnregisterNickName(TMP_Text nickName)
        {
            if (!nickName) return;

            int idx = _names.IndexOf(nickName);
            if (idx < 0) return;

            _names.RemoveAt(idx);

            if (_cullingGroup != null)
                RebuildSpheres_NoAlloc();
        }
        
        void INickNameFadeEffect.FixedUpdateNetwork()
        {
            if (_spheres == null) return;

            if (CompactNullNicknames()) RebuildSpheres_NoAlloc();

            int count = _names.Count;
            if (_spheres.Length < count) count = _spheres.Length;
            for (int i = 0; i < count; i++)
            {
                TMP_Text t = _names[i];
                if (!t) continue;
                _spheres[i].position = t.transform.position;
            }
        }

        void IDisposable.Dispose()
        {
            if (_cullingGroup != null)
            {
                _cullingGroup.onStateChanged -= OnStateChanged;
                _cullingGroup.Dispose();
                _cullingGroup = null;
            }

            _spheres = null;
            _names.Clear();
        }

        private bool CompactNullNicknames()
        {
            bool changed = false;
            for (int i = _names.Count - 1; i >= 0; i--)
            {
                if (!_names[i])
                {
                    _names.RemoveAt(i);
                    changed = true;
                }
            }

            return changed;
        }

        private void RebuildSpheres_NoAlloc()
        {
            int n = _names.Count;

            if (_spheres == null || _spheres.Length < n)
                _spheres = new BoundingSphere[n];

            for (int i = 0; i < n; i++)
            {
                var t = _names[i];

                if (!t) continue;
                _spheres[i].position = t.transform.position;
                _spheres[i].radius = SPHERE_RADIUS;
            }

            _cullingGroup.SetBoundingSpheres(_spheres);
            _cullingGroup.SetBoundingSphereCount(n);
        }

        private void CreateCullingGroup(Camera camera)
        {
            _cullingGroup = new CullingGroup { targetCamera = camera };
            _cullingGroup.SetDistanceReferencePoint(camera.transform);
            _cullingGroup.onStateChanged += OnStateChanged;
        }

        private void BuildDistanceBands()
        {
            _distances = new float[FADE_STEPS + 1];
            for (int i = 0; i <= FADE_STEPS; i++)
                _distances[i] = Mathf.Lerp(FADE_START, FADE_END, (float)i / FADE_STEPS);
            _cullingGroup.SetBoundingDistances(_distances);
        }

        private void RebuildSpheres()
        {
            if (!CompactNullNicknames() && (_names == null || _names.Count == 0))
            {
                _spheres = Array.Empty<BoundingSphere>();
                
                if (_cullingGroup == null) return;
                
                _cullingGroup.SetBoundingSpheres(_spheres);
                _cullingGroup.SetBoundingSphereCount(0);
                _cullingGroup.enabled = false;

                return;
            }

            int aliveCount = _names.Count(t => t);

            if (aliveCount == 0)
            {
                _spheres = Array.Empty<BoundingSphere>();

                if (_cullingGroup == null) return;

                _cullingGroup.SetBoundingSpheres(_spheres);
                _cullingGroup.SetBoundingSphereCount(0);
                _cullingGroup.enabled = false;

                return;
            }

            if (_spheres == null || _spheres.Length != aliveCount)
                _spheres = new BoundingSphere[aliveCount];

            int j = 0;

            foreach (TMP_Text t in _names.Where(t => t))
            {
                _spheres[j] = new BoundingSphere(t.transform.position, SPHERE_RADIUS);
                j++;
            }

            if (_cullingGroup == null) return;

            _cullingGroup.SetBoundingSpheres(_spheres);
            _cullingGroup.SetBoundingSphereCount(aliveCount);
            _cullingGroup.enabled = true;
        }
        
        private void OnStateChanged(CullingGroupEvent evt)
        {
            if (evt.index < 0 || evt.index >= _names.Count) return;

            TMP_Text tmp = _names[evt.index];

            if (tmp == null) return;

            float alpha = 1f - Mathf.Clamp01(evt.currentDistance / (float)FADE_STEPS);
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }
    }
}
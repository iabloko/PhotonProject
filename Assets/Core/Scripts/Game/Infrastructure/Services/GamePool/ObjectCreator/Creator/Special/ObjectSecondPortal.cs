using System;
using System.Collections.Generic;
using System.Threading;
using Core.Scripts.Game.Infrastructure.Services.AssetProviderService;
using Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base;
using Core.Scripts.Game.InteractionObjects;
using Core.Scripts.Game.InteractionObjects.Base;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Special
{
    public sealed class ObjectSecondPortal : PortalCreator
    {
        private SecondPortal _secondPortal;

        public ObjectSecondPortal(NetworkObject reference, ref Dictionary<string, Portal> portals) :
            base(reference, portals)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.PORTAL_SECOND_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectSecondPortal error, prefabId: {prefabId} : created object is {AssetPaths.PORTAL_SECOND_ID}");
                return;
            }
            
            int portalPrefix = GetPortalID(properties);

            _secondPortal = await CreateSpecialObject<SecondPortal>(Reference, p, r, s, parent);

            _secondPortal.UpdatePortalKey(portalPrefix);

            try
            {
                Portals.Add(_secondPortal.PortalKey, _secondPortal);
            }
            catch (Exception e)
            {
                Debug.LogError($"ObjectSecondPortal ERROR Properties {properties}");
                Debug.LogError($"ObjectSecondPortal ERROR {e}");
                Debug.LogError($"ObjectSecondPortal ERROR {e.Message}");
                return;
            }

            SharePortals(_secondPortal, portalPrefix, _secondPortal.PortalKey);
        }
    }
}
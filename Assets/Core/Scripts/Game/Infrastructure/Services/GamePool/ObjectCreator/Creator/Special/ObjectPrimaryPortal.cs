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
    public sealed class ObjectPrimaryPortal : PortalCreator
    {
        private PrimaryPortal _primaryPortal;

        public ObjectPrimaryPortal(NetworkObject reference, ref Dictionary<string, Portal> portals) :
            base(reference, portals)
        {
        }

        public override async UniTask Create(string prefabId, Vector3 p, Quaternion r,
            Vector3 s, Transform parent, string properties, CancellationTokenSource cts)
        {
            if (!prefabId.Equals(AssetPaths.PORTAL_PRIMARY_ID, StringComparison.Ordinal))
            {
                Debug.LogError(
                    $"ObjectPrimaryPortal error, prefabId: {prefabId} : created object is {AssetPaths.PORTAL_PRIMARY_ID}");
                return;
            }
            
            int portalPrefix = GetPortalID(properties);

            _primaryPortal = await CreateSpecialObject<PrimaryPortal>(Reference, p, r, s, parent);
            _primaryPortal.UpdatePortalKey(portalPrefix);

            try
            {
                Portals.Add(_primaryPortal.PortalKey, _primaryPortal);
            }
            catch (Exception e)
            {
                Debug.LogError($"ObjectPrimaryPortal Properties {properties}");
                Debug.LogError($"ObjectPrimaryPortal ERROR {e}");
                Debug.LogError($"ObjectPrimaryPortal ERROR {e.Message}");
                return;
            }

            SharePortals(_primaryPortal, portalPrefix, _primaryPortal.PortalKey);
        }
    }
}
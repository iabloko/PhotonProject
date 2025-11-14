using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Game.InteractionObjects.Base;
using Fusion;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Core.Scripts.Game.Infrastructure.Services.GamePool.ObjectCreator.Creator.Base
{
    public abstract class PortalCreator : SpecialObjectCreator
    {
        private const string PORTAL_ID_KEY = "PortalID";

        protected readonly Dictionary<string, Portal> Portals;

        protected PortalCreator(NetworkObject reference, Dictionary<string, Portal> portals) : base(reference)
        {
            Portals = portals;
        }

        protected int GetPortalID(string properties)
        {
            if (string.IsNullOrEmpty(properties)) return 0;

            try
            {
                JObject json = JObject.Parse(properties);
                JToken portalIdToken = json[PORTAL_ID_KEY];
                
                if (portalIdToken != null && int.TryParse(portalIdToken.ToString(), out int portalId))
                {
                    return portalId;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetPortalID ERROR {ex} {ex.Message}");
            }

            return 0;
        }

        protected void SharePortals(Portal parentPortal, int portalPrefix, string exceptionKey)
        {
            foreach (var portal in 
                     Portals.Where(portal =>
                         portal.Value.PortalID.Equals(portalPrefix) &&
                         portal.Value.PortalKey != exceptionKey))
            {
                parentPortal.nextPortal = portal.Value;
                portal.Value.nextPortal = parentPortal;
            }
        }
    }
}
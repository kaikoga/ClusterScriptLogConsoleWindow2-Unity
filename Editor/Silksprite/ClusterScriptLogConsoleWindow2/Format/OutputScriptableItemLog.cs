using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Format
{
    /// <summary>
    /// ref: https://docs.cluster.mu/creatorkit/world/testing/log/
    /// </summary>
    [PublicAPI]
    [Serializable]
    public class OutputScriptableItemLog
    {
        [SerializeField] public double tsdv;
        [SerializeField] public string dvid;
        [SerializeField] public OutputItemInfo origin;
        [SerializeField] public OutputPlayerInfo player;
        [SerializeField] public string type;
        [SerializeField] public string message;
        [SerializeField] public string kind;
    }
    
    [PublicAPI]
    [Serializable]
    public struct OutputItemInfo
    {
        [SerializeField] public string name;
        [SerializeField] public ulong id;
    }

    [PublicAPI]
    [Serializable]
    public struct OutputPlayerInfo
    {
        [SerializeField] public string id;
        [SerializeField] public string userName;
    }
}

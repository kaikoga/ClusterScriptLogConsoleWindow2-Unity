using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Silksprite.ClusterScriptLogConsoleWindow2.Format
{
    [PublicAPI]
    [Serializable]
    public class OutputScriptableItemLogExt : OutputScriptableItemLog
    {
        /// <summary>
        /// NOTE: optional, current location, [one-based line number, one-based column number]
        /// </summary>
        [SerializeField] public int[] pos = { };
        /// <summary>
        /// NOTE: optional, stack information of JavaScript
        /// </summary>  
        [SerializeField] public OutputStackItemExt[] stack = { };
    }
    
    [PublicAPI]
    [Serializable]
    public struct OutputStackItemExt
    {
        /// <summary>
        /// NOTE: required, [one-based line number, one-based column number]
        /// </summary>
        [SerializeField] public int[] pos;
        /// <summary>
        /// NOTE: optional
        /// </summary>
        [SerializeField] public string info;
    }
}

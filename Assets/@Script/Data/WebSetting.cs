using MewVivor.Enum;
using UnityEngine;

namespace MewVivor.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WebSettings", order = 1)]
    public class WebSettings : ScriptableObject
    {
        public Environment environment;
        public string devIpAddress;
        public string liveIpAddress;
        public string qaIpAddress;
    }
}
using UnityEngine;

namespace MewVivor.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ConfigurationSetting", order = 1)]
    public class ConfigurationSetting : ScriptableObject
    {
        public string webClientId;
        public string AesKey;
        public string AesIV;
    }
}
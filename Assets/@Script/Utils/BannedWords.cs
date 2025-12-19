using System.Text.RegularExpressions;
using UnityEngine;

namespace MewVivor.Util
{
    public static class BannedWords
    {
        public static bool IsCanUseNickName(string nickName)
        {
            return !IsSpecialChar(nickName) && !IsIllegalChar(nickName) && nickName.Length >= 3;
        }

        public static bool IsSpecialChar(string target)
        {
            return !Regex.IsMatch(target, @"^[\w]*$");
        }

        public static bool IsIllegalChar(string target)
        {
            string data = Manager.I.Data.BanWordDataList.Find(v=> v == target);
            return string.IsNullOrEmpty(data);
        }
    }
}
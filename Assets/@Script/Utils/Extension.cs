using System;
using System.Collections;
using System.Collections.Generic;
using MewVivor;
using MewVivor.Enum;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{

    public static bool IsValid(this CreatureController bo)
    {
        if (bo == null || bo.isActiveAndEnabled == false)
            return false;

        CreatureController creature = bo as CreatureController;
        if (creature != null)
            return creature.CreatureStateType != CreatureStateType.Dead;

        return true;
    }

    // public static void AddLayer(this ref LayerMask mask, Layer layer)
    // {
    //     mask |= (1 << (int)layer);
    // }
    //
    // public static void RemoveLayer(this ref LayerMask mask, Define.ELayer layer)
    // {
    //     mask &= ~(1 << (int)layer);
    // }

    public static void DestroyChilds(this GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            Manager.I.Pool.ReleaseObject(go.name, child.gameObject);
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]); //swap
        }
    }
}
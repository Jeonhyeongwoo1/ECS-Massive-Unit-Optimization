using System;
using System.Collections.Generic;
using MewVivor.Interface;

namespace MewVivor.Factory
{
    public static class ModelFactory
    {
        private static readonly Dictionary<Type, IModel> _modelDict = new();

        public static T CreateOrGetModel<T>() where T : IModel, new ()
        {
            if (!_modelDict.TryGetValue(typeof(T), out var model))
            {
                model = new T();
                _modelDict.Add(typeof(T), model);
                return (T) model;
            }

            return (T)model;
        }

        public static void Clear()
        {
            _modelDict.Clear();
        }

    }
}
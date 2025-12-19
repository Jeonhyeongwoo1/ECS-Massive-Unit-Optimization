// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using MewVivor.Attribute;
// using MewVivor.Firebase;
// using MewVivor.Interface;
// using MewVivor.Managers;
// using MewVivor.Server;
// using UnityEngine;
//
// namespace MewVivor.Factory
// {
//     public class ServerHandlerFactory
//     {
//         private static readonly Dictionary<Type, IClientSender> ServerRequestHandlerDict = new();
//
//         public static T Create<T>(params object[] injectParams) where T : IClientSender
//         {
//             if (!ServerRequestHandlerDict.TryGetValue(typeof(T), out var handler))
//             {
//                 var interfaces = typeof(T).GetInterfaces()
//                     .Where(x => x.GetCustomAttribute<ClientSenderAttribute>() != null);
//                 handler = (T)Activator.CreateInstance(typeof(T), injectParams);
//                 foreach (var @interface in interfaces)
//                 {
//                     ServerRequestHandlerDict.Add(@interface, handler);
//                 }
//             }
//
//             return (T)handler;
//         }
//
//         public static T Get<T>() where T : class, IClientSender
//         {
//             if (ServerRequestHandlerDict.TryGetValue(typeof(T), out var handler))
//             {
//                 return (T)handler;
//             }
//
//             Debug.LogError($"Failed Get {typeof(T)}");
//             return null;
//         }
//
//         public static void ClearDict()
//         {
//             ServerRequestHandlerDict.Clear();
//         }
//
//         public static void InitializeServerHandlerRequest(FirebaseController firebaseController, DataManager dataManager)
//         {
//             ClearDict();
//             Create<ServerUserRequestHandler>(firebaseController, dataManager);
//             Create<ServerEquipmentRequestHandler>(firebaseController, dataManager);
//             Create<ServerShopRequestHandler>(firebaseController, dataManager);
//             Create<ServerCheckoutRequestHandler>(firebaseController, dataManager);
//             Create<ServerMissionRequestHandler>(firebaseController, dataManager);
//             Create<ServerAchievementRequestHandler>(firebaseController, dataManager);
//             Create<ServerOfflineRewardRequestHandler>(firebaseController, dataManager);
//         }
//     }
// }
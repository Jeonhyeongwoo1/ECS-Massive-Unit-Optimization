using System;
using Cysharp.Threading.Tasks;
using MewVivor.Data.Server;
using MewVivor.Enum;
using MewVivor.Factory;
using MewVivor.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MewVivor.Controller
{
    public class BaseSceneController : MonoBehaviour
    {
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                string sceneName = SceneManager.GetActiveScene().name;
                if (sceneName == SceneType.TitleScene.ToString() || sceneName == SceneType.GameScene.ToString())
                {
                    return;
                }
                
                RequestAuthResume().Forget();
            }
        }

        protected async UniTask RequestAuthResume()
        {
            UserModel userModel = ModelFactory.CreateOrGetModel<UserModel>();
            var response = await Manager.I.Web.SendRequest<AuthResumeResponseData>("/auth/resume", null);
            if (response.statusCode != (int)ServerStatusCodeType.Success)
            {
                Debug.LogError($"Failed error {response.message}");
                return;
            }

            userModel.SetUserInfiniteTicketAndStamina(response.data.infiniteTicket, response.data.stamina);
        }
    }
}
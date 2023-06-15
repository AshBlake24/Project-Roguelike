using System;
using Agava.YandexGames;

namespace Roguelike.UI.Windows.Confirmations
{
    public class AuthorizationWindow : ConfirmationWindow
    {
        protected override void Initialize()
        {
            
#if UNITY_WEBGL && !UNITY_EDITOR
            if (YandexGamesSdk.IsInitialized)
                Destroy(gameObject);
#endif
        }

        protected override void OnConfirm()
        {
            base.OnConfirm();
#if UNITY_WEBGL && !UNITY_EDITOR
            if (PlayerAccount.IsAuthorized)
                throw new ArgumentNullException(nameof(PlayerAccount), "Account has already authorized");

            PlayerAccount.Authorize();
#endif
        }
    }
}
using AlphabetUpdate.Client;
using AuthYouClient.ProcessInteractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthYouClient
{
    public static class AuthYouExtensions
    {
        public static void AddAuthYouInteractor(this LauncherCoreBuilder builder, AuthYouProcessInteractor interactor)
        {
            builder.AddProcessInteractor(interactor);
        }

        public static void AddAuthYouInteractor(this LauncherCoreBuilder builder, AuthYouClientCore client)
        {
            builder.AddProcessInteractor(new AuthYouProcessInteractor(client));
        }

        public static void AddAuthYouInteractor(this LauncherCoreBuilder builder, AuthYouClientSettings clientSettings)
        {
            builder.AddProcessInteractor(
                new AuthYouProcessInteractor(
                    new AuthYouClientCore(clientSettings)));
        }
    }
}

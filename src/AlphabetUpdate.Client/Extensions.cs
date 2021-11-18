using AlphabetUpdate.Common.Models;
using CmlLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphabetUpdate.Client
{
    public static class Extensions
    {
        public static void UseDirectServerConnection(this LauncherCoreBuilder builder, string ip, int port)
        {
            builder.AddLaunchOptionAction((MLaunchOption launchOption) =>
            {
                launchOption.ServerIp = ip;
                launchOption.ServerPort = port;
            });
        }

        public static void UseDirectServerConnection(this LauncherCoreBuilder builder, string ipport)
        {
            var ipspl = ipport.Split(':');
            if (ipspl.Length != 0)
            {
                if (ipspl.Length == 2)
                    builder.UseDirectServerConnection(ipspl[0], int.Parse(ipspl[1]));
                else
                    builder.UseDirectServerConnection(ipspl[0], 25565);
            }
        }
    }
}

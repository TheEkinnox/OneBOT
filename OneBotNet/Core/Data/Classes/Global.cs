#region MÉTADONNÉES

// Nom du fichier : Global.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-27
// Date de modification : 2019-03-01

#endregion

#region USING

using Discord.WebSocket;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    public static class Global
    {
        #region PROPRIÉTÉS ET INDEXEURS

        internal static DiscordSocketClient Client { get; set; }

        #endregion
    }
}
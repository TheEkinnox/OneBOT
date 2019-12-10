﻿#region MÉTADONNÉES

// Nom du fichier : Program.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-27
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OneBotNet.Core.Commands;
using OneBotNet.Core.Data.Classes;

#endregion

namespace OneBotNet
{
    public class Program
    {
        #region ATTRIBUTS

        /// <summary>
        /// Client discord (ici, le bot)
        /// </summary>
        private DiscordSocketClient _client;

        /// <summary>
        /// Attribut permettant l'utilisation du service de commandes discord.net
        /// </summary>
        private CommandService _commands;

        #endregion

        #region MÉTHODES

        /// <summary>
        /// Version synchrone de la méthode MainAsync()
        /// </summary>
        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Méthode Main du programme (asynchrone pour pouvoir être utilisée dans discord)
        /// </summary>
        /// <returns></returns>
        private async Task MainAsync()
        {
            Console.Title = $"OneBot V{Config.Version}";
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            this._commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info
            });

            this._client.MessageReceived += Client_MessageReceived;
            await this._commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            this._client.Log += Client_Log;
            this._client.Ready += Client_Ready;
            this._client.UserJoined += AnnounceUserJoined;

            string token;
            using (FileStream stream = new FileStream(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Token.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader readToken = new StreamReader(stream))
            {
                token = readToken.ReadToEnd();
            }

            await this._client.LoginAsync(TokenType.Bot, token);
            await this._client.StartAsync();
            Global.Client = this._client;
            await Task.Delay(-1);
        }

        /// <summary>
        /// Envoie un message de bienvenue
        /// </summary>
        /// <param name="user">Utilisateur qui a rejoins(destinataire du message)</param>
        /// <returns></returns>
        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            SocketGuild guild = user.Guild;
            SocketTextChannel channel = guild.SystemChannel;
            await channel.SendMessageAsync(Config.WelcomeMessage + " " + user.Mention);
        }

        /// <summary>
        /// Affiche les logs à la Logs
        /// </summary>
        /// <param name="message">Message de log</param>
        private async Task Client_Log(LogMessage message)
        {
            Logs.WriteLine($"at {message.Source} {message.Message}");
        }

        /// <summary>
        /// Actions à effectuer lorsque le client est prêt
        /// </summary>
        /// <returns></returns>
        private async Task Client_Ready()
        {
            await this._client.SetGameAsync($"{Config.PrefixPrim} {Config.PrefixSec}");
            await RepeatingTimer.StartTimer();
            Global.ChannelsBanques = new List<SocketTextChannel>
            {
                //OPNO
                Global.Client.GetGuild(549301561478873105).GetTextChannel(551091430152470528),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(551490991127920653)
            };
            Global.ChannelsPrime = new List<SocketTextChannel>
            {
                //OPNO
                Global.Client.GetGuild(549301561478873105).GetTextChannel(550370804878278666),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(552220317011935260)
            };
            Global.ChannelsReput = new List<SocketTextChannel>
            {
                //OPNO
                Global.Client.GetGuild(549301561478873105).GetTextChannel(619146742453108739),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(623706699332845579)
            };
            Global.StuffLists = new List<SocketTextChannel>
            {
                //OPNO
                Global.Client.GetGuild(549301561478873105).GetTextChannel(553713542721962004),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(557201110566174743)
            };
        }

        private async Task Client_MessageReceived(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(this._client, message);
            Global.Context = context;

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int argPos = 0;
            if (!(message.HasStringPrefix(Config.PrefixPrim, ref argPos) || message.HasStringPrefix(Config.PrefixSec, ref argPos))) return;

            IResult result = await this._commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess)
            {
                Logs.WriteLine($"at Commands] Une erreur s'est produite en exécutant une commande. Texte: {context.Message.Content} | Erreur: {result.ErrorReason}");
            }

            if (!result.IsSuccess && result.ErrorReason.Contains("Unknown command") && context.Message.Content != Config.PrefixPrim && context.Message.Content != Config.PrefixSec && context.Message.Content != Config.PrefixPrim + "exit" && context.Message.Content != Config.PrefixSec + "exit")
            {
                await context.Channel.SendMessageAsync($"La commande **{context.Message.Content}** n'existe pas");
            }
        }

        #endregion
    }
}
#region MÉTADONNÉES

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
        static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Méthode Main du programme (asynchrone pour pouvoir être utilisée dans discord)
        /// </summary>
        /// <returns></returns>
        private async Task MainAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            this._commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
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
        /// Méthode permettant d'ajouter le salaire défini pour un personnage au dit personnage
        /// </summary>
        public static async Task VerserSalaireAsync(BankAccount bankAccount)
        {
            string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");
            BankAccount methodes = new BankAccount("");
            List<BankAccount> bankAccounts = await methodes.ChargerDonneesPersosAsync(nomFichier);
            if (bankAccount != null)
            {
                string bankName = bankAccount.Name;
                decimal bankSalaire = bankAccount.Salaire;
                ulong bankUserId = bankAccount.UserId;
                decimal ancienMontant = bankAccount.Amount;
                decimal nvMontant = ancienMontant + bankSalaire;
                bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, bankName));
                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                BankAccount newAccount = new BankAccount(bankName, nvMontant, bankUserId, bankSalaire);
                bankAccounts.Add(newAccount);
                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                Logs.WriteLine($"Salaire de {bankName} ({bankSalaire} couronnes) versé");
                Logs.WriteLine(newAccount.ToString());
            }
        }

        /// <summary>
        /// Mise à jour des channels banque
        /// </summary>
        public static async Task UpdateBank(SocketTextChannel[] banques)
        {
            try
            {
                BankAccount methodes = new BankAccount("");
                string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");


                foreach (SocketTextChannel banque in banques)
                {
                    foreach (IMessage message in await banque.GetMessagesAsync().FlattenAsync())
                    {
                        await message.DeleteAsync();
                    }

                    foreach (string msg in await methodes.AccountsListAsync(nomFichier))
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await banque.SendMessageAsync(msg);
                            Logs.WriteLine(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
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
            string guildName = guild.Name;
            if (guildName == "ServeurTest")
            {
                //await this.Context.Channel.SendMessageAsync("Bienvenue sur Alternia " + this.Context.User.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#" + GetChannelByName("contexte-rp", guildName) + ">\n<#" + GetChannelByName("geographie-de-alternia", guildName) + ">\n" + GetChannelByName("banque", guildName) + "\n" + GetChannelByName("regles", guildName) + "\n" + GetChannelByName("liens-utiles", guildName) + "\n" + GetChannelByName("fiche-prototype", guildName) + "\n" + GetChannelByName("les-races-disponibles", guildName) +
                //                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + this.Context.Guild.GetRole(541492279894999080).Mention + "!", false, null, null);
                await channel.SendMessageAsync("Bienvenue sur Alternia " + user.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#542072451324968972>\n<#542070741504360458>\n<#541493264180707338>\n<#542070805236940837>\n<#542072285033660437>\n<#542073013722546218>\n<#542073051790049302>" +
                                               "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + guild.GetRole(541492279894999080).Mention + " !");
            }
            else
            {
                await channel.SendMessageAsync("Bienvenue sur Alternia " + user.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#410438433849212928>\n<#410531350102147072>\n<#411969883673329665>\n<#409789542825197568>\n<#409849626988904459>\n<#410424057050300427>\n<#410487492463165440>" +
                                               "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + guild.GetRole(420536907525652482).Mention + " !");
            }
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
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task Client_Ready()
        {
            await this._client.SetGameAsync("prefix: a! ^^ @AlterBot");
            await RepeatingTimer.StartTimer();
        }

        private async Task Client_MessageReceived(SocketMessage messageParam)
        {
            SocketUserMessage message = messageParam as SocketUserMessage;
            SocketCommandContext context = new SocketCommandContext(this._client, message);

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int argPos = 0;
            if (!(message.HasStringPrefix("^^", ref argPos) || message.HasStringPrefix("a!", ref argPos) || message.HasMentionPrefix(this._client.CurrentUser, ref argPos))) return;

            IResult result = await this._commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess)
            {
                Logs.WriteLine($"at Commands] Une erreur s'est produite en exécutant une commande. Texte: {context.Message.Content} | Erreur: {result.ErrorReason}");
            }

            if (!result.IsSuccess && result.ErrorReason.Contains("Unknown command"))
            {
                await context.Channel.SendMessageAsync($"La commande **{context.Message.Content}** n'existe pas");
            }
        }

        #endregion
    }
}
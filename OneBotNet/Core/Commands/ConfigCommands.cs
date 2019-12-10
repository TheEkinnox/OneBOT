using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using OneBotNet.Core.Commands;

namespace OneBotNet.Core.Data.Classes
{
    public class ConfigCommands : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        [Command("config"), Alias("cfg", "settings", "stgs"), Summary("Commande de gestion de la configuration du bot")]
        public async Task SendConfig(string option = "none", [Remainder] string input = "none")
        {
            try
            {
                if (Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                {
                    switch (option.ToLower())
                    {
                        case "show":
                            string motd = String.IsNullOrWhiteSpace(Config.Motd) ? "Aucun" : Config.Motd;
                            await ReplyAsync($"**- Configuration Actuelle -**\n__Version :__\n{Config.Version}\n__Prefix :__\n*{Config.PrefixPrim}* ou *{Config.PrefixSec}*\n__Message du jour :__\n{motd}\n__Message de bienvenue: __\n{Config.WelcomeMessage}");
                            break;
                        case "setversion":
                        case "setver":
                            if (Global.HasRole(this.Context.User as SocketGuildUser, "Fondateur"))
                            {
                                if (input == "none")
                                {
                                    await ReplyAsync("Veuillez entrer un numéro de version valide...");
                                    throw new ArgumentNullException(null, "Aucun message de bienvenue n'a été entré.");
                                }
                                else
                                    Config.Version = input;
                            }
                            else
                            {
                                await ReplyAsync("Seul TheEkinnox est autorisé à modifier la version du Bot...");
                                throw new InvalidOperationException($"{this.Context.User.Username} a tenté de modifier la version du bot mais n'a pas les autorisations nécessaires.");
                            }

                            break;
                        case "setprimpref":
                        case "setprefix":
                        case "setpp":
                            if (input == "none")
                            {
                                await ReplyAsync("Veuillez entrer un préfix...");
                                throw new ArgumentNullException(null, "Aucun prefix n'a été entré.");
                            }
                            else if (input.Length > 2)
                            {
                                await ReplyAsync("Veuillez entrer un préfix plus court (2 charactères max)...");
                                throw new ArgumentException($"La longueure du prefix entré par {this.Context.User.Username} est superieur à 2 charactères.");
                            }
                            else
                                Config.PrefixPrim = input;

                            break;
                        case "setsecpref":
                        case "setsprefix":
                        case "setsp":
                            if (input == "none")
                            {
                                await ReplyAsync("Veuillez entrer un préfix...");
                                throw new ArgumentNullException(null, "Aucun prefix n'a été entré.");
                            }
                            else if (input.Length > 2)
                            {
                                await ReplyAsync("Veuillez entrer un préfix plus court (2 charactères max)...");
                                throw new ArgumentException($"La longueure du prefix entré par {this.Context.User.Username} est superieur à 2 charactères.");
                            }
                            else
                                Config.PrefixSec = input;

                            break;
                        case "setmotd":
                        case "chmotd":
                            if (input == "none")
                                Config.Motd = "";
                            else
                                Config.Motd = input;
                            break;
                        case "setwelcome":
                        case "setwc":
                        case "chwc":
                        case "chwelcome":
                            if (input == "none")
                            {
                                await ReplyAsync("Veuillez définir un message à envoyer lorsqu'un utilisateur rejoint le serveur...");
                                throw new ArgumentNullException(null, "Aucun message de bienvenue n'a été entré.");
                            }
                            else
                                Config.WelcomeMessage = input;
                            break;
                        case "none":
                            await ReplyAsync("Veuillez entrer une option pour utiliser cette commande...");
                            throw new ArgumentNullException(null, "Aucune option n'a été entrée.");
                        default:
                            await ReplyAsync("L'option entrée est invalide. Veuillez réessayer.");
                            throw new ArgumentException("L'option entrée est invalide.");
                    }

                    Logs.WriteLine($"La commande config {option} à été utilisée par {this.Context.User.Username} (UserID:{this.Context.User.Id}) avec succès.");
                    await ReplyAsync($"La commande config {option} à été utilisée par {this.Context.User.Username} avec succès.");
                }
                else
                {
                    await ReplyAsync("Vous devez être Admin pour utiliser les commandes config...");
                    throw new InvalidOperationException($"{this.Context.User.Username} a tenté de modifier la configuration du bot mais n'a pas les autorisations nécessaires.");
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine("Une erreur s'est produite lors de l'utilisation de la commande config avec le message suivant : " + e.Message);
                throw;
            }
        }

        #endregion
    }
}

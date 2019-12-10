#region MÉTADONNÉES

// Nom du fichier : BankCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-08
// Date de modification : 2019-09-16

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OneBotNet.Core.Data.Classes;

#endregion

namespace OneBotNet.Core.Commands
{
    public class BankCommands : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private readonly Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("bank"), Alias("bnk", "money", "bk"), Summary("Affiche l'argent en banque d'un utilisateur")]
        public async Task SendBank([Remainder] string input = "none")
        {
            //SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
            string[] argus;
            string error = "Valeur invalide, bank help pour plus d'information.";
            string message = "";

            try
            {
                if (input != "none")
                {
                    await Global.ChargerDonneesPersosAsync();
                    // ====================================
                    // = Gestion de la commande bank help =
                    // ====================================
                    if (input == "help")
                    {
                        string staff = "";
                        message += "Aide sur la commande: `bank help`\n";
                        message += "Actualiser le channel bank: `bank update`\n";
                        message += "Afficher le montant sur le compte d'un personnage: `bank info (nom_Personnage)`\n";
                        staff += "(staff) Ajouter de l'argent sur le compte d'un personnage: `bank deposit (montant) (nom_Personnage)`\n";
                        staff += "(staff) Retirer de l'argent sur le compte d'un personnage: `bank withdraw (montant) (nom_Personnage)`\n";
                        staff += "(staff) Définir le montant sur le compte d'un personnage: `bank set (montant) (nom_Personnage)`\n";
                        message += "Transférer de l'argent d'un compte à un autre: `bank pay (montant) (nom_Personnage1) (nom_Personnage2)`\n";
                        staff += "(staff) Définir le salaire d'un personnage: `bank sts (nom_personnage)`\n";
                        try
                        {
                            await ReplyAsync("Aide envoyée en mp");
                            Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                            EmbedBuilder eb = new EmbedBuilder();
                            eb.WithTitle("**Aide de la commande bank (bnk,money)**")
                                .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                                .AddField("========== Commandes Staff ==========", staff)
                                .AddField("========== Commandes Publiques ==========", message);
                            await this.Context.User.SendMessageAsync("", false, eb.Build());
                            Logs.WriteLine(message);
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.Message);
                            throw;
                        }
                    }
                    // =====================================================
                    // = Gestion de la commande bank info (nom_Personnage) =
                    // =====================================================
                    else if (input.StartsWith("info"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours info
                        if (argus[0] == "info")
                        {
                            if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                input = input.Replace(argus[0] + " ", "");
                                BankAccount infoAccount = await Global.GetBankAccountByNameAsync(input);
                                int index = Global.GetCharacterIndexByName(input);
                                EmbedBuilder eb = new EmbedBuilder();
                                eb.WithColor((Color) EmbedColors.BankInfo)
                                    .WithThumbnailUrl(Global.Characters[index].LienImage)
                                    .AddField($"= Argent de **{Global.Characters[index].Prenom} {Global.Characters[index].Nom}** =", await Global.GetBankInfoAsync(infoAccount) + $"\n__**Propriétaire:**__ {this.Context.Guild.GetUser(Global.Characters[index].OwnerId).Mention}");
                                await ReplyAsync("", false, eb.Build());
                            }
                        }
                    }
                    // ==========================================================================
                    // = Gestion de la commande (admin) bank deposit (montant) (nom_Personnage) =
                    // ==========================================================================
                    else if (input.StartsWith("deposit") || input.StartsWith("dp"))
                    {
                        if (Global.IsStaff((SocketGuildUser) this.Context.User))
                        {
                            argus = input.Split(' ');
                            // Sert à s'assurer qu'argus[0] == toujours deposit
                            if (argus[0] == "deposit" || argus[0] == "dp")
                            {
                                if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                    Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                                }
                                else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                    Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                                }
                                else
                                {
                                    decimal.TryParse(argus[1], out montant);
                                    BankAccount depositAccount = await Global.GetBankAccountByNameAsync(argus[2]);
                                    if (depositAccount != null)
                                    {
                                        decimal nvMontant = depositAccount.Montant + montant;
                                        Global.Characters[Global.GetCharacterIndexByName(argus[2])].CompteEnBanque = new BankAccount
                                        {
                                            Salaire = depositAccount.Salaire,
                                            Montant = nvMontant
                                        };
                                        await Global.EnregistrerDonneesPersosAsync();
                                        await ReplyAsync($"{montant} berrys ajoutée(s) sur le compte de {argus[2]}");
                                        Logs.WriteLine($"{montant} berrys ajoutée(s) sur le compte de {argus[2]}");
                                        await SendBank("up");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                        Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Context.Guild.Name == "ServeurTest")
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                            else
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                        }
                    }
                    // ===========================================================================
                    // = Gestion de la commande (admin) bank withdraw (montant) (nom_Personnage) =
                    // ===========================================================================
                    else if (input.StartsWith("withdraw") || input.StartsWith("wd"))
                    {
                        if (Global.IsStaff((SocketGuildUser) this.Context.User))
                        {
                            argus = input.Split(' ');
                            // Sert à s'assurer qu'argus[0] == toujours withdraw
                            if (argus[0] == "withdraw" || argus[0] == "wd")
                            {
                                if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                    Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                                }
                                else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                    Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                                }
                                else
                                {
                                    decimal.TryParse(argus[1], out montant);
                                    BankAccount withdrawAccount = await Global.GetBankAccountByNameAsync(argus[2]);
                                    if (withdrawAccount != null)
                                    {
                                        int index = Global.GetCharacterIndexByName(argus[2]);
                                        decimal nvMontant = withdrawAccount.Montant - montant;
                                        nvMontant = nvMontant < 0 ? throw new InvalidOperationException("Tu ne peux pas dépenser plus que ce que tu as...") : nvMontant;
                                        Global.Characters[index].CompteEnBanque = new BankAccount
                                        {
                                            Montant = nvMontant,
                                            Salaire = withdrawAccount.Salaire
                                        };
                                        await Global.EnregistrerDonneesPersosAsync();
                                        await ReplyAsync($"{montant} berrys retirée(s) sur le compte de {Global.Characters[index].Prenom}");
                                        Logs.WriteLine($"{montant} berrys retirée(s) sur le compte de {Global.Characters[index].Prenom}");
                                        await SendBank("up");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                        Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Context.Guild.Name == "ServeurTest")
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                            else
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                        }
                    }
                    // ======================================================================
                    // = Gestion de la commande (admin) bank set (montant) (nom_Personnage) =
                    // ======================================================================
                    else if (input.StartsWith("set"))
                    {
                        if (Global.IsStaff((SocketGuildUser) this.Context.User))
                        {
                            argus = input.Split(' ');
                            // Sert à s'assurer qu'argus[0] == toujours set
                            if (argus[0] == "set")
                            {
                                if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                    Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                                }
                                else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                    Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                                }
                                else
                                {
                                    decimal.TryParse(argus[1], out montant);
                                    BankAccount setAccount = await Global.GetBankAccountByNameAsync(argus[2]);
                                    if (setAccount != null)
                                    {
                                        decimal nvMontant = montant < 0 ? 0 : montant;
                                        int index = Global.GetCharacterIndexByName(argus[2]);
                                        Global.Characters[index].CompteEnBanque = new BankAccount
                                        {
                                            Montant = nvMontant,
                                            Salaire = setAccount.Salaire
                                        };
                                        await Global.EnregistrerDonneesPersosAsync();
                                        await ReplyAsync($"Le montant sur le compte de \"**{Global.Characters[index].Prenom}**\" est désormais de \"**{nvMontant}**\" berrys");
                                        Logs.WriteLine($"Le montant sur le compte de {Global.Characters[index].Prenom} est désormais de \"**{nvMontant}**\" berrys");
                                        await SendBank("up");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                        Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Context.Guild.Name == "ServeurTest")
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                            else
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                        }
                    }
                    // =================================================================================
                    // = Gestion de la commande bank pay (montant) (nom_Personnage1) (nom_Personnage2) =
                    // =================================================================================
                    else if (input.StartsWith("pay") || input.StartsWith("transfer") || input.StartsWith("tr"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours withdraw
                        if (argus[0] == "pay" || argus[0] == "transfer" || argus[0] == "tr")
                        {
                            if (argus.Length > 4 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément montant (et qu'il n'y a que 4 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 4) // Sert à s'assurer que argus[1] == forcément montant (et qu'il n'y a que 4 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                decimal.TryParse(argus[1], out montant);
                                BankAccount withdrawAccount = await Global.GetBankAccountByNameAsync(argus[2]);
                                BankAccount depositAccount = await Global.GetBankAccountByNameAsync(argus[3]);
                                if (withdrawAccount != null && depositAccount != null)
                                {
                                    //Retire l'argent du premier compte
                                    int index = Global.GetCharacterIndexByName(argus[2]);
                                    decimal ancienMontant = withdrawAccount.Montant;
                                    decimal nvMontant = ancienMontant - montant;
                                    nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                    nvMontant = nvMontant < 0 ? throw new InvalidOperationException("Tu ne peux pas dépenser plus que ce que tu as...") : nvMontant;
                                    Global.Characters[index].CompteEnBanque = new BankAccount
                                    {
                                        Montant = nvMontant,
                                        Salaire = withdrawAccount.Salaire
                                    };
                                    Logs.WriteLine($"{ancienMontant - nvMontant} berrys retirée(s) sur le compte de {Global.Characters[index].Prenom}");
                                    // Ajoute l'argent sur le 2eme compte
                                    int index2 = Global.GetCharacterIndexByName(argus[3]);
                                    decimal montantTr = nvMontant == 0 ? ancienMontant : montant;
                                    decimal depositAccountNewAmount = depositAccount.Montant + montantTr;
                                    Global.Characters[index2].CompteEnBanque = new BankAccount
                                    {
                                        Salaire = depositAccount.Salaire,
                                        Montant = depositAccountNewAmount
                                    };
                                    await Global.EnregistrerDonneesPersosAsync();
                                    await ReplyAsync($"{montant} berrys ajoutée(s) sur le compte de {Global.Characters[index2].Prenom}");
                                    Logs.WriteLine($"{montant} berrys ajoutée(s) sur le compte de {Global.Characters[index2].Prenom}");

                                    await Global.EnregistrerDonneesPersosAsync();
                                    await ReplyAsync($"{montantTr} berrys transférées du compte de {Global.Characters[index].Prenom} vers le compte de {Global.Characters[index2].Prenom}");
                                    await SendBank("up");
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    // =========================================================================
                    // = Gestion de la commande (admin) bank setsal (montant) (nom_Personnage) =
                    // =========================================================================
                    else if (input.StartsWith("sts"))
                    {
                        if (Global.IsStaff((SocketGuildUser) this.Context.User))
                        {
                            try
                            {
                                argus = input.Split(' ');
                                // Sert à s'assurer qu'argus[0] == toujours setsal
                                if (argus[0] == "sts" || argus[0] == "setsal")
                                {
                                    try
                                    {
                                        if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                        {
                                            await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                            Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                                        }
                                        else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                        {
                                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                            Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                                        }
                                        else
                                        {
                                            decimal.TryParse(argus[1], out montant);
                                            BankAccount setAccount = await Global.GetBankAccountByNameAsync(argus[2]);
                                            if (setAccount != null)
                                            {
                                                int index = Global.GetCharacterIndexByName(argus[3]);
                                                string stsName = Global.Characters[index].Prenom;
                                                decimal nvMontant = montant;
                                                nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                                Global.Characters[index].CompteEnBanque = new BankAccount
                                                {
                                                    Salaire = nvMontant,
                                                    Montant = setAccount.Montant
                                                };
                                                await Global.EnregistrerDonneesPersosAsync();
                                                await ReplyAsync($"Le salaire de {stsName} est désormais de {nvMontant} berrys");
                                                Logs.WriteLine($"Le salaire de {stsName} est désormais de {nvMontant} berrys");
                                                await SendBank("up");
                                            }
                                            else
                                            {
                                                await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                                Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        throw;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logs.WriteLine(e.ToString());
                                throw;
                            }
                        }
                        else
                        {
                            if (this.Context.Guild.Name == "ServeurTest")
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                            else
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                        }
                    }
                    // ================================================================
                    // = Gestion de la commande (admin) bank givesal (nom_Personnage) =
                    // ================================================================
                    else if (input.StartsWith("givesal") || input.StartsWith("gs"))
                    {
                        if (Global.IsStaff((SocketGuildUser) this.Context.User))
                        {
                            argus = input.Split(' ');
                            // Sert à s'assurer qu'argus[0] == toujours deposit
                            if (argus[0] == "givesal" || argus[0] == "gs")
                            {
                                if (argus.Length > 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                    Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                                }
                                else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                                {
                                    await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                    Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                                }
                                else
                                {
                                    if (argus[1] == "all")
                                    {
                                        await Global.VerserSalairesAsync();
                                    }
                                    else
                                    {
                                        Character depositAccount = await Global.GetCharacterByNameAsync(argus[1]);
                                        if (depositAccount != null)
                                        {
                                            string dpName = depositAccount.Prenom;
                                            decimal dpSalaire = depositAccount.CompteEnBanque.Salaire;
                                            await ReplyAsync($"Salaire de {dpSalaire} berrys versé sur le compte de {dpName}");
                                            await Global.VerserSalaireAsync(depositAccount);
                                        }
                                        else
                                        {
                                            await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: character create pour créer un nouveau personnage");
                                            Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: character create pour créer un nouveau personnage");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (this.Context.Guild.Name == "ServeurTest")
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                            else
                                await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                        }
                    }
                    // ======================================
                    // = Gestion de la commande bank update =
                    // ======================================
                    else if (input == "update" || input == "up")
                    {
                        try
                        {
                            foreach (SocketTextChannel banque in Global.ChannelsBanques)
                            {
                                foreach (IMessage toDel in await banque.GetMessagesAsync().FlattenAsync())
                                    await toDel.DeleteAsync();
                                foreach (Character perso in Global.Characters)
                                {
                                    input = perso.Nom + perso.Prenom;
                                    BankAccount infoAccount = await Global.GetBankAccountByNameAsync(input);
                                    int index = Global.GetCharacterIndexByName(input);
                                    EmbedBuilder eb = new EmbedBuilder();
                                    eb.WithColor((Color) EmbedColors.BankInfo)
                                        .WithThumbnailUrl(Global.Characters[index].LienImage)
                                        .AddField($"= Argent de **{Global.Characters[index].Prenom} {Global.Characters[index].Nom}** =", await Global.GetBankInfoAsync(infoAccount) + $"\n__**Propriétaire:**__ {this.Context.Guild.GetUser(Global.Characters[index].OwnerId).Mention}");
                                    await banque.SendMessageAsync("", false, eb.Build());
                                    await Task.Delay(200);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.Message);
                            throw;
                        }

                        Logs.WriteLine("Comptes en banque mis à jour");
                    }
                    else
                    {
                        await ReplyAsync(error);
                    }
                }
                else if (input == "none")
                {
                    await ReplyAsync(error);
                    Logs.WriteLine(error);
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(error + " " + e.Message);
                await ReplyAsync(e.Message);
            }
        }

        #endregion
    }
}
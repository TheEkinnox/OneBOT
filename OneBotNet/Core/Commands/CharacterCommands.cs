#region MÉTADONNÉES

// Nom du fichier : CharacterCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-11
// Date de modification : 2019-09-21

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using OneBotNet.Core.Data.Classes;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

#endregion

namespace OneBotNet.Core.Commands
{
    public class CharacterCommands : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        //Todo: Compléter les commandes character
        [Command("character"), Alias("perso", "char")]
        public async Task CharacterCommandsTask(string subCommand = "none", [Remainder] string input = null)
        {
            try
            {
                IUser mentionnedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
                await Global.ChargerDonneesPersosAsync();
                switch (subCommand.ToUpperInvariant())
                {
                    case "INFO":
                        if (input == null)
                            throw new ArgumentException("Le nom d'un personnage doit être entré.");
                        Character character = await Global.GetCharacterByNameAsync(input);
                        if (character == null)
                            throw new ArgumentException($"Le personnage {input} est introuvable... Veuillez vérifier le nom saisi s.v.p.");
                        EmbedBuilder eb = new EmbedBuilder();
                        switch (character.Camps)
                        {
                            case Side.Marine:
                                eb.Color = (Color) EmbedColors.Marine;
                                break;
                            case Side.Pirate:
                                eb.Color = (Color) EmbedColors.Pirate;
                                break;
                            case Side.Revolutionnaire:
                                eb.Color = (Color) EmbedColors.Revolutionnaire;
                                break;
                        }

                        eb.AddField($"= Infos de {character.Prenom} {character.Nom} =", $"{await Global.GetInfoPersoAsync(character)}\n__**Image du personnage:**__")
                            .WithImageUrl(character.LienImage);
                        await ReplyAsync(null, false, eb.Build());
                        await Task.Delay(15);
                        break;
                    case "CREATE":
                    case "CR":
                        if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                            throw new UnauthorizedAccessException("Vous devez être administrateur pour créer un personnage...");
                        ulong ownerId = mentionnedUser != null ? mentionnedUser.Id : throw new ArgumentNullException(null, "Vous devez mentionner le propriétaire du personnage à créer...");
                        await ReplyAsync("Veuillez terminer la procédure en mp s.v.p.");
                        IMessageChannel mp = await this.Context.User.GetOrCreateDMChannelAsync();

                        await mp.SendMessageAsync("Vous pouvez annuler le processus de création à tout moment en envoyant \"o!exit\"");
                        string nom, prenom, race, sexe, nomImagePerso, lienImage;
                        int age, reputation = 0;
                        decimal prime = 0, montant, salaire;
                        bool dead = true, alive = true;
                        Side camps;
                        List<string> items = new List<string>();
                        await mp.SendMessageAsync("__**Prenom:**__");
                        RestUserMessage msg;
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        prenom = msg.Content;
                        await msg.AddReactionAsync(new Emoji("👌"));
                        await mp.SendMessageAsync("__**Nom:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        nom = msg.Content;
                        await msg.AddReactionAsync(new Emoji("👌"));
                        demanderRace:
                        await mp.SendMessageAsync("__**Race(1:Humain, 2:Homme poisson, 3:Géant, 4:Mink, 5:Nain, 6:Cyborg):**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            race = msg.Content switch
                            {
                                "1" => "Humain",
                                "2" => "Homme poisson",
                                "3" => "Géant",
                                "4" => "Mink",
                                "5" => "Nain",
                                "6" => "Cyborg",
                                _ => throw new Exception(),
                            };
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderRace;
                        }

                        demanderSexe:
                        await mp.SendMessageAsync("__**Sexe(1:Homme, 2:Femme):**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            sexe = msg.Content switch
                            {
                                "1" => "Homme",
                                "2" => "Femme",
                                _ => throw new Exception()
                            };
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderSexe;
                        }

                        demanderAge:
                        await mp.SendMessageAsync("__**Age:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            age = int.Parse(msg.Content);
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderAge;
                        }

                        demanderCamps:
                        await mp.SendMessageAsync("__**Camps (1:Révolutionnaire, 2:Pirate, 3:Marine):**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            camps = msg.Content switch
                            {
                                "1" => Side.Revolutionnaire,
                                "2" => Side.Pirate,
                                "3" => Side.Marine,
                                _ => throw new Exception(),
                            };
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderCamps;
                        }

                        if (camps == Side.Marine)
                            goto demanderReputation;
                        demanderPrime:
                        await mp.SendMessageAsync("__**Prime:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            prime = decimal.Parse(msg.Content);
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderPrime;
                        }

                        await msg.AddReactionAsync(new Emoji("👌"));
                        demanderNiveauRecherche:
                        await mp.SendMessageAsync("__**Recherche (1:Mort ou Vif, 2:Vivant seulement, 3:Mort seulement):**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            switch (msg.Content)
                            {
                                case "1":
                                    dead = true;
                                    alive = true;
                                    break;
                                case "2":
                                    dead = false;
                                    alive = true;
                                    break;
                                case "3":
                                    dead = true;
                                    alive = false;
                                    break;
                                default:
                                    throw new Exception();
                            }

                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderNiveauRecherche;
                        }

                        goto demanderMontant;
                        demanderReputation:
                        await mp.SendMessageAsync("__**Réputation:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            reputation = int.Parse(msg.Content);
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderReputation;
                        }

                        await msg.AddReactionAsync(new Emoji("👌"));
                        demanderMontant:
                        await mp.SendMessageAsync("__**Argent disponible:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            montant = decimal.Parse(msg.Content);
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderPrime;
                        }

                        await msg.AddReactionAsync(new Emoji("👌"));
                        demanderSalaire:
                        await mp.SendMessageAsync("__**Salaire:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        try
                        {
                            salaire = decimal.Parse(msg.Content);
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        catch (Exception)
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderSalaire;
                        }

                        await msg.AddReactionAsync(new Emoji("👌"));
                        demanderAjouterItems:
                        await mp.SendMessageAsync("Ajouter un objet à l'inventaire du personnage <o/n>?");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        if (msg.Content.ToLower() == "o")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto demanderItems;
                        }
                        else if (msg.Content.ToLower() == "n")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto demanderImage;
                        }
                        else
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderAjouterItems;
                        }

                        demanderItems:
                        await mp.SendMessageAsync("__**Objet:**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }

                        items.Add(msg.Content);
                        await msg.AddReactionAsync(new Emoji("👌"));
                        goto demanderAjouterItems;
                        demanderImage:
                        await mp.SendMessageAsync("__**Image (fichier ou lien):**__");
                        do
                        {
                            msg = (RestUserMessage) await mp.GetMessagesAsync().Flatten().FirstOrDefault();
                            await Task.Delay(15);
                        } while (msg.Author.Id != this.Context.User.Id);

                        if (msg.Content.ToLower() == "o!exit")
                        {
                            await msg.AddReactionAsync(new Emoji("👌"));
                            goto exit;
                        }
                        else
                        {
                            Attachment attachment = msg.Attachments.FirstOrDefault();
                            if (attachment == null)
                                goto checkUrl;
                            lienImage = attachment.Url;
                            goto ajouterPerso;
                        }

                        checkUrl:
                        if (Uri.TryCreate(msg.Content, UriKind.Absolute, out Uri uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                        {
                            lienImage = msg.Content;
                            await msg.AddReactionAsync(new Emoji("👌"));
                        }
                        else
                        {
                            await mp.SendMessageAsync("Valeur invalide...");
                            goto demanderImage;
                        }

                        ajouterPerso:
                        msg = (RestUserMessage) await mp.SendMessageAsync("Création du personnage...");
                        nomImagePerso = prenom.ToLower(CultureInfo.InvariantCulture).Replace(".", "").Replace(" ", "") + nom.ToLower(CultureInfo.InvariantCulture).Replace(".", "").Replace(" ", "") + ".png";
                        Global.SaveImageFromUrl(lienImage, Global.CheminImagesPersos + nomImagePerso, ImageFormat.Png);
                        character = new Character
                        {
                            OwnerId = ownerId,
                            Prenom = prenom,
                            Nom = nom,
                            Race = race,
                            Sexe = sexe,
                            Age = age,
                            Camps = camps,
                            Prime = prime,
                            Reputation = reputation,
                            CompteEnBanque = new BankAccount
                            {
                                Montant = montant,
                                Salaire = salaire
                            },
                            Items = items,
                            LienImage = lienImage,
                            NomImagePerso = nomImagePerso,
                            Dead = dead,
                            Alive = alive
                        };
                        await msg.ModifyAsync(m => m.Content = "Ajout du personnage à la base de données...");
                        Global.Characters.Add(character);
                        await Global.EnregistrerDonneesPersosAsync();
                        await Task.Delay(500);
                        await msg.ModifyAsync(m => m.Content = "Personnage créé avec succès!");
                        await ReplyAsync("Personnage créé avec succès!");
                        await CharacterCommandsTask("info", character.Nom + character.Prenom);
                        exit:
                        break;
                    case "DEL":
                    case "DELETE":
                        if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                            throw new UnauthorizedAccessException("Vous devez être administrateur pour supprimer un personnage...");
                        if (input == null)
                            throw new ArgumentException("Le nom d'un personnage doit être entré.");
                        int characterIndex = await Global.GetCharacterIndexByNameAsync(input);
                        if (characterIndex < 0)
                            throw new ArgumentException($"Le personnage {input} est introuvable... Veuillez vérifier le nom saisi s.v.p.");
                        Global.Characters.RemoveAt(characterIndex);
                        await Global.EnregistrerDonneesPersosAsync();
                        await ReplyAsync($"Le personnage \"{input}\" a été supprimé avec succès.");
                        break;
                    case "LST":
                    case "LIST":
                        List<Character> characters = Global.Characters;
                        foreach (Character perso in characters)
                        {
                            await CharacterCommandsTask("info", perso.Nom + perso.Prenom);
                            await Task.Delay(200);
                        }

                        break;
                    case "EDIT":
                        if (string.IsNullOrWhiteSpace(input))
                            throw new ArgumentException("Vous devez entrer le paramètre à modifier...");
                        if (input.Split(' ').Length < 4)
                            throw new ArgumentException("Il doit y avoir au moins 4 paramètres.(syntaxe: edit option nomPerso operation valeur)");
                        string nomEdit = input.Split(' ')[0];
                        int eIndex = await Global.GetCharacterIndexByNameAsync(nomEdit);
                        string option = input.Split(' ')[1];
                        string ope = input.Split(' ')[2];
                        if (ope != "=" && ope != "+=" && ope != "+" && ope != "-=" && ope != "-" && ope != "*=" && ope != "*" && ope != "/=" && ope != "/")
                            throw new ArgumentException("Les operations disponible sont =, += ou +, -= ou -, *= ou *, /= ou /");
                        int intInput;
                        input = input.Replace($"{nomEdit} {option} {ope} ", "");
                        SocketGuildUser user = (SocketGuildUser) this.Context.User;
                        switch (option.ToUpperInvariant())
                        {
                            // Todo: âge, camps, prime/reputation modifiable par admins seulement
                            case "NAME":
                            case "NOM":
                                if (!Global.HasRole(user, "Admin"))
                                    throw new UnauthorizedAccessException("Vous devez être administrateur pour modifier le nom d'un personnage.");
                                if (ope == "=")
                                {
                                    Global.Characters[eIndex].Nom = input.Replace($"{nomEdit} {option} {ope} ", "");
                                    await Global.EnregistrerDonneesPersosAsync();
                                }
                                else
                                    throw new ArgumentException("La seule opérations disponible pour le paramètre nom est \"=\".");

                                break;
                            case "PRENOM":
                            case "PN":
                                if (!Global.HasRole(user, "Admin"))
                                    throw new UnauthorizedAccessException("Vous devez être administrateur pour modifier le prénom d'un personnage.");
                                if (ope == "=")
                                {
                                    Global.Characters[eIndex].Prenom = input;
                                    await Global.EnregistrerDonneesPersosAsync();
                                }
                                else
                                    throw new ArgumentException("La seule opérations disponible pour le paramètre prénom est \"=\".");

                                break;
                            case "AGE":
                                if (!Global.HasRole(user, "Admin"))
                                    throw new UnauthorizedAccessException("Vous devez être administrateur pour modifier l'âge d'un personnage.");
                                try
                                {
                                    intInput = int.Parse(input);
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException("L'âge entré est invalide. Vérifiez qu'il s'agit bien d'__**UN__ NOMBRE**");
                                }

                                if (ope == "+=")
                                    if (nomEdit.ToUpperInvariant() == "ALL")
                                        foreach (Character persoEdit in Global.Characters)
                                        {
                                            persoEdit.Age += intInput;
                                        }

                                break;
                            case "PRIME":
                            case "BOUNTY":
                            case "BNT":
                                if (!Global.HasRole(user, "Admin"))
                                    throw new UnauthorizedAccessException("Vous devez être administrateur pour modifier le nom d'un personnage.");
                                try
                                {
                                    intInput = int.Parse(input);
                                }
                                catch (Exception)
                                {
                                    throw new ArgumentException("L'âge entré est invalide. Vérifiez qu'il s'agit bien d'__**UN__ NOMBRE**");
                                }

                                if (ope == "+=")
                                    if (nomEdit.ToUpperInvariant() == "ALL")
                                        foreach (Character persoEdit in Global.Characters)
                                        {
                                            if (persoEdit.Camps != Side.Marine)
                                                persoEdit.Prime += intInput;
                                            else persoEdit.Reputation += intInput;
                                        }
                                    else
                                        Global.Characters[eIndex].Prime += intInput;

                                else if (ope == "=")
                                    if (nomEdit.ToUpperInvariant() == "ALL")
                                        foreach (Character persoEdit in Global.Characters)
                                        {
                                            if (persoEdit.Camps != Side.Marine)
                                                persoEdit.Prime = intInput;
                                            else persoEdit.Reputation = intInput;
                                        }

                                await Global.EnregistrerDonneesPersosAsync();
                                await Global.GenererAvisDeRecherche(Global.Characters[eIndex], Global.Characters[eIndex].Dead, Global.Characters[eIndex].Alive);
                                await ReplyAsync($"La prime de {(await Global.GetCharacterByNameAsync(nomEdit)).Prenom + (await Global.GetCharacterByNameAsync(nomEdit)).Nom} a été mise à jour avec succès.");
                                await this.Context.Channel.SendFileAsync(Global.CheminImagesWanted + Global.Characters[eIndex].NomImagePerso);
                                break;
                            //Todo: Image modifiable par Admins ou propriétaire
                            case "IMAGE":
                            case "IMG":
                                if (!Global.HasRole(user, "Admin") && Global.Characters[eIndex].OwnerId != this.Context.User.Id)
                                    throw new UnauthorizedAccessException("Vous devez être administrateur ou propriétaire du personnage pour modifier le nom d'un personnage.");
                                if (ope == "=")
                                {
                                    Attachment attachment = this.Context.Message.Attachments.FirstOrDefault();
                                    if (attachment != null)
                                        Global.Characters[eIndex].LienImage = attachment.Url;
                                    if (string.IsNullOrWhiteSpace(input))
                                        throw new ArgumentException("Aucune image détectée. Vérifiez si une image est liée ou si le message contient un lien s.v.p...");
                                    if (Uri.TryCreate(input, UriKind.Absolute, out Uri urlResult)
                                        && (urlResult.Scheme == Uri.UriSchemeHttp || urlResult.Scheme == Uri.UriSchemeHttps))
                                        Global.Characters[eIndex].LienImage = input;
                                    else
                                        throw new ArgumentException("L'image entrée est invalide...");
                                    string cheminImgEdit = Global.CheminImagesPersos + Global.Characters[eIndex].NomImagePerso;
                                    if (File.Exists(cheminImgEdit))
                                        File.Delete(cheminImgEdit);
                                }
                                else
                                    throw new ArgumentException("La seule opérations disponible pour le paramètre nom est \"=\".");

                                await Global.EnregistrerDonneesPersosAsync();
                                await ReplyAsync($"L'image de {Global.Characters[eIndex].Prenom} a été modifiée avec succès");
                                await CharacterCommandsTask("info", nomEdit);
                                break;
                            default:
                                throw new ArgumentException($"Impossible de modifier le paramètre {option}. Vérifiez l'orthographe ou demandez à un admin.");
                        }

                        break;
                    case "NONE":
                        throw new ArgumentException("La commande character ne peut pas être utilisée san paramètre(s).");
                    default:
                        throw new ArgumentException("Le paramètre entré est invalide. character help pour voir la liste des paramètres valides.");
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine("Message d'erreur de la commande character: " + e.Message);
                await ReplyAsync(e.Message);
            }
        }

        #endregion
    }
}
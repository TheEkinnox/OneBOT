#region MÉTADONNÉES

// Nom du fichier : CharacterCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-11
// Date de modification : 2019-09-12

#endregion

#region USING

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OneBotNet.Core.Data.Classes;

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

                        eb.WithTitle($"**{character.Prenom} {character.Nom}**")
                            .AddField("==============================================", $"{await Global.GetInfoPersoAsync(character)}\n__**Image du personnage:**__");
                        await ReplyAsync(null, false, eb.Build());
                        await this.Context.Channel.SendFileAsync(Global.CheminImagesPersos + character.NomImagePerso);
                        break;
                    case "NONE":
                        throw new ArgumentException("La commande character ne peut pas être utilisée san paramètre(s).");
                    default:
                        throw new ArgumentException("Le paramètre entré est invalide. character help pour voir la liste des paramètres valides.");
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine("Message d'erreur de la commande character" + e.Message);
                await ReplyAsync(e.Message);
            }
        }

        #endregion
    }
}
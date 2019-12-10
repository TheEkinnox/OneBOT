#region MÉTADONNÉES

// Nom du fichier : WantedCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-11
// Date de modification : 2019-09-15

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
    public class WantedCommands : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        //Todo: Compléter les commandes wanted
        [Command("wanted"), Alias("bounty", "prime")]
        public async Task WantedCommandsTask(string subCommand = "none", [Remainder] string input = null)
        {
            try
            {
                await Global.ChargerDonneesPersosAsync();
                switch (subCommand.ToUpperInvariant())
                {
                    case "LST":
                    case "LIST":
                        List<Character> characters = Global.Characters;
                        await this.Context.Message.DeleteAsync();
                        foreach (Character perso in characters)
                        {
                            (await Global.GenererAvisDeRecherche(perso, perso.Dead, perso.Alive)).Dispose();
                            if (perso.Camps != Side.Marine)
                                await this.Context.Channel.SendFileAsync(Global.CheminImagesWanted + perso.NomImagePerso, "``` ```" /*+ this.Context.Guild.GetUser(perso.OwnerId).Mention*/);
                            await Task.Delay(20);
                        }

                        break;
                    case "REP":
                    case "MARINE":
                        characters = Global.Characters;
                        await this.Context.Message.DeleteAsync();
                        foreach (Character perso in characters)
                        {
                            (await Global.GenererAvisDeRecherche(perso, perso.Dead, perso.Alive)).Dispose();
                            if (perso.Camps == Side.Marine)
                                await this.Context.Channel.SendFileAsync(Global.CheminImagesWanted + perso.NomImagePerso, "``` ```" /*+ this.Context.Guild.GetUser(perso.OwnerId).Mention*/);
                            await Task.Delay(20);
                        }

                        break;
                    case "TYPE":
                        string nom = input.Split(' ')[0];
                        int index = await Global.GetCharacterIndexByNameAsync(nom);
                        input = input.Replace($"{nom} ", "");
                        bool dead = input.ToUpperInvariant().Contains("DEAD")||input.ToUpperInvariant().Contains("MORT");
                        bool alive = input.ToUpperInvariant().Contains("ALIVE")||input.ToUpperInvariant().Contains("VIF")||input.ToUpperInvariant().Contains("VIVANT");
                        Global.Characters[index].Dead = dead;
                        Global.Characters[index].Alive = alive;
                        await Global.EnregistrerDonneesPersosAsync();
                        string strTemp;
                        if (dead && alive)
                            strTemp = "__**Mort ou Vif**__";
                        else if (!dead && alive)
                            strTemp = "**vivant seulement**";
                        else strTemp = "**mort seulement**";
                        await ReplyAsync($"{Global.Characters[index].Prenom} {Global.Characters[index].Nom} est désormais recherché {strTemp}.");
                        break;
                    case "UP":
                        case "UPDATE":
                        try
                        {
                            foreach (SocketTextChannel primeChan in Global.ChannelsPrime)
                            {
                                foreach (IMessage toDel in await primeChan.GetMessagesAsync().FlattenAsync())
                                    await toDel.DeleteAsync();
                                foreach (Character perso in Global.Characters)
                                {
                                    (await Global.GenererAvisDeRecherche(perso, perso.Dead, perso.Alive)).Dispose();
                                    if (perso.Camps != Side.Marine)
                                        await primeChan.SendFileAsync(Global.CheminImagesWanted + perso.NomImagePerso, "``` ```" /*+ this.Context.Guild.GetUser(perso.OwnerId).Mention*/);
                                    await Task.Delay(200);
                                }
                            }
                            foreach (SocketTextChannel primeChan in Global.ChannelsReput)
                            {
                                foreach (IMessage toDel in await primeChan.GetMessagesAsync().FlattenAsync())
                                    await toDel.DeleteAsync();
                                foreach (Character perso in Global.Characters)
                                {
                                    if (perso.Camps == Side.Marine)
                                        await primeChan.SendFileAsync(Global.CheminImagesWanted + perso.NomImagePerso, "``` ```" /*+ this.Context.Guild.GetUser(perso.OwnerId).Mention*/);
                                    await Task.Delay(200);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.Message);
                            throw;
                        }

                        break;
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine("Message d'erreur de la commande bounty : " + e.ToString());
                await ReplyAsync(e.Message);
            }
        }

        #endregion
    }
}
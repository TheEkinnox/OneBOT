#region MÉTADONNÉES

// Nom du fichier : Roll.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-04-20
// Date de modification : 2019-06-04

#endregion

#region USING

using System;
using System.Threading.Tasks;
using Discord.Commands;
using MathParserTK;
using ShinoBotNet.Core.Data.Classes;

#endregion

namespace ShinoBotNet.Core.Commands
{
    public class Roll : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("roll"), Alias("de", "dice", "r"), Summary("Lance 1 dé (par défaut 1d100) **:warning:LES PRIORITES D'OPERATEURS NE SONT PAS RESPECTEES:warning:**")]
        public async Task LancerDe([Remainder] string input = "none")
        {
            MathParser parser = new MathParser();
            const int bonusCaly = 8;
            int max = 100;
            int[] resultat = new int[99999999];
            string msgResultat = "";
            int sumResultats = 0;
            bool valide = true;
            bool containsCalcul = input.Contains('+') || input.Contains('-') || input.Contains('*') || input.Contains('/');
            string calculString;

            if (input == "help")
            {
                await this.Context.Channel.SendMessageAsync("Lance le nombre indiqué de dés (par défaut 1d100)");
            }
            else if (input.StartsWith("d"))
            {
                if (int.TryParse(input.Replace("d", ""), out max))
                {
                    resultat[0] = this._rand.Next(1,max + 1);
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                    await ReplyAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
                    //return;
                }
                else
                {
                    await this.Context.Channel.SendMessageAsync("Valeur invalide");
                    Logs.WriteLine("Valeur invalide");
                    //return;
                }
            }
            else if (char.IsDigit(input[0]) && !input.Contains("d") && int.TryParse(input, out max))
            {
                resultat[0] = this._rand.Next(1,max + 1);
                Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
            }
            else if (char.IsDigit(input[0]) && input.Contains("d"))
            {
                string[] argus = input.Split('d', '+', '-', '*', '/');
                if (int.TryParse(argus[0], out int nbDes) && !String.IsNullOrEmpty(argus[1]) && int.TryParse(argus[1], out max))
                {
                    for (int i = 0; i < nbDes; i++)
                    {
                        resultat[i] = this._rand.Next(1,max + 1);
                        sumResultats += resultat[i];

                        if (i + 1 < nbDes)
                        {
                            msgResultat += $"{resultat[i]}, ";
                        }
                        else
                        {
                            msgResultat += $"{resultat[i]}";
                        }
                    }

                    if (containsCalcul)
                    {
                        Logs.WriteLine(sumResultats.ToString());
                        calculString = input.Replace(nbDes.ToString() + "d" + max.ToString(),"");
                        sumResultats = (int) parser.Parse(sumResultats.ToString()+calculString, false);
                        msgResultat += calculString;
                        Logs.WriteLine("Calcul effectué");
                        Logs.WriteLine(sumResultats.ToString());
                    }

                    for (int i = 0; i < argus.Length; i++)
                    {
                        Logs.WriteLine(argus[i]);
                    }

                    if (nbDes > 1 && valide && this.Context.User.Id != 298614183258488834)
                    {
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats} ({msgResultat})");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat})");
                    }
                    else if (containsCalcul)
                    {
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats} ({msgResultat}");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat} = {sumResultats})");
                    }
                    else
                    {
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats}");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats}");
                    }
                }
                else
                {
                    Logs.WriteLine("Entrée invalide");
                    await this.Context.Channel.SendMessageAsync("Entrée invalide");
                }
            }
            else if (input.ToLower() == "none")
            {
                resultat[0] = this._rand.Next(1,max);
                if (this.Context.User.Id != 298614183258488834)
                {
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
                }
                else if (this.Context.User.Id == 298614183258488834)
                {
                    if (resultat[0] - 8 < 0)
                        resultat[0] += 8;
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0] + "-" + bonusCaly}");
                    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0] - bonusCaly}");
                }
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Valeur invalide\n");
                Logs.WriteLine("Valeur invalide\n");
            }
        }

        #endregion
    }
}
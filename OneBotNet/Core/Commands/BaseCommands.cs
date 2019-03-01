#region MÉTADONNÉES

// Nom du fichier : BaseCommands.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-03-01
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using OneBotNet.Core.Data.Classes;

#endregion

namespace OneBotNet.Core.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("hello"), Summary("Commande hello")]
        public async Task SendMessage()
        {
            await this.Context.Channel.SendMessageAsync($"Wsh {this.Context.User.Mention}, bien ou bien ma couille");
        }

        [Command("testembed"), Alias("embed", "te", "emb")]
        public async Task SendEmbed([Remainder] string input = "None")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor("Test embed", this.Context.User.GetAvatarUrl());
            embed.WithColor(250, 125, 125);
            embed.WithFooter("Le proprio génial du discord", this.Context.Guild.Owner.GetAvatarUrl());
            embed.WithDescription("This is a **PEEEEERFECT** random desc with an `Awesome` link.\n" +
                                  "[Le meilleur des wiki :heart::heart:](http://fr.alternia.wikia.com)");

            await this.Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("plop")]
        public async Task SendOctoplop()
        {
            // ReSharper disable once InconsistentNaming
            string imgsPath = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Data\Plop\");
            string[] vectImgs = new string[]
            {
                $"{imgsPath}p0.jpg",
                $"{imgsPath}p1.jpg",
                $"{imgsPath}p2.jpg",
                $"{imgsPath}p3.jpg",
                $"{imgsPath}p4.jpg",
                $"{imgsPath}p5.jpg",
                $"{imgsPath}p6.jpg",
                $"{imgsPath}p7.jpg",
            };
            string[] vectCapt = new String[]
            {
                "POUUUUUUULPE!",
                "Quelqu'un a parlé de Takoyakis?",
                "FAAAAABULEUX!",
                "Un beau cadeau pour...naaaan oublie",
                "C mi ke jsui le + bô",
                "La vie est beeeeeelle!",
                "Plop mon poulpe",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!"
            };
            try
            {
                await this.Context.Channel.SendFileAsync(vectImgs[this._rand.Next(vectImgs.Length)], $"{vectCapt[this._rand.Next(vectCapt.Length)]}");
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                return;
            }
        }

        [Command("say")]
        public async Task SendSayMessage([Remainder] string input = "")
        {
            await this.Context.Message.DeleteAsync();
            await ReplyAsync(input);
        }

        [Command("help"), Summary("Envoie la liste des commandes disponibles en mp")]
        public async Task SendHelp()
        {
            string rp = "";
            string autre = "";
            rp += "\n";
            rp += "Liste des commandes: `help`\n";
            rp += "Aide sur la commande bank: `bank help`\n";
            rp += "Aide sur la commande stuff: `stuff help`\n";
            autre += "Envoyer une image de poulpe avec un message aléatoire: `plop`\n";
            rp += "Lancer un dé: `roll 1d100`\n";
            autre += "Faire parler le bot (c useless): `say message`\n";
            autre += "Saluer l'utilisateur qui a envoyé la commande: `hello`\n";
            autre += "(staff) Tester le message de bienvenue sur le serveur: `testjoin`\n";
            try
            {
                await ReplyAsync("Infos envoyées en mp");
                Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle("**Liste des commandes disponibles**")
                    .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                    .AddField("=========== Commandes RP ===========", rp)
                    .AddField("========= Autres Commandes =========", autre);
                //await this.Context.User.SendMessageAsync(infoAccount.ToString());
                await this.Context.User.SendMessageAsync("", false, eb.Build());
                Logs.WriteLine(rp);
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                return;
            }
        }

        #endregion
    }
}
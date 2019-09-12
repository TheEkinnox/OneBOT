#region MÉTADONNÉES

// Nom du fichier : BaseCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-08
// Date de modification : 2019-09-11

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
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
            string imgsPath = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Data\Plop\");
            List<string> plopImgs = new List<string>();
            foreach (string file in Directory.GetFiles(imgsPath, "*.jpg"))
                plopImgs.Add(file);

            List<string> captions = new List<string>()
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
                await this.Context.Channel.SendFileAsync(plopImgs[this._rand.Next(plopImgs.Count)], $"{captions[this._rand.Next(captions.Count)]}");
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
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

        [Command("testwanted"), Alias("tw")]
        public async Task SendTestWanted(string type = "Dead or alive")
        {
            await Global.ChargerDonneesPersosAsync();
            Character persoTest = await Global.GetCharacterByNameAsync("test");
            if (persoTest == null)
            {
                Global.Characters.Add(new Character
                {
                    Nom = "D. Test",
                    Prenom = "Plop",
                    CompteEnBanque = new BankAccount
                    {
                        Montant = 5000,
                        Salaire = 0
                    },
                    Age = 25,
                    Camps = Side.Revolutionnaire,
                    NomImagePerso = "plopdtest.png",
                    Prime = 100000000,
                    Race = "Cyborg",
                    OwnerId = 260385529474842626
                });
                await Global.EnregistrerDonneesPersosAsync();
                persoTest = Global.Characters[Global.Characters.Count - 1];
            }
            bool dead = type.ToLower().Contains("dead");
            bool alive = type.ToLower().Contains("alive");
            await Global.GenererAvisDeRecherche(persoTest, dead, alive);
            await this.Context.Channel.SendFileAsync(Global.CheminImagesWanted + persoTest.Prenom + ".png");
        }

        #endregion
    }
}
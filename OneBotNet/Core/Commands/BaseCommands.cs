#region MÉTADONNÉES

// Nom du fichier : BaseCommands.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-08
// Date de modification : 2019-09-22

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

namespace OneBotNet.Core.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private readonly Random _rand = new Random();
        #endregion

        #region MÉTHODES

        [Command("hello"), Summary("Commande hello")]
        public async Task SendMessage()
        {
            await this.Context.Channel.SendMessageAsync($"Wsh {this.Context.User.Mention}, bien ou bien ma couille");
        }

        [Command("testembed"), Alias("embed", "te", "emb")]
        public async Task SendEmbed()
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
        public async Task SendTestWanted(string character = "test", string type = "Dead or alive")
        {
            try
            {
                await Global.ChargerDonneesPersosAsync();
                Character persoTest = await Global.GetCharacterByNameAsync(character);
                if (persoTest == null && (character.Contains("test") || character.Contains("plop")))
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
                        LienImage = "https://cdn.discordapp.com/attachments/549842832970612746/621728425799057420/plopdtest.png",
                        Prime = 100000000,
                        Race = "Cyborg",
                        OwnerId = 260385529474842626,
                        Dead = type.ToLower().Contains("dead"),
                        Alive = type.ToLower().Contains("alive")
                    });
                    await Global.EnregistrerDonneesPersosAsync();
                    persoTest = Global.Characters[Global.Characters.Count - 1];
                }
                else if (persoTest == null)
                    throw new ArgumentNullException("Le personnage demandé est introuvable...");

                await Global.GenererAvisDeRecherche(persoTest, persoTest.Dead, persoTest.Alive);
                await this.Context.Channel.SendFileAsync(Global.CheminImagesWanted + persoTest.NomImagePerso);
            }
            catch (Exception e)
            {
                Logs.WriteLine("Erreur de la commande testwanted: " + e.Message);
                await ReplyAsync(e.Message);
            }
        }

        [Command("ping")]
        public async Task SendPing()
        {
            await ReplyAsync($"Latence: {Global.Context.Client.Latency}ms");
        }
        [Command("testdate")]
        public async Task SendTestDate()
        {
            ItemToSell toSell = new ItemToSell
            {
                Nom = "ObjTest",
                Description = "DescTest",
                DateAjout = DateTime.Parse("30/03/2002"),
                NomVendeur = "VendeurTest",
                PrixAchat = 5000,
                PrixVente = 2500,
                Quantite = 255,
                TempsAffichage = new TimeSpan(8, 8, 8, 8)
            };
            string msg = $"Aujourd'hui: {DateTime.UtcNow.ToString()}\nFin d'affichage: {(toSell.DateAjout + toSell.TempsAffichage).ToString()}\nPlus tôt ajd?: {(DateTime.UtcNow < toSell.DateAjout + toSell.TempsAffichage).ToString()}\nPlus tard ajd?: {(DateTime.UtcNow > toSell.DateAjout + toSell.TempsAffichage).ToString()}";
            Logs.WriteLine(msg);
            await ReplyAsync(msg);
        }

        [Command("roll-race")]
        public async Task RollRaceTask()
        {
            string race = Global.Random.Next(1,7) switch
            {
                1 => "Humain",
                2 => "Homme poisson",
                3 => "Géant",
                4 => "Mink",
                5 => "Nain",
                6 => "Cyborg",
                _ => throw new Exception()
            };
            await ReplyAsync($"Ton prochain perso sera un **{race}**!!");
        }

        [Command("roll-fruit")]
        public async Task RollFruitTask(string strTier = "0")
        {
            if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
            {
                await this.Context.Channel.SendMessageAsync($"La commande **{this.Context.Message.Content}** est réservée aux admins pour le moment.");
                throw new UnauthorizedAccessException($"Seuls les admins peuvent roll fruit mais {this.Context.User.Username} à tenté.");
            }

            if (!int.TryParse(strTier, out int tier) || tier < 0)
            {
                await this.Context.Channel.SendMessageAsync($"Le rang du fruit doit être un nombre entier entre 0(random) et {Global.FruitTiers.Count}...");
                throw new ArgumentException($"Le tier du fruit doit être un nombre entier entre 0(random) et {Global.FruitTiers.Count}.");
            }
            if (tier == 0)
                tier = Global.Random.Next(1,Global.FruitTiers.Count+1);
            
            string fruit = Global.FruitTiers[tier-1][Global.Random.Next(0,Global.FruitTiers[tier-1].Count)];
            await ReplyAsync($"Ton fruit sera le **{fruit}** !!");
        }
        #endregion
    }
}
#region MÉTADONNÉES

// Nom du fichier : RepeatingTimer.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-27
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    /// <summary>
    /// Classe permettant la génération d'une horloge
    /// </summary>
    public static class RepeatingTimer
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        private static Timer _loopingTimer;
        private static List<BankAccount> _initialBankAccounts;
        private static SocketTextChannel[] _banques;

        static string _cheminComptesEnBanque = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Ressources\Database\bank.altr");
        static BankAccount _methodes = new BankAccount("");
        private static bool _salaireVerse = false;
        private const DayOfWeek jourSalaire = DayOfWeek.Friday;
        private static int _heureSalaire = DateTime.Now.Hour;
        private static int _minuteSalaire = DateTime.Now.Minute;

        private static int _ticksPasses = 3;

        #endregion

        #region MÉTHODES

        internal static Task StartTimer()
        {
            RepeatingTimer._initialBankAccounts = RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque).GetAwaiter().GetResult();
            RepeatingTimer._banques = new SocketTextChannel[]
            {
                //OnePieceRP
                Global.Client.GetGuild(549301561478873105).GetTextChannel(551091430152470528),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(541493264180707338)
            };

            RepeatingTimer._loopingTimer = new Timer()
            {
                Interval = 15000,
                AutoReset = true,
                Enabled = true
            };
            RepeatingTimer._loopingTimer.Elapsed += RepeatingTimer.OnTimerTicked;
            Logs.WriteLine("StartTimer");
            return Task.CompletedTask;
        }

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
            => RepeatingTimer.OnTimerTickedAsync(sender, e).GetAwaiter().GetResult();

        private static async Task OnTimerTickedAsync(object sender, ElapsedEventArgs e)
        {
            Logs.WriteLine("Timer ticked");

            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =============================================
            // = Actualise la liste dans le channel banque =
            // =============================================
            List<BankAccount> updatedBankAccounts = RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque).GetAwaiter().GetResult();
            if (!updatedBankAccounts.Equals(RepeatingTimer._initialBankAccounts))
            {
                RepeatingTimer._ticksPasses++;
                Logs.WriteLine(RepeatingTimer._ticksPasses.ToString());
                if (RepeatingTimer._ticksPasses >= 120)
                {
                    try
                    {
                        RepeatingTimer._methodes.EnregistrerDonneesPersos(RepeatingTimer._cheminComptesEnBanque, updatedBankAccounts);
                        RepeatingTimer._initialBankAccounts = updatedBankAccounts;
                        Logs.WriteLine("Comptes en banque mis à jour");
                        await Program.UpdateBank(RepeatingTimer._banques);
                    }
                    catch (Exception exception)
                    {
                        Logs.WriteLine(exception.ToString());
                        throw;
                    }

                    RepeatingTimer._ticksPasses = 0;
                }
            }
        }

        #endregion
    }
}
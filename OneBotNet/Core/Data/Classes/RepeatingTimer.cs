#region MÉTADONNÉES

// Nom du fichier : RepeatingTimer.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-08
// Date de modification : 2019-09-17

#endregion

#region USING

using System;
using System.Threading.Tasks;
using System.Timers;
using Discord;

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

        private static bool _salaireVerse;
        private const DayOfWeek jourSalaire = DayOfWeek.Sunday;
        private static readonly int heureSalaire = 18;
        private static readonly int minuteSalaire = 00;

        private static int _ticksPasses = 120;

        #endregion

        #region MÉTHODES

        internal static Task StartTimer()
        {
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
            => RepeatingTimer.OnTimerTickedAsync().GetAwaiter().GetResult();

        private static async Task OnTimerTickedAsync()
        {
            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            if (DateTime.UtcNow.DayOfWeek == RepeatingTimer.jourSalaire &&
                DateTime.UtcNow.Hour == RepeatingTimer.heureSalaire &&
                DateTime.UtcNow.Minute == RepeatingTimer.minuteSalaire && !RepeatingTimer._salaireVerse &&
                RepeatingTimer._ticksPasses >= 3)
            {
                Logs.WriteLine("Versement des salaires");
                await Global.VerserSalairesAsync();

                RepeatingTimer._ticksPasses = 120;
                RepeatingTimer._salaireVerse = true;
                Logs.WriteLine("Salaires versés");
            }
            else if (DateTime.Now.Minute != RepeatingTimer.minuteSalaire && RepeatingTimer._salaireVerse)
                RepeatingTimer._salaireVerse = false;

            if (Global.Client.LoginState != LoginState.LoggedIn)
                Program.Main();
        }

        #endregion
    }
}
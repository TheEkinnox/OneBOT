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
        private static Timer _loopingTimer;

        //internal static Task StartTimer()
        //{

        //    RepeatingTimer._loopingTimer = new Timer()
        //    {
        //         Interval = 15000,
        //         AutoReset = true,
        //         Enabled=true
        //    };
        //RepeatingTimer._loopingTimer.Elapsed += RepeatingTimer.OnTimerTicked;
        //    Logs.WriteLine("StartTimer");
        //    return Task.CompletedTask;
        //}

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
            => RepeatingTimer.OnTimerTickedAsync().GetAwaiter().GetResult();

        private static bool _salaireVerse = false;
        private const DayOfWeek jourSalaire = DayOfWeek.Sunday;
        private static int _heureSalaire = 18;
        private static int _minuteSalaire = 00;

        private static int _ticksPasses = 120;

        private static async Task OnTimerTickedAsync()
        {
            Logs.WriteLine("Timer ticked");

            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer._heureSalaire && DateTime.Now.Minute == RepeatingTimer._minuteSalaire && !RepeatingTimer._salaireVerse && RepeatingTimer._ticksPasses >= 3)
            {
                Logs.WriteLine("Versement des salaires");
                await Global.VerserSalairesAsync();

                RepeatingTimer._ticksPasses = 120;
                RepeatingTimer._salaireVerse = true;
                Logs.WriteLine("Salaires versés");
            }
            else if (DateTime.Now.Minute != RepeatingTimer._minuteSalaire && RepeatingTimer._salaireVerse)
                RepeatingTimer._salaireVerse = false;

            if (Global.Client.LoginState != Discord.LoginState.LoggedIn)
                Program.Main();
        }
    }
}
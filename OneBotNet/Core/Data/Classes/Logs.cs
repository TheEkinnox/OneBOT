#region MÉTADONNÉES

// Nom du fichier : Logs.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-27
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.IO;
using System.Reflection;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    public static class Logs
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        private static string _nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Data\Logs\");
        private static string _date = $"{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}.altr";

        #endregion

        #region MÉTHODES

        public static void WriteLine(string args)
        {
            StreamWriter fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, true);
            fluxEcriture.Close();
            StreamReader fluxLecture = new StreamReader(Logs._nomFichier + Logs._date);
            string fichierTexte = fluxLecture.ReadToEnd();
            fluxLecture.Close();

            fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, false);
            fichierTexte = fichierTexte.EndsWith('\n') ? fichierTexte.Remove(fichierTexte.Length - 1) : fichierTexte;
            fichierTexte += $"\n{DateTime.Now} {args}";
            fluxEcriture.WriteLine($"{fichierTexte}");
            fluxEcriture.Close();
            Console.WriteLine($"{DateTime.Now} {args}");
        }

        public static void Write(string args)
        {
            StreamWriter fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, true);
            fluxEcriture.Close();
            StreamReader fluxLecture = new StreamReader(Logs._nomFichier + Logs._date);
            string fichierTexte = fluxLecture.ReadToEnd().Replace("\r", "");
            fluxLecture.Close();
            fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, false);
            fichierTexte = fichierTexte.EndsWith('\n') ? fichierTexte.Remove(fichierTexte.Length - 1) : fichierTexte;
            fichierTexte += $"\n{DateTime.Now} {args}";
            fluxEcriture.Write($"{fichierTexte}");
            fluxEcriture.Close();
            Console.Write($"{DateTime.Now} {args}");
        }

        #endregion
    }
}
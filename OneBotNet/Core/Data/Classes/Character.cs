#region MÉTADONNÉES

// Nom du fichier : Character.cs
// Auteur : (Loïck Noa Obiang Ndong)
// Date de création : 2019-09-08
// Date de modification : 2019-09-13

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    public class Character
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public int Age { get; set; }
        public string Sexe { get; set; }
        public string Race { get; set; }
        public Side Camps { get; set; }
        public decimal Prime { get; set; }
        public int Reputation { get; set; }
        public BankAccount CompteEnBanque { get; set; }
        public List<string> Items { get; set; }
        public String NomImagePerso { get; set; }
        public String LienImage { get; set; }
        public ulong OwnerId { get; set; }
        public bool Dead{get;set;}
        public bool Alive{get; set; }

        #endregion
    }

    public enum Side
    {
        [XmlEnum(Name = "Pirate")] Pirate,
        [XmlEnum(Name = "Marine")] Marine,
        [XmlEnum(Name = "Revolutionnaire")] Revolutionnaire
    }
}
#region MÉTADONNÉES

// Nom du fichier : Item.cs
// Auteur :  (Loïck Obiang Ndong)
// Date de création : 2019-09-20
// Date de modification : 2019-09-20

#endregion

#region USING

using System;
using System.Threading.Tasks;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    public class Item
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public string Nom { get; set; }
        public string Description { get; set; }
        public decimal PrixAchat { get; set; }
        public decimal PrixVente { get; set; }

        #endregion
    }


    public class ItemToSell : Item
    {
        #region PROPRIÉTÉS ET INDEXEURS

        internal string NomVendeur { get; set; }
        internal DateTime DateAjout { get; set; }
        internal TimeSpan TempsAffichage { get; set; }
        internal int Quantite { get; set; }


        #endregion
    }
}
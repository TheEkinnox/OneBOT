#region MÉTADONNÉES

// Nom du fichier : BankAccount.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-27
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace OneBotNet.Core.Data.Classes
{
    public class BankAccount
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        private const decimal salaire = 0;

        #endregion

        #region PROPRIÉTÉS ET INDEXEURS

        public string Name { get; set; }
        public decimal Amount { get; set; }
        public ulong UserId { get; set; }
        public decimal Salaire { get; set; }

        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Constructeur permettant l'initialisation d'un compte en banque
        /// </summary>
        /// <param name="name">Nom du propriétaire du compte</param>
        /// <param name="amount">Montant disponible sur le compte</param>
        /// <param name="userId">ID Discord du créateur du compte</param>
        /// <param name="salaire">Salaire du compte</param>
        public BankAccount(string name, decimal amount = 500, ulong userId = 0, decimal salaire = 0)
        {
            this.Name = name;
            this.Amount = amount;
            this.UserId = userId;
            this.Salaire = salaire;
        }

        #endregion

        #region MÉTHODES

        public void Deposit(decimal montant)
        {
            this.Amount += montant;
        }

        public void Withdraw(decimal montant)
        {
            this.Amount -= montant;
        }

        public void EnregistrerDonneesPersos(string cheminFichier, List<BankAccount> savedBankAccounts)
        {
            StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

            String personneTexte;
            for (int i = 0; i < savedBankAccounts.Count; i++)
            {
                if (savedBankAccounts[i] != null)
                {
                    personneTexte = savedBankAccounts[i].Name + "," + savedBankAccounts[i].Amount + "," +
                                    savedBankAccounts[i].Salaire + "," + savedBankAccounts[i].UserId;

                    fluxEcriture.WriteLine(personneTexte);
                }
            }

            fluxEcriture.Close();
        }

        public async Task<List<BankAccount>> ChargerDonneesPersosAsync(string cheminFichier)
        {
            StreamReader fluxLecture = new StreamReader(cheminFichier);

            String fichierTexte = fluxLecture.ReadToEnd();
            fluxLecture.Close();

            fichierTexte = fichierTexte.Replace("\r", "");

            String[] vectLignes = fichierTexte.Split('\n');

            int nbLignes = vectLignes.Length;

            if (vectLignes[vectLignes.Length - 1] == "")
            {
                nbLignes = vectLignes.Length - 1;
            }

            BankAccount[] bankAccounts = new BankAccount[nbLignes];


            String[] vectChamps;
            string name;
            decimal amount;
            decimal salaire;
            ulong userId;

            for (int i = 0; i < bankAccounts.Length; i++)
            {
                vectChamps = vectLignes[i].Split(',');
                name = vectChamps[0].Trim();
                amount = decimal.Parse(vectChamps[1]);
                salaire = decimal.Parse(vectChamps[2]);
                userId = ulong.Parse(vectChamps[3]);

                bankAccounts[i] = new BankAccount(name, amount, userId, salaire);
            }

            return bankAccounts.ToList();
        }

        public List<BankAccount> ChargerDonneesPersos(string cheminFichier)
            => new BankAccount("").ChargerDonneesPersosAsync(cheminFichier).GetAwaiter().GetResult();


        public async Task<BankAccount> GetBankAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
            BankAccount userAccount = null;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
                {
                    userAccount = regAccounts[i];
                }
            }

            return userAccount;
        }

        public BankAccount GetBankAccountByName(string nomFichier, string nomPerso)
            => new BankAccount("").GetBankAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<int> GetBankAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
            int userAccountIndex = -1;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
                {
                    userAccountIndex = i;
                }
            }

            return userAccountIndex;
        }

        public int GetBankAccountIndexByName(string nomFichier, string nomPerso)
            => new BankAccount("").GetBankAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<List<string>> AccountsListAsync(string nomFichier)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
            List<string> message = new List<string>();
            int lastIndex = 0;
            for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
            {
                try
                {
                    message.Add("");

                    for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
                    {
                        try
                        {
                            message[i] += $"{regAccounts[j].ToString()}\n";
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.ToString());
                            throw;
                        }
                    }

                    lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
                }
                catch (Exception e)
                {
                    Logs.WriteLine(e.ToString());
                    throw;
                }
            }

            return message;
        }

        public List<string> AccountsList(string nomFichier)
            => new BankAccount("").AccountsListAsync(nomFichier).GetAwaiter().GetResult();

        public override string ToString()
        {
            string message = $"**{this.Name + ":**"} {this.Amount} couronne(s)\n{"Salaire" + ":"} {this.Salaire} couronne(s)";
            //string message = string.Format("**{0}:** {this.Amount} couronne(s)\nsalaire: {this.Salaire} couronne(s)", this.Name);
            return message;
        }

        #endregion
    }
}
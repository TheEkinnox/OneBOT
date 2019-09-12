using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Discord;

namespace OneBotNet.Core.Data.Classes
{
    public class Character
    {
        public string Nom{get; set; }
        public string Prenom{get;set;}
        public int Age {get;set;}
        public string Race { get; set; }
        public Side Camps {get;set;}
        public decimal Prime {get;set;}
        public BankAccount CompteEnBanque{get; set; }
        public List<string> Items{get;set;}
        public String NomImagePerso{get;set;}
        public ulong OwnerId{get;set;}
    }

    public enum Side
    {
        [XmlEnum(Name = "Pirate")]Pirate,
        [XmlEnum(Name = "Marine")]Marine,
        [XmlEnum(Name = "Revolutionnaire")]Revolutionnaire
    }
}

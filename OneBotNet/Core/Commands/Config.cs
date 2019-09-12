using System;
using System.Collections.Generic;
using System.Text;
using OneBotNet.Core.Data.Classes;

namespace OneBotNet.Core.Commands
{
    public static class Config
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public static string Version
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("version")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("version")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string PrefixPrim
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("prefixprim")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("prefixprim")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string PrefixSec
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("prefixsec")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("prefixsec")[0].InnerText = value;
                    Global.ConfigXml.Save(Global.CheminConfig);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string Motd
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("motd")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("motd")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string WelcomeMessage
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("welcomemessage")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du message de bienvenue avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("welcomemessage")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du message de bienvenue avec le message suivant : " + e.Message);
                }
            }
        }

        #endregion
    }
}

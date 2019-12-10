#region USING

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace OneBotNet.Core.Data.Classes
{
	public static class Global
	{
		#region CONSTANTES ET ATTRIBUTS STATIQUES

		internal static string CheminConfig = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Core\Data\config.opno");

		internal static string CheminImagesPersos = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Data\CharacterImages\");

		internal static string CheminImagesWanted = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Data\Wanted\");

		private static Bitmap _bmpWantedVide = Image.FromFile(Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Data\Wanted\emptywanted.png")) as Bitmap;

		private static string _cheminPersosXml = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Ressources\Database\persos.opno");

		internal static string CheminDossierFonts = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\OneBotNet.dll", @"Data\Fonts\");

		internal static XmlDocument ConfigXml = Global.ChargerConfigXml();

		internal static List<Character> Characters = new List<Character>();
		private static List<BankAccount> _bankAccounts = new List<BankAccount>();

		internal static List<List<string>> FruitTiers = new List<List<string>>
		{
			new List<string>
			{
				"fruit 1_1",
				"fruit 1_2",
				"fruit 1_3",
				"fruit 1_4",
				"fruit 1_5",
				"fruit 1_6",
				"fruit 1_7",
				"fruit 1_8",
				"fruit 1_9",
				"fruit 1_10",
				"fruit 1_11",
				"fruit 1_12",
			},
			new List<string>
			{
				"Ope Ope no mi",
				"Gura Gura no mi",
				"Tori Tori no mi, modèle: phoenix",
				"Fruit du magnétisme",
				"Ito Ito no mi",
				"Ryu Ryu no mi",
				"Fruit de Kaido",
				"Nikyu Nikyu no Mi",
				"Hie Hie no Mi",
				"Hito Hito no mi, modèle: daibutsu",
			}
		};

		internal static Random Random = new Random();

		#endregion

		#region PROPRIÉTÉS ET INDEXEURS

		internal static DiscordSocketClient Client { get; set; }
		internal static SocketCommandContext Context { get; set; }
		internal static List<SocketTextChannel> ChannelsBanques { get; set; }
		internal static List<SocketTextChannel> ChannelsPrime { get; set; }
		internal static List<SocketTextChannel> ChannelsReput { get; set; }
		internal static List<SocketTextChannel> StuffLists { get; set; }
		internal static List<SocketTextChannel> MarketsLists { get; set; }
		internal static List<ItemToSell> MarketPlace { get; set; }

		#endregion

		#region MÉTHODES

		/// <summary>
		/// Vérifie si l'utilisateur est membre du Staff ou non
		/// </summary>
		/// <param name="user">Utilisateur à vérifier</param>
		/// <returns>True si l'utilisateur est membre du Staff ou false sinon</returns>
		public static bool IsStaff(SocketGuildUser user)
		{
			string targetRoleName = "Staff";
			return Global.HasRole(user, targetRoleName);
		}

		/// <summary>
		/// Vérifie si l'utilisateur possède le role indiqué ou non
		/// </summary>
		/// <param name="user">Utilisateur à vérifier</param>
		/// <param name="roleName">Nom du role à vérifier</param>
		/// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
		public static bool HasRole(SocketGuildUser user, string roleName)
		{
			string targetRoleName = roleName;
			IEnumerable<ulong> result = from r in user.Guild.Roles
				where r.Name == targetRoleName
				select r.Id;
			ulong roleId = result.FirstOrDefault();
			if (roleId == 0) return false;
			SocketRole targetRole = user.Guild.GetRole(roleId);
			return user.Roles.Contains(targetRole);
		}

		/// <summary>
		/// Vérifie si l'utilisateur possède le role indiqué ou non
		/// </summary>
		/// <param name="roleName">Nom du role à vérifier</param>
		/// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
		public static SocketRole GetRoleByName(SocketCommandContext context, string roleName)
		{
			string targetRoleName = roleName;
			IEnumerable<ulong> result = from r in context.Guild.Roles
				where r.Name == targetRoleName
				select r.Id;
			ulong roleId = result.FirstOrDefault();
			if (roleId == 0) throw new ArgumentException("Le role recherché n'existe pas");
			return context.Guild.GetRole(roleId);
		}

		public static async Task EnregistrerDonneesPersosAsync()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Character>));
			StreamWriter writer = new StreamWriter(Global._cheminPersosXml);
			serializer.Serialize(writer, Global.Characters);
			writer.Close();
		}

		public static async Task ChargerDonneesPersosAsync()
		{
			if (!File.Exists(Global._cheminPersosXml))
			{
				await Global.EnregistrerDonneesPersosAsync();
				return;
			}

			XmlSerializer serializer = new XmlSerializer(typeof(List<Character>));
			StreamReader reader = new StreamReader(Global._cheminPersosXml);
			Global.Characters = serializer.Deserialize(reader) as List<Character>;
			reader.Close();
		}

		public static async Task<Character> GetCharacterByNameAsync(string nomPerso)
		{
			int index = await Global.GetCharacterIndexByNameAsync(nomPerso);
			return index > -1 ? Global.Characters[index] : null;
		}

		public static Character GetCharacterByName(string nomPerso)
			=> Global.GetCharacterByNameAsync(nomPerso).GetAwaiter().GetResult();
		//
		//
		//

		public static void ChargerDonneesPersos()
			=> Global.ChargerDonneesPersosAsync().GetAwaiter().GetResult();

		public static string GetInfoPerso(Character character)
			=> Global.GetInfoPersoAsync(character).GetAwaiter().GetResult();

		public static async Task<string> GetInfoPersoAsync(Character character)
		{
			string msg = $"__**Age:**__ {character.Age}" +
			             $"\n__**Sexe:**__ {character.Sexe}" +
			             $"\n__**Camps:**__ {character.Camps}" +
			             $"\n__**Race:**__ {character.Race}" +
			             $"{(character.Prime > 0 ? "\n__**Prime:**__ " + $"{character.Prime:c0}".Replace("$", "berry").Replace("€", "berry").Trim() : "")}" +
			             $"{(character.Reputation > 0 ? "\n__**Réputation:**__ " + $"{character.Reputation:c0}".Replace("$", "").Replace("€", "").Trim() : "")}" +
			             $"\n__**Propriétaire:**__ {Global.Context.Guild.GetUser(character.OwnerId).Mention}";
			return msg;
		}

		/// <summary>
		/// Méthode permettant d'ajouter le salaire défini pour un personnage au dit personnage
		/// </summary>
		public static async Task VerserSalaireAsync(Character character)
		{
			Global.ChargerDonneesBank();
			if (character != null)
			{
				decimal bankSalaire = character.CompteEnBanque.Salaire;
				decimal ancienMontant = character.CompteEnBanque.Montant;
				decimal nvMontant = ancienMontant + bankSalaire;
				Global.Characters[Global.GetCharacterIndexByName(character.Nom)].CompteEnBanque = new BankAccount
				{
					Salaire = bankSalaire,
					Montant = nvMontant
				};
				await Global.EnregistrerDonneesPersosAsync();
				Logs.WriteLine($"Salaire de {character.Prenom} ({bankSalaire} berry) versé");
			}
		}

		public static async Task VerserSalairesAsync()
		{
			Global.ChargerDonneesBank();

			for (int i = 0; i < Global._bankAccounts.Count; i++)
			{
				try
				{
					Character depositAccount = Global.Characters[i];
					if (depositAccount != null)
						await Global.VerserSalaireAsync(depositAccount);
				}
				catch (Exception exception)
				{
					Logs.WriteLine(exception.ToString());
				}
			}
		}

		public static async Task<BankAccount> GetBankAccountByNameAsync(string nomPerso)
			=> (await Global.GetCharacterByNameAsync(nomPerso)).CompteEnBanque;

		public static BankAccount GetBankAccountByName(string nomPerso)
			=> Global.GetBankAccountByNameAsync(nomPerso).GetAwaiter().GetResult();

		public static async Task<int> GetCharacterIndexByNameAsync(string nomPerso)
		{
			await Global.ChargerDonneesPersosAsync();
			for (int i = 0; i < Global.Characters.Count; i++)
			{
				Character c = Global.Characters[i];
				if (nomPerso.ToLower().Contains(c.Nom) || nomPerso.ToLower().Contains(c.Prenom.ToLower()) || c.Nom.ToLower().Contains(nomPerso.ToLower()) || c.Prenom.ToLower().Contains(nomPerso.ToLower()))
					return i;
			}

			throw new KeyNotFoundException("Le personnage recherché est introuvable. Veuillez revérifier l'orthographe et réessayer s.v.p.");
		}

		public static int GetCharacterIndexByName(string nomPerso)
			=> Global.GetCharacterIndexByNameAsync(nomPerso).GetAwaiter().GetResult();


		public static void ChargerDonneesBank()
		{
			Global.ChargerDonneesPersos();
			List<BankAccount> bankAccounts = new List<BankAccount>();
			foreach (Character character in Global.Characters)
				bankAccounts.Add(character.CompteEnBanque);

			Global._bankAccounts = bankAccounts;
		}

		public static async Task<string> GetBankInfoAsync(BankAccount bankAccount)
		{
			if (bankAccount == null)
				throw new ArgumentNullException(null, "Le compte en banque demandé est inexistant");
			string message = ($"__**Montant:**__ {bankAccount.Montant:c0}" +
			                  $"\n__**Salaire:**__ {bankAccount.Salaire:c0}").Replace("$", "berry").Replace("€", "berry");
			return message;
		}

		// ========================
		// = TODO méthodes Market =
		// ========================
		public static async Task AddItemToSell(ItemToSell itemToSell)
        {

        }
		// ========================
		// = TODO méthodes Wanted =
		// ========================
		public static async Task<Bitmap> GenererAvisDeRecherche(Character perso, bool dead = true, bool alive = true)
		{
			int offsety = 0;
			if (!File.Exists(Global.CheminImagesPersos + perso.NomImagePerso))
				Global.SaveImageFromUrl(perso.LienImage, Global.CheminImagesPersos + perso.NomImagePerso, ImageFormat.Png);
			Bitmap imagePerso = Image.FromFile(Global.CheminImagesPersos + perso.NomImagePerso) as Bitmap;
			Bitmap imagePersoTemp;
			float bmp2ScaleWidth = 1f * imagePerso.Width / imagePerso.Height;
			float bmp2ScaleHeight = 1f * imagePerso.Height / imagePerso.Width;
			if (338 * bmp2ScaleWidth >= 446 && imagePerso.Height != 338)
			{
				imagePersoTemp = Global.ResizeImage(imagePerso, (int) (338 * bmp2ScaleWidth), 338);
				imagePerso = imagePersoTemp;
			}

			if (446 * bmp2ScaleHeight >= 338 && imagePerso.Width != 446)
			{
				imagePersoTemp = Global.ResizeImage(imagePerso, 446, (int) (446 * bmp2ScaleHeight));
				imagePerso = imagePersoTemp;
			}

			int offsetx = 0;
			if (imagePerso.Width != 446)
				offsetx = (imagePerso.Width - 446) / 2;
			if (imagePerso.Height < 338)
				offsety = (imagePerso.Height - 338) / 2;
			Bitmap tempBitmap = Global.Superimpose(Global._bmpWantedVide, imagePerso, false, 182, 169, offsetx, offsety);
			Font nameFont = new Font(FontFamily.GenericSerif, 256f, FontStyle.Bold);
			Font bountyFont = new Font(FontFamily.GenericSerif, 150f, FontStyle.Bold);
			Bitmap nameBmp = Global.DrawTextImage($"{perso.Prenom} {perso.Nom}", nameFont, Color.FromArgb(230, 33, 25, 22), Color.FromArgb(0, 255, 255, 255)) as Bitmap;
			float nameBmpScaleWidth = 1f * nameBmp.Width / nameBmp.Height;
			float nameBmpScaleHeight = 1f * nameBmp.Height / nameBmp.Width;
			if (nameBmp.Width != 442)
				nameBmp = Global.ResizeImage(nameBmp, 442, (int) (442 * nameBmpScaleHeight));

			if (nameBmp.Width <= nameBmp.Height && nameBmp.Height != 84 || nameBmp.Height > 84)
				nameBmp = Global.ResizeImage(nameBmp, (int) (84 * nameBmpScaleWidth), 84);

			offsetx = -((442 - nameBmp.Width) / 2);
			offsety = -((84 - nameBmp.Height) / 2);
			Bitmap tempBitmap2 = Global.Superimpose(tempBitmap, nameBmp, true, 176, 575, offsetx, offsety);
			Bitmap bountyBmp = perso.Camps != Side.Marine ? Global.DrawTextImage(perso.Prime < 0 ? "N/A" : $"{perso.Prime:c0}".Replace("$", "").Trim(), bountyFont, Color.FromArgb(230, 33, 25, 22), Color.FromArgb(0, 255, 255, 255)) as Bitmap : Global.DrawTextImage(perso.Reputation < 0 ? "N/A" : $"{perso.Reputation:c0}".Replace("$", "").Trim(), bountyFont, Color.FromArgb(230, 33, 25, 22), Color.FromArgb(0, 255, 255, 255)) as Bitmap;
			float bountyBmpScaleWidth = 1f * bountyBmp.Width / bountyBmp.Height;
			float bountyBmpScaleHeight = 1f * bountyBmp.Height / bountyBmp.Width;
			if (bountyBmp.Width != 393)
				bountyBmp = Global.ResizeImage(bountyBmp, 393, (int) (393 * bountyBmpScaleHeight));

			if (bountyBmp.Width <= bountyBmp.Height && bountyBmp.Height != 62 || bountyBmp.Height > 62)
				bountyBmp = Global.ResizeImage(bountyBmp, (int) (62 * bountyBmpScaleWidth), 62);
			offsetx = -((393 - bountyBmp.Width) / 2);
			offsety = -((62 - bountyBmp.Height) / 2);
			tempBitmap = Global.Superimpose(tempBitmap2, bountyBmp, true, 225, 669, offsetx, offsety);
			string deadOrAliveText = "DEAD OR ALIVE";
			if (dead && !alive)
				deadOrAliveText = "ONLY DEAD";
			else if (!dead && alive)
				deadOrAliveText = "ONLY ALIVE";
			Bitmap bmpDeadOrAlive = Global.DrawTextImage(deadOrAliveText, nameFont, Color.FromArgb(230, 33, 25, 22), Color.FromArgb(0, 255, 255, 255)) as Bitmap;
			float deadOrAliveScaleWidth = 1f * bmpDeadOrAlive.Width / bmpDeadOrAlive.Height;
			float deadOrAliveScaleHeight = 1f * bmpDeadOrAlive.Height / bmpDeadOrAlive.Width;
			if (bmpDeadOrAlive.Width > 450)
				bmpDeadOrAlive = Global.ResizeImage(bmpDeadOrAlive, 450, (int) (450 * deadOrAliveScaleHeight));

			if (bmpDeadOrAlive.Width > bmpDeadOrAlive.Height && bmpDeadOrAlive.Height > 52)
				bmpDeadOrAlive = Global.ResizeImage(bmpDeadOrAlive, (int) (52 * deadOrAliveScaleWidth), 52);
			offsetx = -((450 - bmpDeadOrAlive.Width) / 2);
			offsety = -((52 - bmpDeadOrAlive.Height) / 2);
			Bitmap finalBitmap = Global.Superimpose(tempBitmap, bmpDeadOrAlive, true, 174, 518, offsetx, offsety);
			finalBitmap.Save(Global.CheminImagesWanted + perso.NomImagePerso, ImageFormat.Png);
			return finalBitmap;
		}

		public static Bitmap Superimpose(Bitmap bmp1, Bitmap bmp2, bool drawSmallOnTop = true, int posx = 0, int posy = 0, int offsetx = 0, int offsety = 0)
		{
			Bitmap smallImage, bigImage;
			if (bmp1.Width > bmp2.Width || bmp1.Height > bmp2.Height)
			{
				smallImage = bmp2;
				bigImage = bmp1;
			}
			else
			{
				smallImage = bmp1;
				bigImage = bmp2;
			}

			int x = posx - offsetx;
			int y = posy - offsety;
			Bitmap background = new Bitmap(bigImage.Width, bigImage.Height);
			Graphics g = drawSmallOnTop ? Graphics.FromImage(bigImage) : Graphics.FromImage(background);
			g.CompositingMode = CompositingMode.SourceOver;
			g.DrawImage(smallImage, new Point(x, y));
			if (!drawSmallOnTop)
			{
				g.DrawImage(bigImage, new Point(0, 0));
				//background.Dispose();
				return background;
			}

			//background.Dispose();
			return bigImage;
		}

		private static Image DrawTextImage(String currencyCode, Font font, Color textColor, Color backColor)
			=> Global.DrawTextImage(currencyCode, font, textColor, backColor, Size.Empty);

		private static Image DrawTextImage(String currencyCode, Font font, Color textColor, Color backColor, Size minSize)
		{
			//first, create a dummy bitmap just to get a graphics object
			SizeF textSize;
			using (Image img = new Bitmap(1, 1))
			{
				using Graphics drawing = Graphics.FromImage(img);
				//measure the string to see how big the image needs to be
				textSize = drawing.MeasureString(currencyCode, font);
				if (!minSize.IsEmpty)
				{
					textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
					textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
				}
			}

			//create a new image of the right size
			Image retImg = new Bitmap((int) textSize.Width, (int) textSize.Height);
			using (Graphics drawing = Graphics.FromImage(retImg))
			{
				//paint the background
				drawing.Clear(backColor);

				//create a brush for the text
				using Brush textBrush = new SolidBrush(textColor);
				drawing.DrawString(currencyCode, font, textBrush, 0, 0);
				drawing.Save();
			}

			return retImg;
		}

		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			Rectangle destRect = new Rectangle(0, 0, width, height);
			Bitmap destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using ImageAttributes wrapMode = new ImageAttributes();
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}

			return destImage;
		}

		public static void SaveImageFromUrl(string url, string filename, ImageFormat format)
		{
			WebClient client = new WebClient();
			Stream stream = client.OpenRead(url);
			Bitmap bitmap = new Bitmap(stream);


			bitmap.Save(filename, format);
			bitmap.Dispose();
			stream.Flush();
			stream.Close();
			client.Dispose();
		}

		// ===================
		// = Méthodes config =
		// ===================
		private static XmlDocument ChargerConfigXml()
		{
			XmlDocument configXml = new XmlDocument();
			try
			{
				if (!File.Exists(Global.CheminConfig))
					throw new NullReferenceException($"Le fichier est inexistant.");

				configXml.Load(Global.CheminConfig);
			}
			catch (Exception e)
			{
				Logs.WriteLine("Une erreur s'est produite lors du chargement de la configuration du bot avec le message suivant : " + e.Message);
				Environment.Exit(0);
			}

			return configXml;
		}

		public static void EnregistrerConfigXml(XmlDocument nouvelleConfig)
		{
			XmlDocument configXml = new XmlDocument();
			try
			{
				if (!File.Exists(Global.CheminConfig))
					File.Create(Global.CheminConfig).Dispose();

				XmlDeclaration xmlDeclaration = configXml.CreateXmlDeclaration("1.0", "utf-8", null);
				configXml.AppendChild(xmlDeclaration);

				XmlElement root = configXml.CreateElement("config");
				XmlElement version, prefixPrim, prefixSec, motd, welcomeMessage;

				version = configXml.CreateElement("version");
				version.InnerText = nouvelleConfig.GetElementsByTagName("version")[0].InnerText;
				root.AppendChild(version);
				prefixPrim = configXml.CreateElement("prefixprim");
				prefixPrim.InnerText = nouvelleConfig.GetElementsByTagName("prefixprim")[0].InnerText;
				root.AppendChild(prefixPrim);
				prefixSec = configXml.CreateElement("prefixsec");
				prefixSec.InnerText = nouvelleConfig.GetElementsByTagName("prefixsec")[0].InnerText;
				root.AppendChild(prefixSec);
				motd = configXml.CreateElement("motd");
				motd.InnerText = nouvelleConfig.GetElementsByTagName("motd")[0].InnerText;
				root.AppendChild(motd);
				welcomeMessage = configXml.CreateElement("welcomemessage");
				welcomeMessage.InnerText = nouvelleConfig.GetElementsByTagName("welcomemessage")[0].InnerText;
				root.AppendChild(welcomeMessage);
				configXml.AppendChild(root);
				configXml.Save(Global.CheminConfig);
			}
			catch (Exception e)
			{
				Logs.WriteLine("Une erreur s'est produite lors de l'enregistrement de la configuration du bot avec le message suivant : " + e.Message);
				Environment.Exit(0);
			}
		}

		#endregion
	}

	public static class EmbedColors
	{
		#region CONSTANTES ET ATTRIBUTS STATIQUES

		public static Color Marine = Color.FromArgb(52, 152, 219);
		public static Color Revolutionnaire = Color.FromArgb(241, 196, 15);
		public static Color Pirate = Color.Red;
		public static Color BankInfo = Color.Lime;

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace SerwerDrukuZebra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler ErrorWriting = delegate
        {
        };
        public static event EventHandler ErrorReading = delegate
        {
        };

        public static string XMLfile = "Record.xml";
        
        /// <summary>
        /// Sciezka do pliku na pulpicie w ktorym sa zapisywane pliki XML i inne
        /// </summary>
        public static string pathfile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\SerwerDrukuEtykiet";

        private Record dane = new Record();

        /// <summary>
        /// lista w ktorej sa zawierane wszelkie dane
        /// </summary>
        public static RecordList Records;

        private static XmlSerializer RecordSerializer;
        public static XmlTextWriter Writer;
        private static StreamReader Reader;

        private static Random random = new Random();

        /// <summary>
        /// Konstruktor i uruchomienie okna
        /// </summary>
        public MainWindow()
        {
            //zdefiniuj liste i serializer do pracy z XML
            Records = new RecordList();
            RecordSerializer = new XmlSerializer(typeof(RecordList));

            if (File.Exists(pathfile + "\\" + XMLfile))
            {
                //Jesli byla juz praca z plikiem to go zaladuj na prosbe uzytkownika
                MessageBoxResult dialogResult = MessageBox.Show("Do you want to load previous file?", "Previous session", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    LoadRecords();  //Prewencyjnie sproboj wczytac dane po ponownym uruchomieniu programu z pliku.
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    //Jezeli go uzytkownik nie chce to skasuj
                    File.Delete(pathfile + "\\" + XMLfile);
                }
            }
            InitializeComponent();
        }

        /// <summary>
        /// Funkcja losujaca losowe slowo
        /// </summary>
        /// <param name="length">Dlugosc zwroconego slowa</param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Dodawanie nowego rekordu do listy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(pathfile + "\\" + XMLfile))
            {
                //jezeli ktores pole pominiete to nie pusc dalej
                if (string.IsNullOrWhiteSpace(cmb_magazyn.Text) || string.IsNullOrWhiteSpace(txt_gatblachy.Text) || string.IsNullOrWhiteSpace(cmb_rodzblachy.Text) ||
                    string.IsNullOrWhiteSpace(txt_szer.Text) || string.IsNullOrWhiteSpace(txt_wys.Text) || string.IsNullOrWhiteSpace(txt_mkw.Text) || string.IsNullOrWhiteSpace(txt_masa.Text))
                {
                    MessageBox.Show("Fields are empty!", "ERROR");
                }
                else
                {
                    //stworz obiekt, losuj zawartosc, dodaj do listy
                    dane.nazwaMagazynu = cmb_magazyn.SelectedValue.ToString();
                    dane.nazwaBlachy = txt_gatblachy.Text;

                    if (rb_arkusz.IsChecked == true)
                        dane.typBlachy = rb_arkusz.Content.ToString();
                    if (rb_krag.IsChecked == true)
                        dane.typBlachy = rb_krag.Content.ToString();

                    dane.rodzajBlachy = cmb_rodzblachy.SelectedValue.ToString();
                    dane.widthBlachy = double.Parse(txt_szer.Text.ToString().Replace(".", ","));
                    dane.heightBlachy = double.Parse(txt_wys.Text.ToString().Replace(".", ","));
                    dane.mkw = int.Parse(txt_mkw.Text.ToString());
                    dane.masaton = double.Parse(txt_masa.Text.ToString().Replace(".", ","));
                    dane.data = DateTime.Now.ToString("dd.MM.yyyy");
                    dane.kodKreskowy = RandomString(7);

                    Records.Add(dane);
                    //Licznik rekordow na etykiecie
                    statusLabel.Content = "Added data no. " + Records.Count.ToString();
                    //Serializuj liste do plik
                    SaveRecords();

                    //wyczysc pola
                    cmb_magazyn.Text = txt_gatblachy.Text = cmb_rodzblachy.Text = txt_szer.Text = txt_wys.Text = txt_mkw.Text = txt_masa.Text = null;
                }
            }
            else
            {
                statusLabel.Content = "File don't exist!";
                MessageBoxResult dialogResult = MessageBox.Show("File dont exist! Do you want to create new file?", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    //wywolaj tworzenie pliku
                    createXMLfile_Click(sender, e);
                    //dodaj rekord i plik
                    btn_add_Click(sender, e);
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    statusLabel.Content = "File don't exist!";
                }
            }
        }

        /// <summary>
        /// Dodaj kolejna etykiete z wartosciami losowymi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_addrandom_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(pathfile + "\\" + XMLfile))
            {
                //RANDOM EVERYTHING!!!!111
                dane.nazwaMagazynu = RandomString(3);
                dane.nazwaBlachy = RandomString(7);
                switch (random.Next(0, 2))
                {
                    case 0:
                        dane.typBlachy = "COIL";
                        break;
                    case 1:
                        dane.typBlachy = "PLATE";
                        break;
                };
                dane.rodzajBlachy = RandomString(6);
                dane.widthBlachy = random.Next(1, 3000);
                dane.heightBlachy = random.Next(1, 100);
                dane.mkw = random.Next(0, 9);
                dane.masaton = random.Next(1, 1000);
                dane.data = DateTime.Now.ToString("dd.MM.yyyy");
                dane.kodKreskowy = RandomString(7);

                Records.Add(dane);
                statusLabel.Content = "Added Random data no. " + Records.Count.ToString();
                SaveRecords();
            }
            else
                statusLabel.Content = "File dont exist!";

        }

        /// <summary>
        /// Wczytaj zawartość z pliku do listy rekordow
        /// </summary>
        public static void LoadRecords()
        {
            Records = null;
            try
            {
                //zawartosc spod sciezki zczytaj odpowiednio rzutujac do listu
                Reader = new StreamReader(pathfile + "\\" + XMLfile);
                Records = (RecordList)RecordSerializer.Deserialize(Reader);
                Reader.Close(); // zamknij przekaz
            }
            catch (Exception)
            {
                Records = new RecordList();
                ErrorReading(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Zserializuj zawartosc listy do pliku
        /// </summary>
        public static void SaveRecords()
        {
            if (Records != null)
            {
                try
                {
                    Writer = new XmlTextWriter(pathfile + "\\" + XMLfile, null);
                    Writer.Formatting = Formatting.Indented;
                    RecordSerializer.Serialize(Writer, Records);
                    Writer.Close();
                }
                catch (Exception)
                {
                    ErrorWriting(null, EventArgs.Empty);
                    Writer.Close();
                }
            }
        }

        /// <summary>
        /// Stworzenie pliku XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createXMLfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(pathfile))
                {
                    Directory.CreateDirectory(pathfile);
                    statusLabel.Content = "Directory created";
                }
                else
                    statusLabel.Content = "Directory exist";
                if (!File.Exists(pathfile + "\\" + XMLfile))
                {
                    File.Create(pathfile + "\\" + XMLfile).Close();
                    statusLabel.Content = "File created";
                    Thread.Sleep(200);
                }
                else
                    statusLabel.Content = "File exist";
            }
            catch (Exception)
            {
                statusLabel.Content = "File or directory ERROR!";
            }
        }

        /// <summary>
        /// Zamknięcie appki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Usuwa plik XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteXMLfile_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(pathfile + "\\" + XMLfile))
            {
                File.Delete(pathfile + "\\" + XMLfile);
                Records.Clear();
                statusLabel.Content = "File deleted";
            }
        }

        /// <summary>
        /// Pokazuje okno About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mbAbout_Click(object sender, RoutedEventArgs e)
        {
            //prosty messagebox
            MessageBox.Show("Author: Paul Kastel\nSeptember 2016", "About");
        }

        /// <summary>
        /// Wola funkcje tworzaca backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createbackup_Click(object sender, RoutedEventArgs e)
        {
            CreateBackup(pathfile, XMLfile);
        }

        /// <summary>
        /// Tworzy backUp pliku XML przy okeslonych warunkach
        /// </summary>
        /// <param name="pathfile">sciezka pliku</param>
        /// <param name="filename">nazwa pliku</param>
        private void CreateBackup(string pathfile, string filename)
        {
            if (File.Exists(pathfile + "\\" + filename))
            {
                //pobierz rozmiar
                FileInfo sizefile = new FileInfo(pathfile + "\\" + filename);
                //jezeli wiekszy niz 5KB
                if (sizefile.Length > 5000)
                {
                    //zaczytaj z pliku do listy
                    LoadRecords();
                    //stworz backup z formata daty, kopiujac plik, wyczysc wszystko
                    File.Copy(pathfile + "\\" + filename, pathfile + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss_") + filename);
                    Records.Clear();
                    SaveRecords();
                    statusLabel.Content = "BackUp created";
                }
                else
                    statusLabel.Content = "Size must be larger than 5 KB";
            }
            else
                statusLabel.Content = "File dont exist!";
        }

        /// <summary>
        /// Po wybraniu odpowiedniej drukarki laczy sie z nia i wysyla wszystkie pliki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_printZPL_Click(object sender, RoutedEventArgs e)
        {
            //if printer is not selected
            if (string.IsNullOrWhiteSpace(cmb_printer.Text))
            {
                statusLabel.Content = "You need to select printer from list";
            }
            else //if is selected
            {
                //DEFAULT DATA
                string adresFTP = "ftp://10.100.101.100/command_print.zpl";
                string login = "";
                string pass = "";
                string ZPLscript = @"^XA^CF0,80^FO10, 10^FD DEFAULT TEXT ^FS^XZ";

                ////wymiary etykiety 5x21cm 8dpmm 200dpi Label_01
                //path to file with debug
                string labelpath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Labels\");

                //get name of printer
                switch (cmb_printer.SelectedValue.ToString())
                {
                    //jeden case to jedna drukarka, dzieki temu latwo rozbudowac baze drukarek. przykladowe adresy IP oraz dane
                    case "office":
                        adresFTP = "ftp://10.100.101.101/command_print.zpl";
                        login = "biuro";
                        pass = "123456";
                        if (File.Exists(labelpath + "Label_01.zpl"))
                        {
                            //jezeli folder z etykietami jest i nie jest pusty XML
                            if (Records.Count != 0)
                            {
                                foreach (Record dane in Records)
                                {
                                    //kazde dane w locie wprowadz do etykiety i ja wyslij.
                                    ZPLscript = File.ReadAllText(labelpath + "Label_01.zpl");
                                    ZPLscript = ZPLscript.Replace("!_nazwamagazyn_!", dane.nazwaMagazynu).Replace("!_gatunekblachy_!", dane.nazwaBlachy).Replace("!_typblachy_!", dane.typBlachy).Replace("!_rodzajblachy_!", dane.rodzajBlachy).Replace("!_szerokoscblachy_!", dane.widthBlachy.ToString()).Replace("!_wysokoscblachy_!", dane.heightBlachy.ToString()).Replace("!_metrkwadrat_!", dane.mkw.ToString()).Replace("!_masa_!", dane.masaton.ToString()).Replace("!_datatime_!", dane.data).Replace("!_kodkreskowy_!", dane.kodKreskowy);
                                    sendAndPrint(login, pass, adresFTP, ZPLscript);
                                }
                            }
                            else statusLabel.Content = "Record file is empty or doesn't exist!";
                        }
                        else statusLabel.Content = "There is no label!";
                        break;
                    case "warehouse":
                        adresFTP = "ftp://10.100.101.102/command_print.zpl";
                        login = "magazyn";
                        pass = "123456";
                        if (File.Exists(labelpath + "Label_01.zpl"))
                        {
                            if (Records.Count != 0)
                            {
                                foreach (Record dane in Records)
                                {
                                    ZPLscript = File.ReadAllText(labelpath + "Label_01.zpl");
                                    ZPLscript = ZPLscript.Replace("!_nazwamagazyn_!", dane.nazwaMagazynu).Replace("!_gatunekblachy_!", dane.nazwaBlachy).Replace("!_typblachy_!", dane.typBlachy).Replace("!_rodzajblachy_!", dane.rodzajBlachy).Replace("!_szerokoscblachy_!", dane.widthBlachy.ToString()).Replace("!_wysokoscblachy_!", dane.heightBlachy.ToString()).Replace("!_metrkwadrat_!", dane.mkw.ToString()).Replace("!_masa_!", dane.masaton.ToString()).Replace("!_datatime_!", dane.data).Replace("!_kodkreskowy_!", dane.kodKreskowy);
                                    sendAndPrint(login, pass, adresFTP, ZPLscript);
                                }
                            }
                            else statusLabel.Content = "Record file is empty or doesn't exist!";
                        }
                        else statusLabel.Content = "There is no label!";
                        break;
                    case "production":
                        adresFTP = "ftp://10.100.101.103/command_print.zpl";
                        login = "produkcja";
                        pass = "123456";
                        if (File.Exists(labelpath + "Label_01.zpl"))
                        {
                            if (Records.Count != 0)
                            {
                                foreach (Record dane in Records)
                                {
                                    ZPLscript = File.ReadAllText(labelpath + "Label_01.zpl");
                                    ZPLscript = ZPLscript.Replace("!_nazwamagazyn_!", dane.nazwaMagazynu).Replace("!_gatunekblachy_!", dane.nazwaBlachy).Replace("!_typblachy_!", dane.typBlachy).Replace("!_rodzajblachy_!", dane.rodzajBlachy).Replace("!_szerokoscblachy_!", dane.widthBlachy.ToString()).Replace("!_wysokoscblachy_!", dane.heightBlachy.ToString()).Replace("!_metrkwadrat_!", dane.mkw.ToString()).Replace("!_masa_!", dane.masaton.ToString()).Replace("!_datatime_!", dane.data).Replace("!_kodkreskowy_!", dane.kodKreskowy);
                                    sendAndPrint(login, pass, adresFTP, ZPLscript);
                                }
                            }
                            else statusLabel.Content = "Record file is empty or doesn't exist!";
                        }
                        else statusLabel.Content = "There is no label!";
                        break;
                    default:
                        //jak ktos cos wybral zle to nie drukuj
                        statusLabel.Content = "Problem z wyborem drukarki!";
                        break;
                }
            }
        }

        /// <summary>
        /// Odpowiada za polaczenie sie z odpowiednia drukarka Zebra i przeslania polecenia druku etykiety
        /// </summary>
        /// <param name="login">Login admina na zebrze</param>
        /// <param name="pass">haslo do drukarki</param>
        /// <param name="adresFTP">adres zebry</param>
        /// <param name="ZPLscript">polecenie druku etykiety</param>
        private void sendAndPrint(string login, string pass, string adresFTP, string ZPLscript)
        {
            //spróbuj stworzyc polaczenie via FTP
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //utworz plik i zapisz w nim dane
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "command_print.zpl").Close();
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "command_print.zpl", ZPLscript);

                //polacz FTP
                FtpWebRequest ftpReq = (FtpWebRequest)WebRequest.Create(adresFTP);
                ftpReq.UseBinary = true;
                ftpReq.Method = WebRequestMethods.Ftp.UploadFile;
                //login 
                ftpReq.Credentials = new NetworkCredential(login, pass);

                byte[] b = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "command_print.zpl");
                ftpReq.ContentLength = b.Length;
                using (Stream s = ftpReq.GetRequestStream())
                {
                    s.Write(b, 0, b.Length);
                }

                FtpWebResponse ftpResp = (FtpWebResponse)ftpReq.GetResponse();
                if (ftpResp != null)
                {
                    if (ftpResp.StatusDescription.StartsWith("226"))
                    {
                        statusLabel.Content = "SUCCESS! PRINTED!";
                    }
                }

                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "command_print.zpl");
            }
            catch (WebException)
            {
                statusLabel.Content = "Cannot connect with printer via FTP.";
            }
            catch (IOException)
            {
                statusLabel.Content = "Can't create file to send.";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        //-----------------------Anty wpisywanie bzdur----------------------
        //===============do konca nie dziala ale daje rade==================
       
        /// <summary>
        /// Sprawdza czy wprowadza sie liczbe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void preventWrongInput(object sender, TextCompositionEventArgs e)
        {
            CheckIsNumeric((TextBox)sender, e);
        }
       
        /// <summary>
        /// Mozna wprowadzic tylko liczby i jedna kropke
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckIsNumeric(TextBox sender, TextCompositionEventArgs e)
        {
            decimal result;
            bool dot = sender.Text.IndexOf(".") < 0 && e.Text.Equals(".") && sender.Text.Length > 0;
            if (!(Decimal.TryParse(e.Text, out result) || dot))
            {
                e.Handled = true;
            }
        }
        
        /// <summary>
        /// Zapobiega wklejaniu glupot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Sprawdza czy wklejona zawartosc to liczba
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }
        //===================================================================
    }
}

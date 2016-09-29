using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public static string ZPLfile = "ZPLScript.zpl";
        public static string pathfile = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "ZebraPrinterXML";

        public static RecordList Records;
        private static XmlSerializer RecordSerializer;
        public static XmlTextWriter Writer;
        private static StreamReader Reader;

        private static Random random = new Random();

        public MainWindow()
        {
            Records = new RecordList();
            RecordSerializer = new XmlSerializer(typeof(RecordList));
            if (File.Exists(pathfile + "\\" + XMLfile))
            {
                
                MessageBoxResult dialogResult = MessageBox.Show("Do you want to load previous file?", "Previous session", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    LoadRecords();  //Prewencyjnie sproboj wczytac dane po ponownym uruchomieniu programu z pliku.
                }
                else if (dialogResult == MessageBoxResult.No)
                {
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
                    Record r = new Record();
                    r.nazwaMagazynu = cmb_magazyn.SelectedItem.ToString();
                    r.nazwaBlachy = txt_gatblachy.Text;

                    if (rb_arkusz.IsChecked == true)
                        r.typBlachy = rb_arkusz.Content.ToString();
                    if (rb_krag.IsChecked == true)
                        r.typBlachy = rb_krag.Content.ToString();

                    r.rodzajBlachy = cmb_rodzblachy.SelectedItem.ToString();
                    r.widthBlachy = double.Parse(txt_szer.Text.ToString().Replace(".", ","));
                    r.heightBlachy = double.Parse(txt_wys.Text.ToString().Replace(".", ","));
                    r.mkw = int.Parse(txt_mkw.Text.ToString());
                    r.masaton = double.Parse(txt_masa.Text.ToString().Replace(".", ","));
                    r.data = DateTime.Now.ToString("dd.MM.yyyy");
                    r.kodKreskowy = RandomString(7);

                    Records.Add(r);
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
                    createXMLFileToolStripMenuItem_Click(sender, e);
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
                Record r = new Record();
                r.nazwaMagazynu = RandomString(3);
                r.nazwaBlachy = RandomString(7);
                switch (random.Next(0, 2))
                {
                    case 0:
                        r.typBlachy = "KRAG";
                        break;
                    case 1:
                        r.typBlachy = "ARKUSZ";
                        break;
                };
                r.rodzajBlachy = RandomString(6);
                r.widthBlachy = random.Next(1, 3000);
                r.heightBlachy = random.Next(1, 100);
                r.mkw = random.Next(0, 9);
                r.masaton = random.Next(1, 1000);
                r.data = DateTime.Now.ToString("dd.MM.yyyy");
                r.kodKreskowy = RandomString(7);

                Records.Add(r);
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
        private void createXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void CreateBackup(string pathfile, string filename)
        {
            if (File.Exists(pathfile + "\\" + filename))
            {
                //pobierz rozmiar
                FileInfo sizefile = new FileInfo(pathfile + "\\" + filename);
                //jezeli wiekszy niz 5KB lub jest wiecej niz 10 wpisow
                if (sizefile.Length > 5000)
                {
                    //zaczytaj z pliku do listy
                    LoadRecords();
                    //stworz backup z formata daty, kopiujac plik
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
        private void createXMLfile_Click(object sender, RoutedEventArgs e)
        {

        }


        private void closeApp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteXMLfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mbAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Author: Paul Kastel\nSeptember 2016", "About");
        }

        private void createbackup_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

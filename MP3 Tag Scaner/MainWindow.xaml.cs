using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;


namespace MP3_Tag_Scaner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private DirectoryInfo _workDir;
        private List<FileInfo> _fileList;

        private void ChangeNamesToTranslit(FileInfo file)
        {
            var newName = FormNewEnglishName(file.Name);
            //  MessageBox.Show(WorkDir.Name + "|" + file.Name+"|"+file.FullName);
            file.MoveTo(file.FullName.Replace(file.Name, newName));
        }

        private string FormNewEnglishName(string oldName)
        {
            oldName = oldName.Replace("_", " ").Replace("ч", "ch").Replace("Ч", "Ch").Replace("ш", "sh").Replace("Ш", "Sh")
                .Replace("щ", "sh").Replace("Щ", "Sh").Replace("ъ", "").Replace("ь", "").Replace("ы", "i")
                .Replace("э", "e").Replace("Э", "E").Replace("ю", "yu").Replace("Ю", "Yu").Replace("я", "ya").Replace("Я", "Ya");

            const string russian = @"АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦц"; //ЧчШшЩщЪъЫыЬьЭэЮюЯя
            const string english = @"AaBbVvGgDdEeEeJjZzIiIiKkLlMmNnOoPpRrSsTtUuFfHhCc"; //ChchShshSchschYyEeYuyuYaya

            foreach (var letter in oldName)
            {
                for (var i = 0; i < russian.Length; i++)
                {
                    if (letter == russian[i])
                    {
                        oldName = oldName.Replace(letter, english[i]);
                    }
                }
            }
            return oldName;
        }

        class Mp3Info
        {
            private string _artist = null;
            private string _title = null;

            public Mp3Info(Mp3Info template)
            {
                _artist = template.Artist;
                _title = template.Title;
            }

            public Mp3Info(string artist, string title)
            {
                _artist = artist;
                _title = title;
            }

            public Mp3Info()
            {
                _artist = null;
                _title = null;
            }

            public string ShowInfo()
            {
                return this.Artist + " - " + this.Title;
            }

            public string Artist
            {
                private get { return _artist; }
                set { _artist = !string.IsNullOrEmpty(value) ? value : "Unknown artist"; }
            }

            public string Title
            {
                private get { return _title; }
                set
                {
                    _title = !string.IsNullOrEmpty(value) ? value : "Untitled";
                }
            }
        }

        private Mp3Info GetNamesFromTags(FileInfo file)
        {
            var updFile = TagLib.File.Create(file.FullName);
            string artists = null;
            string title = null;
            var srcEncodingFormat = Encoding.GetEncoding("windows-1252");

            if (updFile.Tag.Performers != null)
            {
                byte[] originalByteString = srcEncodingFormat.GetBytes(String.Join(", ", updFile.Tag.Performers));
                artists = Encoding.Default.GetString(originalByteString);
            }
            else
            {
                artists = "Unknown artist";
            }

            if (updFile.Tag.Title != null)
            {

                byte[] originalByteString = srcEncodingFormat.GetBytes(updFile.Tag.Title);
                title = Encoding.Default.GetString(originalByteString);
            }
            else
            {
                title = "Untitled";
            }

            var collectedInfo = new Mp3Info { Artist = artists, Title = title };
            //     MessageBox.Show(collectedInfo.ShowInfo());
            return collectedInfo;
        }

        private void SetTagsFromName(FileInfo file)
        {
            var mp3Info = new Mp3Info(GetNamesFromTags(file));
          //  MessageBox.Show(mp3Info.ShowInfo());

           

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog { Description = Properties.Resources.MainWindow_OpenFolder_Description };
            if (openFolderDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            _workDir = new DirectoryInfo(openFolderDialog.SelectedPath);
            _fileList = new List<FileInfo>();
            foreach (FileInfo file in _workDir.GetFiles("*.mp3"))
            {
                LvSongList.Items.Add(file);
                _fileList.Add(file);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            foreach (var file in _fileList)
            {
                SetTagsFromName(file);


                if (CheckToGetNameFromTag != null && CheckToGetNameFromTag.IsChecked.Value)
                {
              //      GetNamesFromTags(file);
                }
                if (CheckToTranslit.IsChecked != null && CheckToTranslit.IsChecked.Value)
                {
                    ChangeNamesToTranslit(file);
                }
            }
        }
    }
}

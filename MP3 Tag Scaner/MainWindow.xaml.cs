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
using MP3_Tag_Scaner.Classes;


namespace MP3_Tag_Scaner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryInfo _workDir;
        private List<FileInfo> _fileList;

        public MainWindow()
        {
            InitializeComponent();
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

            var mp3info = new Mp3Info();
            var mp3Moder = new Mp3Modifications();
            foreach (var file in _fileList)
            {
                mp3info.SetTagsFromName(file);


                if (CheckToGetNameFromTag.IsChecked.Value)
                {
                    try
                    {
                       mp3Moder.GetNamesFromTags(file);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message.ToString(), "There was a error");
                    }
                }
                if (CheckToTranslit.IsChecked.Value)
                {
                    try
                    {
                        mp3Moder.ChangeNamesToTranslit(file);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message.ToString(), "There was a error");
                    }
                }
            }
        }
    }
}

using Microsoft.WindowsAPICodePack.Dialogs;
using RenameFile.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
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

namespace RenameFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int sofile;

        public bool NeedRefresh { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AllVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            SelectCulture("vi-VN");
            SetLanguageDictionary();
        }
        public static void SelectCulture(string culture)
        {
            if (String.IsNullOrEmpty(culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            //Search for the specified culture.     
            string requestedCulture = string.Format("StringResources.{0}.xaml", culture);
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = "StringResources.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("StringResources.xaml",
                                  UriKind.Relative);
                    break;
                case "vi-VN":
                    dict.Source = new Uri("StringResources.vi-VN.xaml",
                                       UriKind.Relative);              
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Bạn hãy chọn folder mà bạn muốn rename!";
            // fl.SelectedPath = Environment.SpecialFolder.Recent;
            if (Directory.Exists(Properties.Settings.Default.defaultChosePath)) // nếu có đường dẫn rồi thì lấy đường dẫn đó
            {
                dialog.DefaultDirectory = Properties.Settings.Default.defaultChosePath;
                //Directory a= new Directory(Directory);

                // fl.RootFolder =Environment.SpecialFolder.MyPictures;
            }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //lam gi thi lam
                //ghi duong dan ra file
                //MessageBox.Show( Path.GetFullPath(fl.SelectedPath));
                txtInput.Text = dialog.FileName;               
                NeedRefresh = false;
            }
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.defaultChosePath = txtInput.Text;
            Properties.Settings.Default.Save();
            RrefreshFilename(txtInput.Text);
        }
        private void RrefreshFilename(string inputText)
        {
            sofile = 0;
            listviewFile.ItemsSource=null;
            //listviewFile.Sorting = SortOrder.None; todo


           
                GetFileList(inputText);

           

        }
        private void GetFileList(string dauvao)// lay danh sach file
        {
            try
            {

                string[] filePaths = Directory.GetFiles(dauvao);

                // image list

                // imageList1.ColorDepth = ColorDepth.Depth32Bit; //todo
                //imageList1.ImageSize = new Size(16, 16);//todo
                //listviewFile.Invoke(new MethodInvoker(() =>
                //{
                //    listviewFile.SmallImageList = imageList1;
                //}));

                var items = new List<MyFileInfo>();
                // tongSofile += filePaths.Length;
                foreach (string filename in filePaths)
                {




                    if (chkKeepFile.IsChecked == false)
                    {


                        // Set a default icon for the file.
                        // System.Drawing.Icon iconForFile = SystemIcons.WinLogo;
                        FileInfo info = new FileInfo(filename);
                        var myFileInfo = new MyFileInfo() {
                            OldName = System.IO.Path.GetFileName(filename),
                            NewName = "",
                            Path = filename,
                            LastWriteTime = info.LastWriteTime.ToString("yyyy-MM-dd"),
                        };



                        //listviewFile.Invoke(new MethodInvoker(() =>
                        //{
                        //    listviewFile.Items.Add(item);
                        //    sofile += 1;
                        //    lbFileNumber.Text = String.Format("Đã load {0} file", sofile);
                        //}));
                        items.Add(myFileInfo);
                    }
                }

                string[] _thuMuccon = Directory.GetDirectories(dauvao);
                foreach (string thumucconcon in _thuMuccon)
                {
                    if (chkKeepFolder.IsChecked == false)
                    {
                        FileInfo info = new FileInfo(thumucconcon);
                        var myFileInfo = new MyFileInfo()
                        {
                            OldName = System.IO.Path.GetFileName(thumucconcon),
                            NewName = "",
                            Path=thumucconcon,
                            LastWriteTime = info.LastWriteTime.ToString("yyyy-MM-dd"),

                        };
                        items.Add(myFileInfo);
                    }
                    if (chkIncludeSubfolder.IsChecked == true)
                        GetFileList(thumucconcon);
                }
                listviewFile.ItemsSource = items;
                sofile+= items.Count;
                lbFileNumber.Content = String.Format("Đã load {0} file", sofile);               

            }
            catch
            {
                // ignored
            }
            //return tongSofile;
        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
                     
        }
    }
}

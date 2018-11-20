using System;
using System.Collections.Generic;
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

namespace FileVersion.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileHashProvider p1 = new FileHashProvider(this.Check1.IsChecked == true, this.TbQueryTmp.Text);
            var list = p1.GetDirInfo("./");
            UpdateDirInfo(list);
        }



        public void UpdateDirInfo(List<FileHashInfo> list)
        {
            DataGrid1.ItemsSource = list;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.BtnGet.IsEnabled = false;
            var isQuery= this.Check1.IsChecked == true;
            var queryTmp = this.TbQueryTmp.Text;
            var path = this.TbFile.Text;
            var task = Task.Run(() =>
            {
                FileHashProvider p1 = new FileHashProvider(isQuery,queryTmp );
                var list = p1.GetDirInfo(path);
                var item = p1.GetFileHashInfo(path);
                if (item != null)
                {
                    list.Insert(0, item);
                }
                this.Dispatcher.InvokeAsync(()=>UpdateDirInfo(list));
            });
            task.ContinueWith((t) =>
            {
                Dispatcher.Invoke(() => this.BtnGet.IsEnabled = true);
                return t;
            });
        }

        private void DockPanel_Drop(object sender, DragEventArgs e)
        {
            string[] files=null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
            }

            this.BtnGet.IsEnabled = false;
            var isQuery = this.Check1.IsChecked == true;
            var queryTmp = this.TbQueryTmp.Text;
            var task = Task.Run(() =>
            {
                FileHashProvider p1 = new FileHashProvider(isQuery, queryTmp);
                var list = new List<FileHashInfo>();
                foreach (var path in files)
                {
                    var item = p1.GetFileHashInfo(path);
                    list.Add(item);
                }
                
                this.Dispatcher.InvokeAsync(() => UpdateDirInfo(list));
            });
            task.ContinueWith((t) =>
            {
                Dispatcher.Invoke(() => this.BtnGet.IsEnabled = true);
                return t;
            });
        }
    }
}

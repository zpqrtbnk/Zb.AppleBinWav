using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using NAudio.Wave;

namespace ZbAppleBinWav
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //_data = Encoding.UTF8.GetBytes("Hello, world! How are we doing today? Feeling good?      ZZZZ");
            //var length = _data.Length;
            //BytesLength.Content = string.Format("0x{0:X4} bytes ie 0000.{1:X4}R", length, length - 1);

            ReloadButton.Visibility = Visibility.Hidden;
            PlayButton.IsEnabled = false;
            _savedBrush = FileNameBorder.BorderBrush;
        }

        private WaveOut _waveOut;
        private byte[] _data;
        private string _filename;
        private FileSystemWatcher _fileWatcher;
        private bool _dirty;
        private readonly Brush _savedBrush;
        private readonly Brush _dirtyBrush = new SolidColorBrush(Color.FromRgb(198, 0, 0));

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null)
                return;

            if (_waveOut != null)
            {
                PlayButton.Content = "play";
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
                return;
            }

            PlayButton.Content = "stop";

            var waveProvider = new DataWaveProvider(_data);
            _waveOut = new WaveOut();
            _waveOut.Init(waveProvider);
            _waveOut.Volume = 0.8f;

            // only for automatic stop (end of wav) not manual stop
            _waveOut.PlaybackStopped += (o, args) =>
            {
                PlayButton.Content = "play";
                _waveOut.Dispose();
                _waveOut = null;
            };

            _waveOut.Play();
        }

        private void Source_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            //dialog.FileName = "Document"; // Default file name
            //dialog.DefaultExt = ".txt"; // Default file extension
            //dialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 
            
            var result = dialog.ShowDialog();

            if (result == true)
            {
                _filename = dialog.FileName;
                LoadFile();

                if (_fileWatcher != null)
                    _fileWatcher.Dispose();

                _fileWatcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(_filename), System.IO.Path.GetFileName(_filename));
                _fileWatcher.Changed += (o, args) =>
                {
                    if (_dirty) return;
                    _dirty = true;
                    Display.Dispatcher.Invoke(() =>
                    {
                        ReloadButton.Visibility = Visibility.Visible;
                        FileNameBorder.BorderBrush = _dirtyBrush;
                    });
                };
                _fileWatcher.EnableRaisingEvents = true;
            }
        }

        void LoadFile()
        {
            //_data = File.ReadAllBytes(filename);

            _dirty = false;
            ReloadButton.Visibility = Visibility.Hidden;
            FileNameBorder.BorderBrush = _savedBrush;
            FileNameLabel.Content = System.IO.Path.GetFileName(_filename);

            // todo - detect file errors & report properly
            var block = new IntelHex().ReadAllBlocks(_filename).FirstOrDefault();
            if (block == null)
            {
                _data = null;
                Display.Content = "No data?";
                PlayButton.IsEnabled = false;
                return;
            }

            _data = block.Data;

            var length = _data.Length;
            //BytesLength.Content = string.Format("0x{0:X4} bytes\r\n\r\n0000.{1:X4}R", length, length - 1);
            Display.Content = string.Format("0x{0:X4} bytes at 0x{1:X4}\r\n{1:X4}.{2:X4}R",
                length, block.Address, block.Address + length - 1);

            PlayButton.IsEnabled = true;
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_waveOut == null && _dirty && !string.IsNullOrWhiteSpace(_filename))
            {
                LoadFile();
            }
        }

        private void Move_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}

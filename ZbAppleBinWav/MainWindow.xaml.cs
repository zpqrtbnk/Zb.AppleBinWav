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
        }

        private WaveOut _waveOut;
        private byte[] _data;

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_data == null)
                return;

            if (_waveOut != null)
            {
                PlayButton.Content = "Play";
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
                return;
            }

            PlayButton.Content = "Stop";

            var waveProvider = new DataWaveProvider(_data);
            _waveOut = new WaveOut();
            _waveOut.Init(waveProvider);
            _waveOut.Volume = 0.8f;

            // only for automatic stop (end of wav) not manual stop
            _waveOut.PlaybackStopped += (o, args) =>
            {
                PlayButton.Content = "Play";
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

            // todo - load various binary formats from disk

            if (result == true)
            {
                var filename = dialog.FileName;

                //_data = File.ReadAllBytes(filename);

                var block = new IntelHex().ReadAllBlocks(filename).FirstOrDefault();
                if (block == null)
                {
                    _data = null;
                    BytesLength.Content = "No data.";
                    return;
                }

                _data = block.Data;

                var length = _data.Length;
                //BytesLength.Content = string.Format("0x{0:X4} bytes\r\n\r\n0000.{1:X4}R", length, length - 1);
                BytesLength.Content = string.Format("{0}\r\n0x{1:X4} bytes at 0x{2:X4}\r\n{2:X4}.{3:X4}R",
                    System.IO.Path.GetFileName(filename), length, block.Address, block.Address + length - 1);
            }
        }
    }
}

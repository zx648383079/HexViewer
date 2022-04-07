using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.HexViewer.Models;
using ZoDream.HexViewer.Storage;

namespace ZoDream.HexViewer.ViewModels
{
    public class PropertyViewModel: BindableBase
    {

        private string name = string.Empty;

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        private string fileName = string.Empty;

        public string FileName
        {
            get => fileName;
            set => Set(ref fileName, value);
        }

        private long length;

        public long Length
        {
            get => length;
            set => Set(ref length, value);
        }

        private DateTime createdAt;

        public DateTime CreatedAt
        {
            get => createdAt;
            set => Set(ref createdAt, value);
        }


        private DateTime updatedAt;

        public DateTime UpdatedAt
        {
            get => updatedAt;
            set => Set(ref updatedAt, value);
        }

        private ObservableCollection<FileHeaderItem> typeItems = new();

        public ObservableCollection<FileHeaderItem> TypeItems
        {
            get => typeItems;
            set => Set(ref typeItems, value);
        }

        private string typeMessage = string.Empty;

        public string TypeMessage
        {
            get => typeMessage;
            set => Set(ref typeMessage, value);
        }



        public PropertyViewModel()
        {
            if (string.IsNullOrWhiteSpace(App.ViewModel.FileName) || App.ViewModel.Reader == null)
            {
                return;
            }
            FileName = App.ViewModel.FileName;
            var fileInfo = new FileInfo(FileName); 
            Name = fileInfo.Name;
            Length = App.ViewModel.Reader.Length;
            CreatedAt = fileInfo.CreationTime;
            UpdatedAt = fileInfo.LastWriteTime;
            _ = LoadSignatureAsync(App.ViewModel.Reader);
        }

        public async Task LoadSignatureAsync(IByteStream stream)
        {
            if (stream == null)
            {
                return;
            }
            var signFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSignature.csv");
            if (!File.Exists(signFile))
            {
                TypeMessage = $"Signature file error [{signFile}]";
                return;
            }
            TypeMessage = "Loading ...";
            using var sr = Utils.Storage.File.Reader(signFile);
            while (true)
            {
                var line = await sr.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var args = line.Split(new char[] { ',' }, 4);
                if (args.Length < 2)
                {
                    continue;
                }
                try
                {
                    var headerFormat = new FileHeaderItem(args[0].Trim(), args[1].Trim())
                    {
                        FooterHex = args.Length > 2 ? args[2].Trim() : string.Empty,
                        Description = args.Length > 3 ? args[3].Trim() : string.Empty,
                    };
                    if (await headerFormat.MatchAsync(stream))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            TypeItems.Add(headerFormat);
                        });
                    }
                }
                catch (Exception)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TypeMessage = $"Signature hex error [{line}]";
                    });
                    return;
                }
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                TypeMessage = $"Found {TypeItems.Count}";
            });
        }

    }
}

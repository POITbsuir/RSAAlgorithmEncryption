using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThirdTaskApplication.Models;
using ThirdTaskApplication.Service;

namespace ThirdTaskApplication.ViewModels
{
    public class ApplicationMain : INotifyPropertyChanged
    {
        private AlgorithmRSA _currentRSA;
        public AlgorithmRSA CurrentRSA
        {
            get => _currentRSA;
            set
            {
                _currentRSA = value;
                OnPropertyChanged("CurrentRSA");
            }
        }

        public ApplicationMain()
        {
            string binaryPath = @"C:\Users\Константин\Desktop\TheoryInformation\TaskThird\ThirdTaskWPF\ThirdTaskApplication\Data\bin_result_file.bin";
            string inputPath = @"C:\Users\Константин\Desktop\TheoryInformation\TaskThird\ThirdTaskWPF\ThirdTaskApplication\Data\input_file.txt";
            string inputPathPicture = @"C:\Users\Константин\Desktop\TheoryInformation\TaskThird\ThirdTaskWPF\ThirdTaskApplication\Data\TextPictureFile.png";
            string outputPathPicture = @"C:\Users\Константин\Desktop\TheoryInformation\TaskThird\ThirdTaskWPF\ThirdTaskApplication\Data\picture.encrypted";
            CurrentRSA = new AlgorithmRSA(inputPath, binaryPath);
            //CurrentRSA = new AlgorithmRSA(inputPathPicture, outputPathPicture);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        //блок кода с командами
        private AppCommands _selectInputFileCommand;
        public AppCommands SelectInputFileCommand
        {
            get
            {
                return _selectInputFileCommand ?? (_selectInputFileCommand = new AppCommands(obj =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        _currentRSA.SetInputFile(openFileDialog.FileName);
                        _currentRSA.LoadMessageFromFile();
                    }
                }));
            }
        }

        private AppCommands _selectOutputFileCommand;
        public AppCommands SelectOutputFileCommand
        {
            get
            {
                return _selectOutputFileCommand ?? (_selectOutputFileCommand = new AppCommands(obj =>
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Бинарные файлы (*.bin)|*.bin|Все файлы (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        _currentRSA.SetOutputFile(saveFileDialog.FileName);
                    }
                }));
            }
        }

        private AppCommands _encryptionCommands;
        public AppCommands EncryptionCommands
        {
            get
            {
                return _encryptionCommands ?? (_encryptionCommands = new AppCommands(obj =>
                {
                    _currentRSA.EncryptionRSA();

                }));
            }
        }

        private AppCommands _decryptedCommands;
        public AppCommands DecryptedMessage
        {
            get
            {
                return _decryptedCommands ?? (_decryptedCommands = new AppCommands(obj =>
                {
                    string decryptedMessage = _currentRSA.DecryptionRSA();

                }));
            }
        }

        private AppCommands _encryptionFromFile;
        //зашифровка зашифрованного файла
        public AppCommands EncryptionFromFile
        {
            get
            {
                return _encryptionFromFile ?? (_encryptionFromFile = new AppCommands(obj =>
                {
                    _currentRSA.EncryptionAnyFileRSA();
                }));
            }
        }

        //расшировка зашифрованного файла
        private AppCommands _decryptionFromFile;
        public AppCommands DecryptionFromFile
        {
            get
            {
                return _decryptionFromFile ?? (_decryptionFromFile = new AppCommands(obj =>
                {
                    _currentRSA.DecryptionAnyFileRSA();
                }));
            }
        }
    }
}
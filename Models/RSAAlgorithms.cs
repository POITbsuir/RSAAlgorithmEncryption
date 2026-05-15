using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ThirdTaskApplication.Models
{
    public class AlgorithmRSA : INotifyPropertyChanged
    {
        private long _q { get; set; }
        private long _p { get; set; }
        private long _exponent { get; set; }
        private string _message { get; set; }
        private string _outputPathFile { get; set; } //перемнная для хранения пути файла-результата
        private string _inputPathFile { get; set; } //переменная для хранения пути к файлу-исходнику

        private string _latinUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string _latinLower = "abcdefghijklmnopqrstuvwxyz";
        private string _cyrillicUpper = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private string _cyrillicLower = "абвгдеёжзийклмнопрстуфхчшщъыьэюя";
        private string _digits = "0123456789";
        private string _punctuation = " .,!?;:-()\"'«»\r\n\t";

        private FileService _fileService;
        private Dictionary<long, long> _secretKey { get; set; }
        private long[] _arrayEncryptData { get; set; }
        private string _typeInputValues { get; set; }
        private long[] _resultDecryptionArray { get; set; }
        private long[] _ecnryptedDataArray { get; set; } //массив зашифрованных чисел, которые будут записаны в файл
        private long[] _encryptionDataArrayInFile { get; set; } //массив считанных зашифрованных чисел с файла
        private string _decryptedMessage { get; set; } //расшифрованное сообщение
        private Algorithms _algorithms;
        private string _inputData { get; set; } //переменная для храения входящих данных
        private string _originalInputData { get; set; } //оригинальная строка с пробелами

        public long Q { get => _q; set { _q = value; OnPropertyChanged("Q"); } }
        public long P { get => _p; set { _p = value; OnPropertyChanged("P"); } }
        public long Exponent { get => _exponent; set { _exponent = value; OnPropertyChanged("Exponent"); } }
        public string Message { get => _message; set { _message = value; OnPropertyChanged("Message"); } }

        public string InputPathFile
        {
            get => _inputPathFile;
            set
            {
                _inputPathFile = value;
                OnPropertyChanged("InputPathFile");
            }
        }

        public string OutputPathFile
        {
            get => _outputPathFile;
            set
            {
                _outputPathFile = value;
                OnPropertyChanged("OutputPathFile");
            }
        }

        private string _encryptedData;
        public string EncryptedData
        {
            get => _encryptedData;
            set
            {
                _encryptedData = value;
                OnPropertyChanged("EncryptedData");
            }
        }

        public string DecryptedMessage
        {
            get => _decryptedMessage;
            set
            {
                _decryptedMessage = value;
                OnPropertyChanged("DecryptedMessage");
            }
        }

        public AlgorithmRSA()
        {
            _algorithms = new Algorithms();
            _fileService = new FileService();
        }

        public AlgorithmRSA(string inputPath, string outputPath)
        {
            _algorithms = new Algorithms();
            _fileService = new FileService();

            _inputPathFile = inputPath;
            _outputPathFile = outputPath;
        }

        //блок событие для наследоваемого интерфейса
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void SetInputFile(string path)
        {
            _inputPathFile = path;
            OnPropertyChanged("InputPathFile");
        }

        public void SetOutputFile(string path)
        {
            _outputPathFile = path;
            OnPropertyChanged("OutputPathFile");
        }

        public void LoadMessageFromFile()
        {
            if (string.IsNullOrWhiteSpace(_inputPathFile))
            {
                MessageBox.Show("Ошибка! Выберите файл для открытия.");
                return;
            }

            if (!_fileService.IsFileExists(_inputPathFile))
            {
                MessageBox.Show("Ошибка! Выбранный файл не найден.");
                return;
            }

            string text = _fileService.ReadText(_inputPathFile);

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Ошибка! Файл пуст.");
                return;
            }

            if (!IsValidInput(text))
            {
                MessageBox.Show("Ошибка! В файле есть запрещенные символы. Разрешены буквы, цифры, пробелы, запятые и знаки препинания.");
                return;
            }

            Message = text;
        }

        //блок шифрования данных 
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void EncryptionRSA()
        {
            if (!CheckRSAValues())
                return;

            long r = _q * _p;
            long fuctionEiler = (_q - 1) * (_p - 1);
            long exponent = _exponent;

            long[] dExponentArray = _algorithms.AlgorithmEuclidex(exponent, fuctionEiler);
            long x1 = dExponentArray[0];

            // исправление для больших/разных экспонент
            long dExponent = ((x1 % fuctionEiler) + fuctionEiler) % fuctionEiler;

            Dictionary<long, long> key = new Dictionary<long, long>() { { exponent, r } }; //K_o
            _secretKey = new Dictionary<long, long>() { { dExponent, r } };

            IsCorrectInputData(); //ввод выражения

            if (string.IsNullOrEmpty(_inputData))
                return;

            _typeInputValues = "T";
            Encrypt("T", _inputData, key);
            EncryptionOnFile(_outputPathFile);
        }

        private bool CheckRSAValues()
        {
            long r = _q * _p;
            long fuctionEiler = (_q - 1) * (_p - 1);

            if (!IsPrime(_q) || !IsPrime(_p))
            {
                MessageBox.Show("Ошибка! P и Q должны быть простыми числами.");
                return false;
            }

            if (r <= 255)
            {
                MessageBox.Show("Ошибка! Для шифрования байтов нужно, чтобы P * Q было больше 255.");
                return false;
            }

            if (!(_exponent > 1 && _exponent < fuctionEiler))
            {
                MessageBox.Show("Ошибка! Экспонента должна быть больше 1 и меньше функции Эйлера.");
                return false;
            }

            if (Gcd(_exponent, fuctionEiler) != 1)
            {
                MessageBox.Show("Ошибка! Экспонента должна быть взаимно простой с функцией Эйлера.");
                return false;
            }

            return true;
        }

        private void EncryptionOnFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Ошибка! Выберите файл для сохранения результата.");
                return;
            }

            _fileService.SaveToFile(_ecnryptedDataArray, path);
            Console.WriteLine("Данные успешно сохранены в файл: ");
        }

        private void Encrypt(string type, string message, Dictionary<long, long> key)//шифрование в зависимости от типа вводимых данных
        {
            if (type == "T")
            {
                string alphabet = GetFullAlphabet();
                _arrayEncryptData = new long[message.Length];

                for (int i = 0; i < message.Length; i++)
                {
                    int index = alphabet.IndexOf(message[i]);
                    _arrayEncryptData[i] = index;
                }

                EncryptArray(key);
                return;
            }

            message = message.ToUpper(); //привожу к вернему регистру

            if (type == "L")
            {
                _arrayEncryptData = new long[message.Length];

                for (int i = 0; i < message.Length; i++)
                {
                    int index = _latinUpper.IndexOf(message[i]); //записываю числа
                    _arrayEncryptData[i] = index;//ЕСЛИ ДОЕБЕТСЯ хули 2-27, а не 0-26 убрать +2 нужго
                }

                EncryptArray(key);
            }

            if (type == "C")
            {
                _arrayEncryptData = new long[message.Length];

                for (int i = 0; i < message.Length; i++)
                {
                    int index = _cyrillicUpper.IndexOf(message[i]); //записываю числа
                    _arrayEncryptData[i] = index;//ЕСЛИ ДОЕБЕТСЯ хули 2-27, а не 0-26 убрать +2 нужго
                }

                EncryptArray(key);
            }

            if (type == "D")
            {
                _arrayEncryptData = new long[message.Length];

                for (int i = 0; i < message.Length; i++)
                {
                    int index = _digits.IndexOf(message[i]);
                    _arrayEncryptData[i] = index;
                }

                EncryptArray(key);
            }
        }

        private void EncryptArray(Dictionary<long, long> key)
        {
            _ecnryptedDataArray = new long[_arrayEncryptData.Length];

            for (int i = 0; i < _arrayEncryptData.Length; i++)
            {
                _ecnryptedDataArray[i] = EncryptionAlgorithmWithC(_arrayEncryptData[i], key);
            }

            EncryptedData = string.Join(" ", _ecnryptedDataArray);
        }

        private long EncryptionAlgorithmWithC(long digit, Dictionary<long, long> key)
        {
            long e = key.Keys.First();
            long r = key[e];
            return _algorithms.ModPow(digit, e, r);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //блок расшифровки данных
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string DecryptionRSA()
        {
            Console.WriteLine("Расшифрованный массив данных: ");
            _resultDecryptionArray = DecryptionAlgorithmRSA();

            foreach (var i in _resultDecryptionArray)
                Console.Write(i + " ");
            Console.WriteLine();

            string decryptedMessage = ConvertNumbersToLetters(_resultDecryptionArray, _typeInputValues); //храниться расшифрованное сообщение
            DecryptedMessage = decryptedMessage;
            return decryptedMessage;
        }

        public long[] DecryptionAlgorithmRSA()
        {
            DecryptionInFile();

            long[] arrayEncryption = new long[_encryptionDataArrayInFile.Length];

            for (int i = 0; i < _encryptionDataArrayInFile.Length; i++)
            {
                arrayEncryption[i] = Decryption(_encryptionDataArrayInFile[i]);
            }

            return arrayEncryption;
        }

        private long Decryption(long c)
        {
            long dexp = _secretKey.Keys.First();
            long r = _secretKey[dexp];
            return _algorithms.ModPow(c, dexp, r);
        }

        private void DecryptionInFile()
        {
            _encryptionDataArrayInFile = _fileService.OpenFile(_outputPathFile);

            if (_encryptionDataArrayInFile.Length == 0)
                Console.WriteLine("Ошибка, файл пуст...");

            foreach (var i in _encryptionDataArrayInFile)
                Console.Write(i + " ");
            Console.WriteLine();
        }

        private string ConvertNumbersToLetters(long[] numbers, string type)
        {
            string result = "";
            string alphabet = "";

            if (type == "T")
            {
                alphabet = GetFullAlphabet();

                for (int i = 0; i < numbers.Length; i++)
                {
                    int charIndex = (int)numbers[i];

                    if (charIndex >= 0 && charIndex < alphabet.Length)
                    {
                        result += alphabet[charIndex];
                    }
                    else
                    {
                        result += '?';
                    }
                }

                return result;
            }

            if (type == "L")
            {
                alphabet = _latinUpper;
            }
            else if (type == "C")
            {
                alphabet = _cyrillicUpper;
            }
            else if (type == "D")
            {
                return string.Join("", numbers);
            }

            int letterIndex = 0;

            for (int i = 0; i < _originalInputData.Length; i++)
            {
                if (_originalInputData[i] == ' ')
                {
                    result += ' ';
                }
                else
                {
                    if (letterIndex < numbers.Length)
                    {
                        long number = numbers[letterIndex];
                        int charIndex = (int)number; // из 2-27 в 0-25

                        if (charIndex >= 0 && charIndex < alphabet.Length)
                        {
                            result += alphabet[charIndex];
                        }
                        else
                        {
                            result += '?';
                        }

                        letterIndex++;
                    }
                }
            }

            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------ 
        //блок шифрования/дешифрования любых файлов
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void EncryptionAnyFileRSA()
        {
            if (!CheckRSAValues())
                return;

            if (string.IsNullOrWhiteSpace(_inputPathFile))
            {
                MessageBox.Show("Ошибка! Выберите файл для шифрования.");
                return;
            }

            if (!_fileService.IsFileExists(_inputPathFile))
            {
                MessageBox.Show("Ошибка! Выбранный файл не найден.");
                return;
            }

            long r = _q * _p;
            long fuctionEiler = (_q - 1) * (_p - 1);
            long exponent = _exponent;

            long[] dExponentArray = _algorithms.AlgorithmEuclidex(exponent, fuctionEiler);
            long x1 = dExponentArray[0];

            // исправление закрытого ключа
            long dExponent = ((x1 % fuctionEiler) + fuctionEiler) % fuctionEiler;

            _secretKey = new Dictionary<long, long>() { { dExponent, r } };

            byte[] inputBytes = _fileService.ReadBytes(_inputPathFile);
            ushort[] encryptedBlocks = new ushort[inputBytes.Length];

            for (int i = 0; i < inputBytes.Length; i++)
            {
                long encrypted = _algorithms.ModPow(inputBytes[i], exponent, r);
                encryptedBlocks[i] = (ushort)encrypted;
            }

            _fileService.SaveUShortBlocks(_outputPathFile, encryptedBlocks);

            EncryptedData = string.Join(" ", encryptedBlocks);
            MessageBox.Show("Файл успешно зашифрован.");
        }

        public void DecryptionAnyFileRSA()
        {
            if (_secretKey == null || _secretKey.Count == 0)
            {
                MessageBox.Show("Ошибка! Сначала выполните шифрование или задайте закрытый ключ.");
                return;
            }

            long dexp = _secretKey.Keys.First();
            long r = _secretKey[dexp];

            ushort[] encryptedBlocks = _fileService.ReadUShortBlocks(_outputPathFile);
            byte[] decryptedBytes = new byte[encryptedBlocks.Length];

            for (int i = 0; i < encryptedBlocks.Length; i++)
            {
                long decrypted = _algorithms.ModPow(encryptedBlocks[i], dexp, r);
                decryptedBytes[i] = (byte)decrypted;
            }

            string decryptedPath = _inputPathFile + ".decrypted";
            _fileService.SaveBytes(decryptedPath, decryptedBytes);

            DecryptedMessage = "Файл расшифрован: " + decryptedPath;
            MessageBox.Show("Файл успешно расшифрован.");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //блок корректности ввода данных(вынусу потом в отдельный класс ValidateDataClass)
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void IsCorrectInputData()
        {
            string input = _message;

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Ошибка! Введите данные для шифрования.");
                return;
            }

            if (!IsValidInput(input))
            {
                MessageBox.Show("Ошибка! Разрешены только буквы, цифры, пробелы, запятые и знаки препинания.");
                return;
            }

            _originalInputData = input;
            _inputData = input;
        }

        private bool IsValidInput(string input)
        {
            string alphabet = GetFullAlphabet();

            foreach (char c in input)
            {
                if (!alphabet.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }

        private string GetFullAlphabet()
        {
            return _latinUpper + _latinLower + _cyrillicUpper + _cyrillicLower + _digits + _punctuation;
        }

        private bool IsLatin(string text)
        {
            foreach (char c in text)
            {
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return false;
            }

            return true;
        }

        private bool IsCyrillic(string text)
        {
            foreach (char c in text)
            {
                if (!((c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я') || c == 'Ё' || c == 'ё'))
                    return false;
            }

            return true;
        }

        private bool IsDigits(string text)
        {
            foreach (char c in text)
            {
                if (!(c >= '0' && c <= '9'))
                    return false;
            }

            return true;
        }

        private bool IsPrime(long n)
        {
            if (n < 2)
                return false;

            if (n == 2)
                return true;

            if (n % 2 == 0)
                return false;

            for (long i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                    return false;
            }

            return true;
        }

        private long Gcd(long a, long b)
        {
            while (b != 0)
            {
                long t = a % b;
                a = b;
                b = t;
            }

            return a;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
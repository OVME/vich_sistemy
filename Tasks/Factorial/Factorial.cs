using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Factorial
{
    public class Factorial : IParallelableTask
    {
        private BigInteger _factorial;
        private int _number1;
        private int _number2;
        private int _myClientNumber;
        private int _highestClientNumber;
        private InternalPackageProvider _internalPackageProvider;
        public event EventHandler SendEvent;
        public event EventHandler ReadyEvent;
        public Factorial()
        {
            PackageToSend = new InternalPackage();
        }

        public void GetDataFromFile(string filename)
        {

            FileStream fstream;
            try
            {
                fstream = new FileStream(filename, FileMode.Open);
            }
            catch (Exception)
            {
                return;
            }

            TextReader reader = new StreamReader(fstream);

            var data = reader.ReadLine();
            _number1 = 1;
            _number2 = Int32.Parse(data);
            _myClientNumber = 0;
            _highestClientNumber = 0;

        }



        public void LoadTask(int num, int hnum, string s)
        {
            _myClientNumber = num;
            _highestClientNumber = hnum;
            _number1 = Int32.Parse(s.Split(';')[0]);
            _number2 = Int32.Parse(s.Split(';')[1]);
        }

        public Dictionary<int, string> GetDataForWorkers(int num)
        {
            if (_number2 == 0)
                return null;
            var div = _number2/num;
            var dict = new Dictionary<int, string>();
            int begin=0, end=0;
            for (int i = 0; i < num-1; i++)
            {
                begin = div*i+1;
                end = div*(i + 1);
                dict.Add(i,String.Concat(begin.ToString(),";",end.ToString()));
            }
            begin = end + 1;
            end = _number2;
            dict.Add(num - 1, String.Concat(begin.ToString(), ";", end.ToString()));
            return dict;


        }

        public void Execute()
        {
            //это сам алгоритм
            _factorial = 1;
            for (int i = _number1; i < _number2 + 1; i++)
            {
                _factorial *= i;
            }

            Data = _factorial.ToString();

            if (/*_myClientNumber < _highestClientNumber &&*/ _myClientNumber != 0)
            {
                var incomingPackage = _internalPackageProvider.GetPackage();
                var firstMult = BigInteger.Parse(incomingPackage.Data);
                _factorial *= firstMult;
            }
            if (_myClientNumber < _highestClientNumber)
            {
                formPackage(_factorial.ToString(),_myClientNumber+1);
                OnSend();
            }

            if (_myClientNumber == _highestClientNumber)
            {
                formPackage(_factorial.ToString(), -1);
                OnReady();
            }


        }

        private void formPackage(string data, int to)
        {
            PackageToSend.Data = data;
            PackageToSend.From = _myClientNumber;
            PackageToSend.To = to;
        }

        public InternalPackage PackageToSend { get; set; }


        public string Data { get; set; }
        public void SetDataProvider(InternalPackageProvider ipp)
        {
            _internalPackageProvider = ipp;
        }

        public void OnSend()
        {
            var handler = SendEvent;
            if (handler != null) handler(this,EventArgs.Empty);
        }

        public void OnReady()
        {
            var handler = ReadyEvent;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}

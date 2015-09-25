using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Tasks
{
    public interface IParallelableTask
    {
        void GetDataFromFile(string filename);
        void LoadTask(int num, int hnum, string s);
        Dictionary<int, string> GetDataForWorkers(int num);
        void Execute();
        InternalPackage PackageToSend { get; set; }
        
        event EventHandler SendEvent;
        event EventHandler ReadyEvent;
        string Data { get; set; }
        void SetDataProvider(InternalPackageProvider ipp);
    }
}

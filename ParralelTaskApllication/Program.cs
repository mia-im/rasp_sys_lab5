using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ParralelTaskApllication
{
    class XClass
    {
        public void MA(object param) { MB(param); }
        public void MB(object param) { MC(param); }
        object s1 = new object();
        object s2 = new object();
        public void MC(object param)
        {
            if (param == "1")
            {
                MD();
            }
            if (param == "2")
            {
                ME();
            }
            if (param == "3")
            {
                MF();
            }
        }
        public void MD()
        {
            ME();
        }
        public void ME()
        {
            while (true) Thread.SpinWait(int.MaxValue / 20);
        }
        public void MF()
        {
            ML();
        }
        public void MG(object param)
        {
            MH(param);
        }
        public void MH(object param)
        {
            MI(param);
        }
        public void MI(object param)
        {
            if (param == "4")
            {
                MJ();
            }
            else
            {
                MK();

            }
        }
        public void MJ()
        {
            Monitor.Enter(s1); Thread.SpinWait(int.MaxValue / 20);
            Monitor.Enter(s2);
        }
        public void MK()
        {
            Monitor.Enter(s2);
            Thread.SpinWait(int.MaxValue / 10);
            Monitor.Enter(s1);
        }
        public void ML()
        {
            MM();
        }
        public void MM()
        {
            while (true)
            {
                Thread.SpinWait(int.MaxValue / 3);
                Debugger.Break();
            };
        }
    }
    class Program
    {
        static XClass obj = null;
        static void Main(string[] args)
        {
            obj = new XClass();
            Task.Factory.StartNew(obj.MA, "1");
            Task.Factory.StartNew(obj.MA, "2");
            Task.Factory.StartNew(obj.MA, "3");
            Task.Factory.StartNew(obj.MG, "4");
            Task.Factory.StartNew(obj.MG, "5");
            Console.ReadLine();
        }
    }
}

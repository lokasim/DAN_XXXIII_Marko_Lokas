using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading
{
    class Program
    {
        //For maximized console application...
        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        static void Main(string[] args)
        {
            //maximized console app
            ShowWindow(ThisConsole, MAXIMIZE);

            Stopwatch stopWatch = new Stopwatch();

            List<Thread> ThreadList = new List<Thread>();
            ThreadList.Add(new Thread(new ThreadStart(FirstThreadMethod)));
            ThreadList.Add(new Thread(new ThreadStart(SecondThreadMethod)));
            ThreadList.Add(new Thread(new ThreadStart(ThirdThreadMethod)));
            ThreadList.Add(new Thread(new ThreadStart(FourthThreadMethod)));

            int counterName = 1;
            //create threads using for loop
            for (int i = 0; i < ThreadList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ThreadList[i].Name = string.Format("THREAD_" + counterName);

                    string threadName = ThreadList[i].Name.ToString();
                    Console.WriteLine(char.ToUpper(threadName[0]) + threadName.Substring(1).ToLower() + " is created.");
                    counterName++;
                }
                else if (i % 2 == 1)
                {
                    ThreadList[i].Name = string.Format("THREAD_" + counterName + counterName);

                    string threadName = ThreadList[i].Name.ToString();
                    Console.WriteLine(char.ToUpper(threadName[0]) + threadName.Substring(1).ToLower() + " is created.");
                    counterName++;
                }
            }

            ThreadList[0].Priority = ThreadPriority.Normal;
            ThreadList[1].Priority = ThreadPriority.Highest;

            stopWatch.Start();
            //start Thread_1 and Thread_22
            ThreadList[0].Start();
            ThreadList[1].Start();

            //visual representation of random generation of odd numbers
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nPlease wait...");
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            while (ThreadList[1].IsAlive)
            {
                Console.SetCursorPosition(left, top);
                Console.WriteLine($"Loading data {ListOddNumbers.Count / 10} %");
            }
            Console.SetCursorPosition(left, top);

            ThreadList[1].Join();
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("\nExecutin Time for Thread_1 and Thread_22 \n(mm:ss.ms)\n " + elapsedTime);
            Console.ForegroundColor = ConsoleColor.White;


            ThreadList[2].Priority = ThreadPriority.Highest;
            ThreadList[3].Priority = ThreadPriority.AboveNormal;
            //start Thread_3 and Thread_44
            ThreadList[2].Start();
            ThreadList[3].Start();

            Console.ReadLine();
        }

        private static readonly object locker = new object();

        public static void FirstThreadMethod()
        {
            //number of rows and columns in the matrix
            int num = 100;

            //Locks access to the .txt file
            lock (locker)
            {
                using (TextWriter tw = new StreamWriter(@"..\..\FileByThread_1.txt"))
                {
                    for (int i = 0; i < num; i++)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if (i == j)
                            {
                                tw.Write(1);
                            }
                            else
                            {
                                tw.Write(0);
                            }
                        }
                        tw.WriteLine();
                    }
                }
            }
        }

        /// <summary>
        /// Method for generates random numbers
        /// </summary>
        /// <returns></returns>
        static int RandomNumber()
        {
            Random random = new Random();
            return random.Next(0, 10001);
        }

        /// <summary>
        /// List of odd numbers
        /// </summary>
        public static List<int> ListOddNumbers = new List<int>();

        public static void SecondThreadMethod()
        {
            //Locks access to the .txt file
            lock (locker)
            {
                using (TextWriter tw = new StreamWriter(@"..\..\FileByThread_22.txt"))
                {
                    do
                    {
                        int oddNum = RandomNumber();
                        //Prevents random generation of the same numbers
                        //The higher the Thread.Sleep(ms), the better the random works, but the slower it performs
                        Thread.Sleep(1);
                        if (oddNum % 2 == 1)
                        {
                            ListOddNumbers.Add(oddNum);
                            tw.WriteLine(oddNum);
                        }
                    } while (ListOddNumbers.Count != 1000);
                }
            }
        }

        /// <summary>
        /// A method that reads data from a txt file and prints it to the console, 
        /// line by line
        /// </summary>
        public static void ThirdThreadMethod()
        {
            //Locks access to the .txt file
            lock (locker)
            {
                string[] lines = File.ReadAllLines(@"..\..\FileByThread_1.txt");
                foreach (var line in lines)
                {
                    Console.WriteLine($"{line,1}");
                }
            }
        }

        /// <summary>
        /// Method that reads data from a .txt file and adds the sum
        /// </summary>
        public static void FourthThreadMethod()
        {
            lock (locker)
            {
                int totalSumOddNumber = 0;
                int oddNumber;
                string[] lines = File.ReadAllLines(@"..\..\FileByThread_22.txt");
                foreach (var line in lines)
                {
                    oddNumber = Convert.ToInt32(line);
                    totalSumOddNumber += oddNumber;
                }
                Console.WriteLine("\nTOTAL SUM: " + totalSumOddNumber);
            }
        }
    }
}

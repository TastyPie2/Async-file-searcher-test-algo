using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SearchAlgoTest
{
    class Program
    {
        //
        //In terms of speed this is a downgrade
        //

        //global
        public static int filesCount = 0;
        public static int foldersCount = 0;
        public static string keyWord;
        public static bool specificMode = false;
        public static List<Thread> threads = new List<Thread>();


        static void Main()
        {
            Stopwatch searchTime = new Stopwatch();

            Console.WriteLine("Keyword \n-s for specific");
            
            keyWord = Console.ReadLine();

            searchTime.Start();

            if(keyWord.ToLower().StartsWith("-s"))
            {
                keyWord = keyWord.Remove(0,2).TrimStart();

                Console.Title = keyWord;

                specificMode = true;
                
            }
 
            string[] drives = Environment.GetLogicalDrives();

            Console.WriteLine("==========Results==========");

            foreach (string drive in drives)
            {
                Console.WriteLine("Search started in {0}", drive);

                Thread operation = new Thread(() => searchDir(drive));

                operation.Name = string.Format("WorkerThread {0}", drive);

                operation.Priority = ThreadPriority.Highest;

                operation.Start();

                threads.Add(operation);
            }
            
            Thread[] pendingThreads = threads.ToArray();

            bool isDone = false;

            while (!isDone)
            {
                isDone = true;
                for(int i = 0; i<pendingThreads.Length; i++)
                {
                    if(pendingThreads[i].IsAlive)
                    {
                        isDone = false;
                    }
                }
            }

            searchTime.Stop();

            Console.WriteLine("==========Summery==========");
            Console.WriteLine("Elapsedtime {0}ms \nFilesFound: {1} \nFoldersFound: {2} \nPress enter to close this window", searchTime.ElapsedMilliseconds  ,filesCount, foldersCount);

            Console.ReadKey();
            Console.ReadLine();
        }

        static void searchDir(string dir)
        {
            string[] subFolders;
            string[] files;

            if (specificMode)
            {
                if (dir.Split(@"\").Last().Contains(keyWord))
                {
                    new Thread(() => writeWithNewThread(dir)).Start();
                    foldersCount++;
                }
            }
            else
            {
                if (dir.ToLower().Split(@"\").Last().Contains(keyWord))
                {
                    new Thread(() => writeWithNewThread(dir)).Start();
                    foldersCount++;
                }
            }

            try
            {

                subFolders = Directory.GetDirectories(dir);
                files = Directory.GetFiles(dir);
   
            }
            catch(Exception)
            {
                return;
            }
            try
            {

                foreach(string folder in subFolders)
                {
                    try
                    {
                        searchDir(folder);
                        
                    }
                    catch
                    { }
                }
                foreach (string file in files)
                {
                    if (specificMode)
                    {
                        if (file.Split(@"\").Last().Contains(keyWord))
                        {
                            new Thread(() => writeWithNewThread(file)).Start();
                            filesCount++;
                        }
                    }
                    else
                    {
                        if (file.ToLower().Split(@"\").Last().Contains(keyWord))
                        {
                            //i suspected that writing to console was bottlenecking the rest of the program
                            new Thread(() => writeWithNewThread(file)).Start();
                            filesCount++;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static async Task writeWithNewThread(string x)
        {
            Console.WriteLine(x);
        }

    }
}

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
        //global
        public static int filesCount = 0;
        public static int foldersCount = 0;
        public static string keyWord;
        public static bool specificMode = false;
        public static List<Task> tasks = new List<Task>();


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
                 new Thread(() => searchDrive(drive)).Start();
            }
            
            Task[] pendingTasks = tasks.ToArray();

            while(true)
            {
                if(Task.WhenAny(pendingTasks).IsFaulted || Task.WhenAny(pendingTasks).IsCanceled || Task.WhenAny(pendingTasks).IsCompleted)
                {
                    for(int i = 0; i<pendingTasks.Length; i++)
                    {
                        Task task = pendingTasks[i];

                        if(task.IsCanceled || task.IsCompleted || task.IsFaulted)
                        {
                            pendingTasks[i] = null;
                        }
                    }
                }

                bool isDone = false;

                for(int i = 0; i<pendingTasks.Length; i++)
                {
                    try
                    {
                        if (pendingTasks[i] == null)
                        {
                            isDone = true;
                        }
                        else
                        {
                            isDone = false;
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if(isDone)
                {
                    break;
                }
            }

            searchTime.Stop();

            Console.WriteLine("==========Summery==========");
            Console.WriteLine("Elapsedtime {0}ms \nFilesFound: {1} \nFoldersFound: {2} \nPress enter to close this window", searchTime.ElapsedMilliseconds  ,filesCount, foldersCount);

            Console.ReadKey();
            Console.ReadLine();
        }

        static async Task searchDirAsync(string dir)
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
                        searchDirAsync(folder);
                        
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
                            Console.WriteLine(file);
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

        static async Task searchDrive(string drive)
        {
            tasks.Add(searchDirAsync(drive));
        }

    }
}

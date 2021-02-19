using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
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

        static void Main()
        {
            MainAsync();
        }

        async static void MainAsync()
        {
            Stopwatch searchTime = new Stopwatch();

            Console.WriteLine("Keyword \n-s for specific");

            keyWord = Console.ReadLine();

            searchTime.Start();

            if (keyWord.ToLower().StartsWith("-s"))
            {
                keyWord = keyWord.Remove(0, 2).TrimStart();

                Console.Title = keyWord;

                specificMode = true;

            }

            string[] drives = Environment.GetLogicalDrives();

            List<Task> tasks = new List<Task>();

            Console.WriteLine("==========Results==========");

            foreach (string drive in drives)
            {
                tasks.Add(searchDirAsync(drive));
            }

            Task[] pendingTasks = tasks.ToArray();

            bool isDone = false;

            while (!isDone)
            {
                isDone = true;
                for (int i = 0; i < pendingTasks.Length; i++)
                {
                    pendingTasks[i].Wait();
                }
            }

            searchTime.Stop();

            Console.WriteLine("==========Summery==========");
            Console.WriteLine("Elapsedtime {0}ms \nFilesFound: {1} \nFoldersFound: {2} \nPress enter to close this window", searchTime.ElapsedMilliseconds, filesCount, foldersCount);

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
                    Console.WriteLine(dir);
                    foldersCount++;
                }
            }
            else
            {
                if (dir.ToLower().Split(@"\").Last().Contains(keyWord))
                {
                    Console.WriteLine(dir);
                    foldersCount++;
                }
            }

            try
            {
                subFolders = Directory.GetDirectories(dir);
                files = Directory.GetFiles(dir);

            }
            catch (Exception)
            {
                return;
            }
            try
            {

                foreach (string folder in subFolders)
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
                            Console.WriteLine(file);
                            filesCount++;
                        }
                    }
                    else
                    {
                        if (file.ToLower().Split(@"\").Last().Contains(keyWord))
                        {
                            Console.WriteLine(file);
                            filesCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}

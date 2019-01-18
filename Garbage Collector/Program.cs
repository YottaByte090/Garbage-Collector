using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Garbage_Collector
{
    class Program
    {
        private static string targetDir = AppDomain.CurrentDomain.BaseDirectory;
        private static List<string> fileList = new List<string>();

        static void Main(string[] args)
        {
            Console.Title = "Garbage Collector";

            println($"\"{ targetDir }\" 에 있는 쓰레기 파일 정리를 시작합니다.", ConsoleColor.White);
            println("\n파일 목록을 가져오는중...", ConsoleColor.White);

            var files = Directory.GetFiles(targetDir);

            foreach(string file in files)
            {
                println($"파일 추가 : { Path.GetFileName(file) }", ConsoleColor.Gray);
                fileList.Add(file);
            }

            println("\n용량이 같은 파일을 검사하는중...", ConsoleColor.White);

            Dictionary<long, List<string>> duplicateFileSize = new Dictionary<long, List<string>>();

            foreach (var file in fileList)
            {
                println($"용량 검사 : { Path.GetFileName(file) }", ConsoleColor.Gray);

                long size = new FileInfo(file).Length;

                println($"-> { size }Byte", ConsoleColor.Blue);

                if (!duplicateFileSize.ContainsKey(size))
                    duplicateFileSize.Add(size, new List<string>());

                duplicateFileSize[size].Add(file);
            }

            println("\n용량이 같은 파일을 목록으로 만드는중...", ConsoleColor.White);

            List<List<string>> duplicateFile1 = new List<List<string>>();

            foreach (var fileList in duplicateFileSize)
            {
                if(fileList.Value.Count > 1)
                {
                    duplicateFile1.Add(fileList.Value);
                }
            }

            println("\n해쉬가 같은 파일을 검사하는중...", ConsoleColor.White);

            Dictionary<string, string> duplicateFileHash = new Dictionary<string, string>();
            List<string> duplicateFile2 = new List<string>();

            foreach(var fileList in duplicateFile1)
            {
                println($"\n용량이 { new FileInfo(fileList[0]).Length }Byte인 파일의 해쉬를 검사하는중...", ConsoleColor.White);

                foreach (var file in fileList)
                {

                    println($"해쉬 검사 : { Path.GetFileName(file) }", ConsoleColor.Gray);

                    string hash = calculateHash(file);

                    println($"-> { hash }", ConsoleColor.Blue);

                    if (!duplicateFileHash.ContainsKey(hash))
                    {
                        duplicateFileHash.Add(hash, file);
                    }
                    else
                    {
                        println($"중복 발견 : { Path.GetFileName(file) }", ConsoleColor.Green);

                        duplicateFile2.Add(file);
                    }
                }
            }

            if(duplicateFile2.Count > 0)
            {
                println("\n[ 중복 파일 ]", ConsoleColor.White);

                foreach (var file in duplicateFile2)
                {
                    println("* " + Path.GetFileName(file), ConsoleColor.White);
                }

                print("\n모두 삭제하시겠습니까? (y/N) : ", ConsoleColor.White);

                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    foreach(var file in duplicateFile2)
                    {
                        File.Delete(file);
                    }
                }
                else
                {
                    println("사용자가 취소했습니다.", ConsoleColor.Red);
                }
            }
            else
            {
                println("\n중복 파일이 없습니다.", ConsoleColor.Red);
            }


            println("종료하려면 아무 키나 누르십시오...", ConsoleColor.White);
            Console.ReadKey();
        }

        private static string calculateHash(string file)
        {
            string result = "";
            SHA256 hash = SHA256.Create();

            foreach(byte b in hash.ComputeHash(new FileStream(file, FileMode.Open)))
            {
                result += b.ToString("x2");
            }

            return result;
        }

        private static void print(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void println(string str, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

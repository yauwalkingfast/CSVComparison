using System;
using System.Collections.Generic;
using System.IO;
using static System.Environment;
using ExcelPOC;
using System.Linq;

namespace Excel
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, List<Record>> allTables = new Dictionary<string, List<Record>>();
            List<string> fileNames = new List<string>();

            List<Record> missing;
            Dictionary<string, Record> missMatched;
            string baseTable, comparedToTable;

            string folderPath = GetComparePath();
            if (CheckFilesExist(folderPath))
            {
                LoadCSV(folderPath, allTables);
                fileNames = IndexFileName(allTables);
            }

            (int selection1, int selection2) = Selection(fileNames);

            // Extension for other uses case after comparison possible from here
            (missing, missMatched, baseTable, comparedToTable) = Comparison(selection1, selection2, allTables, fileNames);

            DisplayComparison(missing, missMatched, baseTable, comparedToTable);

        }

        private static void DisplayComparison(List<Record> missing, Dictionary<string, Record> missMatched, string baseTable, string comparedTable)
        {
            if (missMatched.Count != 0)
            {
                Console.WriteLine($"\n------- Miss-matched column detected in {comparedTable} -------");
                foreach (var kvp in missMatched)
                {
                    Console.WriteLine($"Mismatched: {kvp.Key} vs {kvp.Value.DataType},{kvp.Value.Precision}");
                }
            }
            else
            {
                Console.WriteLine($"\n------- No missmatch record detected -------\n");
            }

            if (missing.Count != 0)
            {
                Console.WriteLine("\n------- Missing records detected -------");
                foreach (Record record in missing)
                {
                    Console.WriteLine($"{record.TableName},{record.ColumnName},{record.DataType},{record.Precision} is missing in {comparedTable}");
                }
            }
            else
            {
                Console.WriteLine("\n------- No Missing record detected -------");
            }


        }

        private static (List<Record>, Dictionary<string, Record>, string, string) Comparison(int selection1, int selection2, Dictionary<string, List<Record>> allTables, List<string> fileNames)
        {
            List<Record> compare1 = allTables[fileNames[selection1]];
            List<Record> compare2 = allTables[fileNames[selection2]];

            List<Record> missing = new List<Record>(); // can't find in record2
            Dictionary<string, Record> missMatched = new Dictionary<string, Record>();
            foreach (Record record1 in compare1)
            {
                if (!IsHeader(record1))
                {
                    bool matchFound = false;
                    //foreach (Record record2 in compare2)
                    for (int i = 0; i < compare2.Count; i++)
                    {
                        Record record2 = compare2[i];
                        
                        if (!IsHeader(record2))
                        {
                            if (record1.TableName == record2.TableName && record1.ColumnName == record2.ColumnName)
                            {
                                matchFound = true;
                                if (!record1.Equals(record2))
                                {
                                    missMatched.Add($"{record1.TableName},{record1.ColumnName} ----> {record1.DataType},{record1.Precision}", record2);
                                    break; ;
                                }
                            }

                        }
                    }
                    if (!matchFound)
                    {
                        missing.Add(record1);
                    }
                }
            }
            return (missing, missMatched, fileNames[selection1], fileNames[selection2]);
        }

        private static (int, int) Selection(List<string> fileNames)
        {
            if (fileNames is null)
            {
                ExitMessage("No files in Compare folder");
            }

            for (int i = 0; i < fileNames.Count; i++)
            {
                Console.WriteLine($"{i} - {fileNames[i]}");
            }
            Console.WriteLine("Enter two file number to compare\nEnter first file number");
            int.TryParse(Console.ReadLine(), out int selection1);
            Console.WriteLine("Enter second file number");
            int.TryParse(Console.ReadLine(), out int selection2);
            return (selection1, selection2);
        }

        private static List<string> IndexFileName(Dictionary<string, List<Record>> tables)
        {
            return tables.Keys.ToList();
        }

        private static void LoadCSV(string folderPath, Dictionary<string, List<Record>> allTables)
        {
            foreach (string file in Directory.GetFiles(folderPath))
            {
                List<Record> records = new List<Record>();
                StreamReader reader = new StreamReader(file);
                //Check for header
                //Console.WriteLine(Reader.ReadLine());
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    records.Add(new Record(values[0], values[1], values[2], values[3], values[4]));
                }
                string tableName = Path.GetFileNameWithoutExtension(file);
                allTables.Add(tableName, records);
            }
        }

        public static string GetComparePath()
        {
            Console.WriteLine("Loading CSV files........");
            string targetFolder = Path.Combine(GetFolderPath(SpecialFolder.Desktop), "Compare");

            // Folder exist?
            if (!Directory.Exists(targetFolder))
            {
                ExitMessage("Compare folder in Desktop does not exist.\nExiting Program");
            }
            return targetFolder;
        }

        public static bool CheckFilesExist(string path)
        {
            // files exist in folder? 
            if (Directory.GetFiles(path).Length == 0)
            {
                ExitMessage("No files in folder");
            }
            if (Directory.GetFiles(path).Length == 1)
            {
                ExitMessage("Not enough files to compare");
            }
            return true;
        }


        public static void ExitMessage(string message)
        {
            Console.WriteLine($"Fail to load CSV: { message}");
            Exit(1);
        }

        public static bool IsHeader(Record record)
        {
            return record.Equals(new Record("TableName", "ColumnName", "ColumnNumber", "DataType", "Precision"));
        }
    }
}

// Get User's download folder
// string userPath = GetFolderPath(SpecialFolder.UserProfile);
// string downloadFolderPath = Path.Combine(GetFolderPath(SpecialFolder.UserProfile), "Downloads");
// Console.WriteLine(downloadFolderPath);

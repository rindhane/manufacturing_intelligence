using System.IO; //to get acces to the FileStream API 
using System.Text; //to get the encoding.UTF8
using System.Collections.Generic;

namespace FileHandler { 

    public interface IFileHandler{
        public void writeCandidates(string name, string surname);
        public List<(string, string)> ReadCandidates();
    }
    public class ResultHandler:IFileHandler {
        
        private string _path ;
        public ResultHandler(string path) {
            _path = path;
            //_path = System.Environment.GetEnvironmentVariable("logFile");
        }

        public void writeCandidates(string name, string surname) {
            int buffer=4096;
            FileStream fs= new FileStream(_path,
                                        FileMode.Append,
                                        FileAccess.Write,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);//File.AppendText(_path);
            string timeStamp=System.DateTime.Now.ToString("dd MMM HH:mm:ss");
            string note = $"{name}"+$"~{surname}"+$"~{timeStamp}"; 
            sw.WriteLine(note);
            sw.Close();
            sw.Dispose();
        }

        public List<(string, string)> ReadCandidates(){
            int buffer=4096;
            FileStream fs= new FileStream(_path,
                                        FileMode.OpenOrCreate,
                                        FileAccess.Read,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            var sr = new StreamReader(fs, Encoding.UTF8);//File.AppendText(_path);
            string line;
            string[] temp;
            var Candidates = new List<(string, string)>();
            while((line = sr.ReadLine()!) != null) {
                temp = line.Split("~");
                Candidates.Add((temp[0],temp[1]));
            }
            sr.Close();
            sr.Dispose();
            return Candidates;
        }
    }
}

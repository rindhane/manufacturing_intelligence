using System.IO; //to get access to FileStream;
using System.Text; //to get accsss to Encoding.UTF8;
using System.Collections.Generic;

namespace DFQhandler{


    class DFQWriter{

        string _filePath{get;set;}
        public DFQWriter (string filePathString){
            _filePath=filePathString;
        }
        
        public bool fileTextWriter(List<System.Tuple<string,string>> writeInput, int position, bool ISheader=false ) {
            int buffer=4096;
            checkForParentDirectory();
            FileStream fs= new FileStream(_filePath,
                                        FileMode.Append,
                                        FileAccess.Write,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            foreach (System.Tuple<string,string> vals in writeInput ){
                if(!ISheader) {
                    sw.WriteLine("{0}/{1} {2}", vals.Item1, position.ToString() ,vals.Item2); 
                    continue;   
                }
                sw.WriteLine("{0} {1}", vals.Item1 ,vals.Item2);
            }
            sw.Close();
            sw.Dispose();
            return true;
        }

        public void checkForParentDirectory(){
            if(!System.IO.Directory.Exists(_filePath)){
                var parentDir = System.IO.Directory.GetParent(_filePath);
                System.IO.Directory.CreateDirectory(parentDir!.FullName);
            }
        }

        public void writeHeader(int characteristic_count){
            var temp = new List<System.Tuple<string,string>>{
                new System.Tuple<string,string>("K0100", characteristic_count.ToString())
            };
            fileTextWriter(temp,1,true);
        }
         
    }

}
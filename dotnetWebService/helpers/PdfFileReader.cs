using System.IO; //to get acces to the FileStream API 
using System.Text; //to get the encoding.UTF8
using System.Threading.Tasks;

namespace pdfFileReader{
    class PdfDataProvider{
        //Following class provides functions to provide data from the pdf File 
        string _path ; //path to the file
        public PdfDataProvider(string path){
            _path =path ;
        }
        public async Task<byte[]> ReadFileData(){
            byte[]result= await File.ReadAllBytesAsync(_path);
            return result;
        }
    }
}
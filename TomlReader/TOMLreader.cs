using Tomlyn; // to access the Toml class
using Tomlyn.Model; // to access the TomTable Type
using System.IO; // to access the fileStream class
using System.Text; // to access the encoding type
using System.Collections.Generic; // to access the dictionary type

namespace TOMLreader{
    public class tomlConfigReader{

        string configFile ; 
        public tomlConfigReader(string path) {
            configFile = path;
        }
        string configFromFile(){
            int buffer=4096;
            FileStream fs= new FileStream(configFile,
                                        FileMode.OpenOrCreate,
                                        FileAccess.Read,
                                        FileShare.ReadWrite,
                                        buffer, FileOptions.Asynchronous);
            var sr = new StreamReader(fs, Encoding.UTF8);
            string toml = sr.ReadToEnd();            
            sr.Close();
            sr.Dispose();
            return toml;
        }
        
        public TomlTable getTomlTable(){
            var toml = configFromFile();
            var model=Toml.ToModel(toml);
            return model;
        }
        public object getKeyValue(string key, string table=""){
            var model=getTomlTable();
            if (table.Equals(string.Empty)){
                //provide the bareKey
                return model[key];
            }
            return ((TomlTable)model[table])[key];
        }
        public T[] getArrayType<T>(string key, string table=""){
            var temp3=(TomlArray)getKeyValue(key,table);
            var result = new T[temp3.Count];
            for(int i =0; i<temp3.Count; i++){
                result[i]=(T)temp3[i]!;
            }
            return result;
        }

        public Dictionary<string,string> getSetOfTableValues(string tableName,List<string>keys){
            var model=getTomlTable();
            var table = (TomlTable)model[tableName];
            var result = new Dictionary<string,string>();
            foreach(string key in keys){
                if(table.ContainsKey(key)){
                    result.Add(key,(string)table[key]);
                }
            }
            return result;
        }
    }
} 


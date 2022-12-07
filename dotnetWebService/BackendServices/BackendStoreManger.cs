using System.Collections.Generic;
using System.Threading.Tasks;
using App.Configurations; // for accessing the runTimeConfiguration instance keeper
using JWT; //for accessing the TokenHandler class

namespace BackendStoreManager {
public class authKeeper{

    Dictionary<string,object> authRecords;

    public authKeeper(){
        authRecords= new Dictionary<string, object>();
    }
    public async Task initiateTempAuth(string key, object matter) {
        authRecords.Add(key,matter);
        await Task.Delay(30*1000); //30 secods before auth gets vanished;
        authRecords.Remove(key);
        return ;    
    }

    public bool checkAuthExist(string key) {
        return authRecords.ContainsKey(key);
    }
}

public class TokenManager{
    string secretkey;
    public TokenHandler handler;
    
    public TokenManager(runTimeConfiguration keeper){
        secretkey=keeper.getTokenHandlerSecret();
        handler = new TokenHandler(secretkey);
    }
}
}
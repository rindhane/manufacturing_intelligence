using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using helpers;

namespace JWT
{
    
    public static class TestHandler {
        public static void HandlerTest (){
            var handler =new TokenHandler("123");
            handler.payloadBuilder.name="name1";
            handler.payloadBuilder.subject="auth1";
            handler.payloadBuilder.surname="surname1";
            var token = handler.generateToken();
            System.Console.WriteLine(handler.JWTSigner(token));
        } 
    }
    
    public class TokenHandler {         
        string secretKey{get;set;} 
        public PayloadBuilder payloadBuilder; 

        public TokenHeadBuilder tokenHeadBuilder ;
        public TokenHandler(string key, string type="default") {
            this.secretKey=key;
            this.payloadBuilder = new PayloadBuilder();
            this.tokenHeadBuilder = new TokenHeadBuilder(type);
        }
        
        public Token generateToken(){
            var tokenHead=this.tokenHeadBuilder.Create();
            var payload=this.payloadBuilder.NewCreate();
            var token = new Token{
                head=tokenHead,
                payload=payload
            };
            return token;
        }
        public string JWTSigner(Token token){
            var hash = computeTokenHash(token);
            var signature= WebEncoders.Base64UrlEncode(hash);
            var result = UnsignedTokenString(token)+"."+ signature;
            return result;
        }

        public byte[] computeTokenHash(Token token){
            string unSignedString=UnsignedTokenString(token);
            var collection=System.Text.Encoding.UTF8.GetBytes(unSignedString);
            //creating hash : 
            var hash = generateHash(collection);
            return hash;
        }

        public string UnsignedTokenString(Token token){
            var header_string= WebEncoders.Base64UrlEncode( 
                System.Text.Encoding.UTF8.GetBytes( this.getHeaderJsonString(token.head))
            );
            var payload_string= WebEncoders.Base64UrlEncode(
                System.Text.Encoding.UTF8.GetBytes( this.getPayLoadJsonString(token.payload) ) 
            );
            string UnsignedString=header_string+"."+payload_string;
            return UnsignedString;
        }

        public string getHeaderJsonString(Token_header head)
        {
            string json = JsonConvert.SerializeObject(head);
            return json;
        }
        
        public string getPayLoadJsonString(JPayload payload) {
            string json = JsonConvert.SerializeObject(payload);
            return json;
        }

        public string newSecretKeyGenerate (){
            this.secretKey=WebEncoders.Base64UrlEncode(RandomGenerators.RandomByteGenerators(64));
            return this.secretKey;
        }

        public byte[] generateHash(byte[] collection){
            var hmac= new HMACSHA256(WebEncoders.Base64UrlDecode(this.secretKey));
            byte[] computeHash = hmac.ComputeHash(collection);
            hmac.Dispose();
            return computeHash;
        }

        public static T unwrapFromString<T>(string s){
            var result= WebEncoders.Base64UrlDecode(s);
            string temp = System.Text.Encoding.UTF8.GetString(result);
            T load= JsonConvert.DeserializeObject<T>(temp);
            return load;
        }

        public bool validateStringToken(string s){
            var temp = UnpackTokenElementFromString(s);
            Token_header head = temp.Item1;
            JtPayload pLoad = temp.Item2;
            string signature = temp.Item3;    
            var tempLoad = fromJtPayload(pLoad);
            Token token = new Token{
                head=head,
                payload=tempLoad
            };
            var hash = computeTokenHash(token); 
            bool result = WebEncoders.Base64UrlEncode(hash).Equals(signature);
            return result;
        }

        public (Token_header,JtPayload,string) UnpackTokenElementFromString(string s ) {
            var temp = s.Split(".");
            string head = temp[0];
            string payload=temp[1];
            string signature = temp[2];
            var pLoad= unwrapFromString<JtPayload>(payload);
            var tempHead = unwrapFromString<Token_header>(head);
            return (tempHead,pLoad,signature);
        }

        public static JPayload fromJtPayload(JtPayload load){
            var payload= new JPayload();
            payload.name=System.String.Copy(load.name);
            payload.subject =System.String.Copy(load.subject);
            payload.surname =System.String.Copy(load.surname);
            payload.iat = System.String.Copy(load.iat);
            return payload;
        }


    }

    public class Token {
        
        public Token_header head{get;set;}

        public JPayload payload{get;set;}

    } 

    public class Token_header {
        public string alg {get; set;}
        public string typ {get; set;}
    }

    public class TokenHeadBuilder {
        public string alg {get; set;}
        public string typ {get; set;}
        public TokenHeadBuilder(string type){
            if (!"HS256".Equals(type) && !"default".Equals(type)){
                throw new System.Exception("type is not recognized");
            }
            this.alg="HS256";
            this.typ="JWT";
        }
        public Token_header Create(){
            if(alg!=null && typ!=null ) {
                var tokenHead = new Token_header {
                    alg=alg,
                    typ=typ
                };
                return tokenHead;
            }
            throw new System.Exception("head type details not provided");
        }

        
    }

    public class JPayload {

        public string subject {get;set;}
        public string name {get; set;}

        public string surname {get; set;}

        public string iat {get;set;}

    }

    public class JtPayload : JPayload {

        public string auth {get;set;}

    }

    public class PayloadBuilder {
        public string subject {get;set;}
        public string name {get; set;}

        public string surname {get; set;}

        public string auth {get;set;}

        public string iat {get;set;}

        public PayloadBuilder(){}
        public JPayload NewCreate(){
            if(subject!=null && name!=null && surname!=null ) {
                iat = ((System.DateTimeOffset)System.DateTime.Now).ToUnixTimeSeconds().ToString();
                var payload = new JPayload();
                payload.subject=subject;
                payload.name=name;
                payload.surname=surname;
                payload.iat=iat;
                return payload;
            }
            throw new System.Exception("payload attributes are not complete");
        }
    }

    public static class PayloadTester{
        public static string PayloadValues(string payload){
           var result= WebEncoders.Base64UrlDecode(payload);
           string temp = System.Text.Encoding.UTF8.GetString(result);
           JtPayload load= JsonConvert.DeserializeObject<JtPayload>(temp);
           System.Console.WriteLine($"{load.auth};{load.name};{load.subject};{load.surname};{load.iat}");
           return temp;
        }
    }  
    
}
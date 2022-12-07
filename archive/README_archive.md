## Objective of Test
The main objective is to build a test web-client which can communicate with server over http-protocol based api to register itself by following the indicated below procedures. The success criteria of the task completion is to have the user name register on the site's local url-endpoint : `/Results`.    


## Test Instructions/Procedures 
Build a client to interact with the server as per the following handshake procedures :  

### Steps to capture all the flags through the webclient :
1. Initiate with test's start page with the ***HTTP-GET-REQUEST*** to url-endpoint `/testStart`.
1. Successful completion of step-1 will provide ***Cookie*** for authoriziation access associated with the tag : **Flag1**
1. The request success can be verified with the response body containing statement `cookie updated`     
1. Make sure to capture and preserve this cookie with the client as it will act as the authorization token for the subsequent steps. Also make sure to always pass this cookie to the server when sending the subsequent requests to server. This cookie is referred as the first flag *(flag1)* capture in subsequent instructions.  
1. Next step is to send XML document within the body of ***HTTP-POST-REQUEST*** as a data payload string as UTF-8 encoded bytes. 
1. Pass the captured value from the flag1 within the xml element `Auth-Code` of the provided xml document structure *(indicate below)* and pass this xml document as a *well-formed XML 1.0 document* within the body-payload of a ***HTTP-POST-REQUEST*** to the url-endpoint `/checkAuthorization`:  
    1. xml-payload document structure is shown below: 
    ```
    <payload>
        <UserDetails>
            <Name>ABCD</Name>
            <Surname>EFGH</Surname>
        </UserDetails>
        <Secret>
            <Auth-Code>ASDFGH</Auth-Code>
        </Secret>
    </payload>
    ```
    1. Build the xml document using the above structure. 
    1. Update the value of elements values  fields: Name, Surname, Auth-Code with your relevant values. This values will be used for registration in the subsequent steps
    1. Make sure to prepare the XML document string utilizing the `UTF-8` encoding.
1. The successful submission of above XML-auth as  `POST-Request` will result in following `HTTP-Response`: 
    1. The `Response-Body` will contain the following statement : `authorized`
    1. The `Response-Status` will be `307` for Re-direction to next steps.
    1. Updated `Response-Headers` will have the URL-endpoint *(U1)* for next steps.
    1. `Response-Headers` will have a header with `JWT` tag containing the authentication token in the form of `JSON web token (JWT)` signed by the server. *This JWT is second flag (flag2) capture*.
1. Make sure to capture this JWT and next url-endpoint from the previous post-request and preserve within the client. These detail are required in subsequent steps. 
1. Now you have to prepare the JWT for next submission. Follow these sub-procedures to prepate he JWT for submission.
    1. Isolate the payload section from the JWT. Preserve the head & signature. 
    1. Unpack the payload to Json format.
    1. Within the Json Payload, add one more `name-value pair` within the existing name-value pairs hierarchy. Put this new name-value pair at the last of all the pairs during serialization. Also make sure the serialization maintain the same order of the name-value pairs as provided in the original JWT.   
    1. The new `name-value pair` for addition is `auth:flag1`. Substitute the flag1 with the value received in the flag1.  
    1. Serialize the resultant JSON payload back to base64. 
    1. Repack the JWT with existing head & signature and the modified payload.
1. Pass this JWT through `HTTP-POST-REQUEST` against the same header `JWT` tag to the url-endpoint *(U1)* received in the previous `response-header`.
1. On successful submission of above `POST-Request` , following response will be received from the server :
    1. The `Response-Body` will contain the following statement : `authorized`.
    2. The `Response-Status` will be `200`.  
1. This successfully completes the registration of your client with the server. Check-out the url-endpoint `/Result`, it will reflect your name (provided in the xml) within the **Candidate Results** list. 

## After succesfull completion of web-client :  

1. Re-factor and organize client code into more demonstrable structure for review.
1. Add comments for proper understanding of the code execution.
1. (Optional) Add documentation to the client code for better explanation of code structure.
1. (Optional) Add functional tests to demonstrate the TDD concepts.
1. (Optional) Demonstrating the functionality through the web-page(frontend) based web-client will earn additional points in interview.  
1. Take the screenshot of the `/Result` page and put in the top-level/root directory of the code.
1. Push the code to a public git repository (preferably github ).
1. Share the git repo link for review.      

### Notes :
1. For help on JWT, please refer following [site](https://jwt.io/) for good understanding on their functioning.
1. Make sure to develop a web/http client to complete the entire handshake process as TTL for entire registration process is `30 secs` 
1. Only use http mode (not https) for GET & POST request, since SSL certificate acceptance will create additional issue.
1. The web client for the qualfication test can be prepared through utilizing standard libaries available any language. Preference will be given to dotnet/C# or javascript based web client.
1. Usage of the third-party packages through inbuilt-package managers of the base progamming-languages are allowed, but refrain on relying on such third-party solution unless they are popular extensions of the language domain.
1. Preference will be given to the client-solution built & compiled only through tool-chain comprising code-editor and language's common runtime accessibile through command line. Avoid GUI based IDEs. 
1. If the client is built upon ASP.net then use the dotnet-core libraries (3.1+, 6+ etc) instead of any dotnet framework.
1. If the client is built using javascript then don't use node runtime. If want to use the node-runtime then use webpack to compile the code to ES6 javascript and demonstrate through HTML based frontend. 
1. Utilizing SPA framework like react , angular etc are allowed as well as encouraged for building web-client.
1. If stuck somewhere, please ask query through email by sending it to *rahul.indhane@hexagon.com*.              


***Best of Luck for the qualification test ***


XML Serialization in Post request : 
https://stackoverflow.com/questions/43800307/http-post-request-c-sharp-xml
https://stackoverflow.com/questions/1564718/using-stringwriter-for-xml-serialization
https://stackoverflow.com/questions/17535872/http-post-xml-data-in-c-sharp

JWT : 
https://jwt.io/
https://www.geeksforgeeks.org/json-web-token-jwt/


Markdown to html: 
https://markdowntohtml.com/


cookies in http client 
https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies 
https://www.c-sharpcorner.com/article/asp-net-core-working-with-cookie/


ASp authentication : 
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0

#r "Newtonsoft.Json"
#r "System.Configuration"

using System.Net; 
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

public static async Task<String> Run(TimerInfo myTimer, TraceWriter log)

{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");  
    //log.Verbose($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    string resourceId = "https://graph.microsoft.com";
    string tenantId = "c9687145-521f-42e6-a002-c2df404a71ee";
    string authString = "https://login.microsoftonline.com/" + tenantId;
    string upn = String.Empty;
    string clientId = "a8c646c9-abbe-470b-9522-3404c2dde1fe";
    //string clientSecret = "ROqZv3exzu53CVZxNIQt1izs8nzX280NuEYjSeE3HWg=";
    string clientSecret = ConfigurationManager.AppSettings["clientSecret"];


    log.Verbose("ClientSecret=" + clientSecret);
    log.Verbose("authString=" + authString);

    var authenticationContext = new AuthenticationContext(authString, false);
    
    // Config for OAuth client credentials 
    ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
    AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(resourceId,clientCred);
    string token = authenticationResult.AccessToken;
    log.Verbose("token=" + token);

    var responseString = String.Empty;
    
    using (var client = new HttpClient())
    {        
        string requestUrl = "https://graph.microsoft.com/v1.0/users";
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        log.Verbose(request.ToString());
        
        HttpResponseMessage response = client.SendAsync(request).Result;

        responseString = response.Content.ReadAsStringAsync().Result;  

        log.Verbose(responseString);
    } 

    return responseString;

}
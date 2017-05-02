using IdentityModel.Client;
using Newtonsoft.Json;
using ServiceParameters;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ConsoleClient.ClientCredentials
{
    class Program
    {
        const string clientId = "";
        const string clientSecret = "";
        const string scope = "";

        static void Main(string[] args)
        {
            var token = RequestToken(Params.TokenEndpoint, clientId, clientSecret, scope);

            if (token.IsError)
            {
                Console.WriteLine("An error occured while requesting token:", token.Error);
                return;
            }

            CreatePrincipal(Params.APIEndpoint, Params.PrincipalController, token);
        }

        static TokenResponse RequestToken(string tokenEndpoint, string clientId, string clientSecret, string scope)
        {
            Console.WriteLine("Requesting token....\n");

            var tokenResponse = new TokenClient(tokenEndpoint, clientId, clientSecret)
               .RequestClientCredentialsAsync(scope)
               .Result;

            OutputTokenInfoToConsole(tokenResponse);

            return tokenResponse;
        }

        static void OutputTokenInfoToConsole(TokenResponse token)
        {
            if (token.IsError)
            {
                Console.WriteLine("ErrorType: {0}", token.ErrorType);
                Console.WriteLine("Error: {0}", token.Error);
                Console.WriteLine("ErrorDescription: {0}", token.ErrorDescription);
                
                return;
            }
            
            Console.WriteLine(token.Json);
            Console.WriteLine();
        }

        static void CreatePrincipal(string apiBaseAddress, string controller, TokenResponse tokenResponse)
        {
            Console.WriteLine("Calling API....\n");

            var httpClient = new HttpClient() { BaseAddress = new Uri(apiBaseAddress) };

            httpClient.SetBearerToken(tokenResponse.AccessToken);

            // fill here required properties 
            var principal = new Principal() {

                AccountName = "email@email.com",
                CompanyId = 11698,
                FirstName = "John",
                LastName = "Fakename",
                Position = "Project Manager"
            };

            var principalJson = JsonConvert.SerializeObject(principal);

            var response = httpClient
                .PostAsync(controller, new StringContent(principalJson))
                .Result;

            Console.WriteLine(response);
        }
    }

    public class Principal
    {
        /// <summary>
        /// User first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Other field values defined as Dictionary (KeyValuePair)
        /// Key = attribute shortName,
        /// Value: attribute value 
        /// </summary>
        public Dictionary<string, string> OtherAttributeValues { get; set; }

        /// <summary>
        /// User account name (email)
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Active - allows to create active or inactive user.
        /// Inactive user is unable to login
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// A Company ID to which person belongs
        /// </summary>
        public long CompanyId { get; set; }
    }
}

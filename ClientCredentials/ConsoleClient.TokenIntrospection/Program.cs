using IdentityModel.Client;
using System;
using System.Linq;
using ServiceParameters;

namespace ConsoleClient.TokenIntrospection
{
    /// <summary>
    /// This example for client which has JsonWebTokens
    /// </summary>

    class Program
    {
        const string clientId = "";
        const string clientSecret = "";
        const string scope = "";

        static void Main(string[] args)
        {
            var tokenResponse = ReqeustToken(Params.TokenEndpoint, clientId, clientSecret, scope);

            TokenIntrospection(Params.IntrospectionEndpoint, tokenResponse.AccessToken, scope, clientSecret);
        }

        static TokenResponse ReqeustToken(string tokenEndpoint, string clientId, string clientSecret, string scope)
        {
            Console.WriteLine("Requesting token....\n");

            var tokenResponse = new TokenClient(tokenEndpoint, clientId, clientSecret)
               .RequestClientCredentialsAsync(scope)
               .Result;

            OutputTokenInfoToConsole(tokenResponse);

            return tokenResponse;
        }

        private static void TokenIntrospection(string introscpectionEndpoint, string accessToken, string scope, string secret)
        {
            var client = new IntrospectionClient(introscpectionEndpoint, scope, secret);

            var request = new IntrospectionRequest
            {
                Token = accessToken
            };

            var result = client.SendAsync(request).Result;

            if (result.IsError)
            {
                Console.WriteLine(result.Error);
            }
            else
            {
                if (result.IsActive)
                {
                    var claims = result.Claims.ToList();
                    var expirationClaim = claims.FirstOrDefault(x => x.Type == "exp");
                    var validFromClaim = claims.FirstOrDefault(x => x.Type == "nbf");

                    long expirationTimeStamp = Convert.ToInt64(expirationClaim.Value);
                    long validFromTimeStamp = Convert.ToInt64(validFromClaim.Value);

                    var expirationDate = TimeStampToDate(expirationTimeStamp);
                    var issuedDate = TimeStampToDate(validFromTimeStamp);
                }
                else
                {
                    Console.WriteLine("token is not active");
                }
            }
        }

        static DateTime TimeStampToDate(long timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
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
    }
}

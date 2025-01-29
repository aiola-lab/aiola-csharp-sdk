using System;
using System.Collections.Generic;

public static class AuthService
{
    public static Dictionary<string, string> GetAuthHeaders(string authType, Dictionary<string, string> authCredentials)
    {
        if (authCredentials == null)
        {
            throw new AuthenticationError("Authentication credentials are required.");
        }

        switch (authType)
        {
            case "Cookie":
                if (!authCredentials.ContainsKey("cookie") || string.IsNullOrEmpty(authCredentials["cookie"]))
                {
                    throw new AuthenticationError("Cookie value is required for Cookie authentication.");
                }
                return new Dictionary<string, string> { { "Cookie", authCredentials["cookie"] } };

            case "Bearer":
                if (!authCredentials.ContainsKey("token") || string.IsNullOrEmpty(authCredentials["token"]))
                {
                    throw new AuthenticationError("Token is required for Bearer authentication.");
                }
                return new Dictionary<string, string> { { "Authorization", $"Bearer {authCredentials["token"]}" } };

            case "x-api-key":
                if (!authCredentials.ContainsKey("api_key") || string.IsNullOrEmpty(authCredentials["api_key"]))
                {
                    throw new AuthenticationError("API key is required for x-api-key authentication.");
                }
                return new Dictionary<string, string>
                {
                    { "x-api-key", authCredentials["api_key"] },
                    { "Content-Type", "application/json" }
                };

            default:
                throw new AuthenticationError($"Unsupported authentication type: {authType}");
        }
    }
}
public static class AuthUtils
{
    public static KeyValuePair<string, string> GetBearerAuthHeader(Dictionary<string, string> authCredentials)
    {
        // Check if the Authorization header already exists
        if (authCredentials.ContainsKey("Authorization"))
        {
            return new KeyValuePair<string, string>("Authorization", authCredentials["Authorization"]);
        }

        // Check for a raw token and construct the Authorization header
        if (authCredentials.ContainsKey("token") && !string.IsNullOrEmpty(authCredentials["token"]))
        {
            return new KeyValuePair<string, string>("Authorization", $"Bearer {authCredentials["token"]}");
        }

        // If neither is provided, throw an error
        throw new AuthenticationError("Token or Authorization header is required for Bearer authentication.");
    }
}
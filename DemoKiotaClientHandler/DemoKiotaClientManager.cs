using KiotaPosts.Client;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace DemoKiotaClientHandler;

public static class DemoKiotaClientManager
{
    public static PostsClient GetDemoApiClient()
    {
        var authProvider = new AnonymousAuthenticationProvider();
        var adapter = new HttpClientRequestAdapter(authProvider);
        return new PostsClient(adapter);
    }

}

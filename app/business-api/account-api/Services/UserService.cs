public class UserService : IUserService
{
    private readonly Dictionary<string, Account> _accounts;

    public UserService()
    {
        _accounts = new Dictionary<string, Account>
        {
            {
                "alice.user@contoso.com",
                new Account(
                    "1000",
                    "alice.user@contoso.com",
                    "Alice User",
                    "USD",
                    "2022-01-01",
                    "5000",
                    null
                )
            },
            {
                "bob.user@contoso.com",
                new Account(
                    "1010",
                    "bob.user@contoso.com",
                    "Bob User",
                    "EUR",
                    "2022-01-01",
                    "10000",
                    null
                )
            },
            {
                "charlie.user@contoso.com",
                new Account(
                    "1020",
                    "charlie.user@contoso.com",
                    "Charlie User",
                    "EUR",
                    "2022-01-01",
                    "3000",
                    null
                )
            }
        };
    }

    public List<Account> GetAccountsByUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            return new List<Account>();

        return _accounts.TryGetValue(userName, out var account)
            ? new List<Account> { account }
            : new List<Account>();
    }
}

public class LoggedUserService : IUserService
{
    public LoggedUser GetLoggedUser()
    {
        return GetDefaultUser();
    }

    private LoggedUser GetDefaultUser()
    {
        return new LoggedUser("bob.user@contoso.com", "bob.user@contoso.com", "generic", "Bob The User");
    }
}

;
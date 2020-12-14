namespace VeloGreen.Users.Api.Verifiers
{
    public interface IAccessVerifier
    {
        bool HaveAccess(string type, string value);
    }
}

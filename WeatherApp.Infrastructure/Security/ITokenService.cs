namespace WeatherApp.Infrastructure.Security;

public interface ITokenService
{
    string CreateToken(string username, string role);
}
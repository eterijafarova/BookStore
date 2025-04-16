namespace BookShop.Auth.DTOAuth.Requests;

public record RefreshTokenRequest(string username, string refreshToken);
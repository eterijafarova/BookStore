namespace BookShop.Auth.DTOAuth.Responses;

public record RefreshTokenResponse(string accessToken, string refreshToken);
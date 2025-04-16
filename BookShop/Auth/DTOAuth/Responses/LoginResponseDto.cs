namespace BookShop.Auth.DTOAuth.Responses;

public record LoginResponse(string accessToken, string refreshToken);
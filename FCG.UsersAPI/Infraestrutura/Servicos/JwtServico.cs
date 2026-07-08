// ============================================================
// JwtServico.cs — Geração de tokens JWT
// ============================================================

using FCG.UsersAPI.Domain.Entidades;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG.UsersAPI.Infraestrutura.Servicos;

public class JwtServico
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int    _expirationMinutes;

    public JwtServico(IConfiguration configuration)
    {
        _secret            = configuration["Jwt:Secret"]!;
        _issuer            = configuration["Jwt:Issuer"]!;
        _audience          = configuration["Jwt:Audience"]!;
        _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"]!);
    }

    public string GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role,               usuario.Role)
        };

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
// ============================================================
// PasswordHasherServico.cs — Hash de senha com PBKDF2
// ============================================================

using FCG.UsersAPI.Domain.Entidades;
using Microsoft.AspNetCore.Identity;

namespace FCG.UsersAPI.Infraestrutura.Servicos;

public class PasswordHasherServico
{
    private readonly IPasswordHasher<Usuario> _hasher;

    public PasswordHasherServico(IPasswordHasher<Usuario> hasher)
    {
        _hasher = hasher;
    }

    public string GerarHash(string senha)
        => _hasher.HashPassword(null!, senha);

    public bool VerificarSenha(string senhaInformada, string hashSalvo)
        => _hasher.VerifyHashedPassword(null!, hashSalvo, senhaInformada)
           != PasswordVerificationResult.Failed;
}
// ============================================================
// Usuario.cs — Agregado raiz do contexto Identidade
// Copiado da Fase 1 e adaptado para microsserviço
// ============================================================

using System.Text.RegularExpressions;

namespace FCG.UsersAPI.Domain.Entidades;

public class Usuario
{
    public Guid     Id           { get; private set; }
    public string   Name         { get; private set; } = string.Empty;
    public string   Email        { get; private set; } = string.Empty;
    public string   PasswordHash { get; private set; } = string.Empty;
    public string   Role         { get; private set; } = string.Empty;
    public DateTime CreatedAt    { get; private set; }

    private Usuario() { }

    public static Usuario Create(
        string name,
        string email,
        string senha,
        string passwordHash,
        string role = "User")
    {
        ValidarNome(name);
        ValidarEmail(email);
        ValidarSenha(senha);
        ValidarRole(role);

        return new Usuario
        {
            Id           = Guid.NewGuid(),
            Name         = name.Trim(),
            Email        = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role         = role,
            CreatedAt    = DateTime.UtcNow
        };
    }

    public void UpdateProfile(string name, string senha, string passwordHash)
    {
        ValidarNome(name);
        ValidarSenha(senha);
        Name         = name.Trim();
        PasswordHash = passwordHash;
    }

    public void ChangeRole(string newRole)
    {
        ValidarRole(newRole);
        Role = newRole;
    }

    public static void ValidarSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new DomainException("Senha é obrigatória.");
        if (senha.Length < 8)
            throw new DomainException("Senha deve ter no mínimo 8 caracteres.");
        if (!senha.Any(char.IsUpper))
            throw new DomainException("Senha deve conter pelo menos uma letra maiúscula.");
        if (!senha.Any(char.IsLower))
            throw new DomainException("Senha deve conter pelo menos uma letra minúscula.");
        if (!senha.Any(char.IsDigit))
            throw new DomainException("Senha deve conter pelo menos um número.");
        if (!senha.Any(ch => !char.IsLetterOrDigit(ch)))
            throw new DomainException("Senha deve conter pelo menos um caractere especial.");
    }

    private static void ValidarNome(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");
        if (name.Trim().Length < 2)
            throw new DomainException("Nome deve ter pelo menos 2 caracteres.");
        if (name.Trim().Length > 100)
            throw new DomainException("Nome deve ter no máximo 100 caracteres.");
    }

    private static void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        if (!emailRegex.IsMatch(email))
            throw new DomainException("Formato de e-mail inválido.");
    }

    private static void ValidarRole(string role)
    {
        if (!new[] { "User", "Admin" }.Contains(role))
            throw new DomainException("Role inválida. Use 'User' ou 'Admin'.");
    }
}
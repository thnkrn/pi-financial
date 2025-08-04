using System.Text;

namespace Pi.User.Application.Options;

public class DbConfig
{
    public const string SectionName = "Database";
    private string _privateKey = string.Empty;
    private string _publicKey = string.Empty;
    private string _salt = string.Empty;

    public string PublicKey
    {
        get => _publicKey;
        set => _publicKey = Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }

    public string PrivateKey
    {
        get => _privateKey;
        set => _privateKey = Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }

    public string Salt
    {
        get => _salt;
        set => _salt = Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }
}
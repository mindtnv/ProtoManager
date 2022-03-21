using System.Text;
using ProtoManager.Abstractions;

namespace ProtoManager;

public class EmailGenerator : IEmailGenerator
{
    private static readonly Random Random = new();
    private readonly Func<string>[] _partGenerators =
    {
        () =>
        {
            var letters = "wrtpsdfghjklzxcvnbmaaaaaaeeeoooiii";
            return letters[Random.Next(letters.Length)].ToString();
        },
    };

    public string Generate()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Random.Next(7, 15); i++)
            sb.Append(_partGenerators[Random.Next(_partGenerators.Length)]());
        sb.Append(Random.Next(0, 100));
        return sb.ToString();
    }
}
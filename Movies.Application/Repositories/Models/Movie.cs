

using System.Text.RegularExpressions;

public partial class Movie{

    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required List<string> Genres { get; init; } = new();
    public required int YearOfRelease { get; set; }

    public  string Slug  => GenerateSlug();

    private  string GenerateSlug()
    {
        // var sluggedTitle = Regex.Replace(Title,"[^0-9A-Za-z _-]", string.Empty)
        // .ToLower().Replace(" ","-");

        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
        .ToLower().Replace(" ","-");
        return $"{sluggedTitle}-{YearOfRelease}";

    }


    [GeneratedRegex("[^0-9A-Za-z _-]",RegexOptions.NonBacktracking,10)]
    private static partial Regex SlugRegex();
    
}
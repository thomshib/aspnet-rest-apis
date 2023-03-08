

using FluentValidation;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{

    private static readonly string[] AcceptableSortFields ={
        "title", "yearofrelease"
    } ;

    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        
        RuleFor( x => x.SortField)
        .Must( x => x is null  || AcceptableSortFields.Contains(x,StringComparer.OrdinalIgnoreCase))
        .WithMessage("You can only sort by 'title' or  'yearofrelease' ");

        RuleFor( x => x.Page)
        .GreaterThanOrEqualTo(1);

        RuleFor( x => x.PageSize)
        .InclusiveBetween(1,25)
        .WithMessage("You get bewtween 1 and 25 movies per page");
    }
}
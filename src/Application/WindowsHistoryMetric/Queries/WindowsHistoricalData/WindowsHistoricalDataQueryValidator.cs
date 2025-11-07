namespace MonitorGlass.Application.WindowsHistoryMetric.Queries.WindowsHistoricalData
{
    internal sealed class WindowsHistoricalDataQueryValidator : AbstractValidator<WindowsHistoricalDataQuery>
    {
        public WindowsHistoricalDataQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number should be greater than zero.")
                .WithErrorCode("INVALID_PAGE_NUMBER");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size should be between 1 and 100.")
                .WithErrorCode("INVALID_PAGE_SIZE");

            RuleFor(x => x.From)
                .LessThan(x => x.To)
                .WithMessage("The 'From' date must be earlier than the 'To' date.")
                .WithErrorCode("INVALID_DATE_RANGE");
        }
    }
}

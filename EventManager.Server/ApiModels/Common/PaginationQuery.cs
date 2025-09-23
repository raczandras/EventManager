using System.ComponentModel.DataAnnotations;

namespace EventManager.Server.ApiModels.Common
{
    public class PaginationQuery : IValidatableObject
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int? Page { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be greater than 0.")]
        public int? PageSize { get; set; }

        public string? SortBy { get; set; }
        public bool Descending { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((Page.HasValue && !PageSize.HasValue) || (!Page.HasValue && PageSize.HasValue))
            {
                yield return new ValidationResult(
                    "Both Page and PageSize must be provided together.",
                    [nameof(Page), nameof(PageSize)]
                );
            }
        }
    }
}

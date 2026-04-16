using FluentValidation;
using DNAAnalysis.Api.Requests;
using DNAAnalysis.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace DNAAnalysis.Api.Validators
{
    public class CreateGeneticRequestFormValidator 
        : AbstractValidator<CreateGeneticRequestFormDto>
    {
        private readonly string[] AllowedExtensions = { ".txt" };

        public CreateGeneticRequestFormValidator()
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var father = request.FatherFile;
                    var mother = request.MotherFile;
                    var individual = request.IndividualFile;

                    // 🔥 fix swagger empty file
                    if (father != null && father.Length == 0) father = null;
                    if (mother != null && mother.Length == 0) mother = null;
                    if (individual != null && individual.Length == 0) individual = null;

                    // ================= MoreThanOne =================
                    if (request.TestType == TestType.MoreThanOne)
                    {
                        if (individual != null && father == null && mother == null)
                        {
                            context.AddFailure("This test requires father and mother files. Individual file is not allowed.");
                            return;
                        }

                        if (father == null && mother == null)
                        {
                            context.AddFailure("Father and Mother files are required.");
                            return;
                        }

                        if (individual != null)
                            context.AddFailure("Individual file is not allowed in this test.");

                        if (father == null)
                            context.AddFailure("Father file is required.");

                        if (mother == null)
                            context.AddFailure("Mother file is required.");

                        if (father != null && !IsValidExtension(father))
                            context.AddFailure("Invalid father file type. Only .txt allowed.");

                        if (mother != null && !IsValidExtension(mother))
                            context.AddFailure("Invalid mother file type. Only .txt allowed.");
                    }

                    // ================= Individual =================
                    if (request.TestType == TestType.Individual)
                    {
                        if ((father != null || mother != null) && individual == null)
                        {
                            context.AddFailure("This test requires only your file. Father or Mother files are not allowed.");
                            return;
                        }

                        if (father != null || mother != null)
                            context.AddFailure("Father or Mother files are not allowed in Individual test.");

                        if (individual == null)
                        {
                            context.AddFailure("Individual file is required.");
                            return;
                        }

                        if (individual != null && !IsValidExtension(individual))
                            context.AddFailure("Invalid file type. Only .txt allowed.");
                    }
                });
        }

        private bool IsValidExtension(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            return AllowedExtensions.Contains(extension);
        }
    }
}

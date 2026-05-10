using DNAAnalysis.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace DNAAnalysis.API.Validators;

public static class GeneticRequestValidator
{
    public static List<string> Validate(
        IFormFile? father,
        IFormFile? mother,
        IFormFile? individual,
        TestType testType)
    {
        var errors = new List<string>();

        // 🔥 fix swagger empty file
        if (father != null && father.Length == 0) father = null;
        if (mother != null && mother.Length == 0) mother = null;
        if (individual != null && individual.Length == 0) individual = null;

        // ================= MoreThanOne =================
        if (testType == TestType.MoreThanOne)
        {
            // ❌ لو رافع Individual بس
            if (individual != null && father == null && mother == null)
            {
                errors.Add("This test requires father and mother files. Individual file is not allowed.");
                return errors;
            }

            // ❌ لو رافع Individual مع الأب/الأم
            if (individual != null)
                errors.Add("Individual file is not allowed in this test.");

            // ❌ لو مفيش حاجة خالص
            if (father == null && mother == null)
            {
                errors.Add("Father and Mother files are required.");
                return errors;
            }

            // ❌ نقص واحد
            if (father == null)
                errors.Add("Father file is required.");

            if (mother == null)
                errors.Add("Mother file is required.");

            // ✅ validate type
            if (father != null)
            {
                var error = ValidateFileType(father);
                if (error != null) errors.Add(error);
            }

            if (mother != null)
            {
                var error = ValidateFileType(mother);
                if (error != null) errors.Add(error);
            }
        }

        // ================= Individual =================
        else if (testType == TestType.Individual)
        {
            // ❌ لو رافع أب/أم بس
            if ((father != null || mother != null) && individual == null)
            {
                errors.Add("This test requires only your file. Father or Mother files are not allowed.");
                return errors;
            }

            // ❌ لو رافع الاتنين مع بعض
            if (father != null || mother != null)
                errors.Add("Father or Mother files are not allowed in Individual test.");

            // ❌ لو مفيش حاجة خالص
            if (individual == null)
            {
                errors.Add("Individual file is required.");
                return errors;
            }

            // ✅ validate type
            if (individual != null)
            {
                var error = ValidateFileType(individual);
                if (error != null) errors.Add(error);
            }
        }

        return errors;
    }

    private static string? ValidateFileType(IFormFile file)
    {
       var allowed = new[] { ".txt", ".vcf" };

        var ext = Path.GetExtension(file.FileName).ToLower();

        if (!allowed.Contains(ext))
            return "Invalid file type. Only .txt and .vcf files are allowed.";

        return null;
    }
}
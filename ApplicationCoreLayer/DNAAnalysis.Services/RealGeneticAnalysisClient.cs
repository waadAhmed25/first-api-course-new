using System.Net.Http.Headers;
using DNAAnalysis.Services.Abstraction;
using DNAAnalysis.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace DNAAnalysis.Services;

public class RealGeneticAnalysisClient : IGeneticAnalysisClient
{
    private readonly HttpClient _httpClient;

    public RealGeneticAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AnalyzeAsync(
        IFormFile? fatherFile,
        IFormFile? motherFile,
        IFormFile? individualFile,
        TestType testType)
    {
        using var content = new MultipartFormDataContent();

        HttpResponseMessage response;

        // ================= SINGLE =================
        if (testType == TestType.Individual)
        {
            if (individualFile == null)
                throw new Exception("Individual file is required.");

            var fileContent =
                new StreamContent(individualFile.OpenReadStream());

            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(individualFile.ContentType)
                        ? "application/octet-stream"
                        : individualFile.ContentType);

            content.Add(
                fileContent,
                "file",
                individualFile.FileName);

            response = await _httpClient.PostAsync(
                "https://sama18-cftr-cystic-fibrosis-api.hf.space/analyze-single",
                content);
        }

        // ================= PARENTS =================
        else
        {
            if (fatherFile == null || motherFile == null)
                throw new Exception("Father and Mother files are required.");

            var fatherContent =
                new StreamContent(fatherFile.OpenReadStream());

            fatherContent.Headers.ContentType =
                new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(fatherFile.ContentType)
                        ? "application/octet-stream"
                        : fatherFile.ContentType);

            content.Add(
                fatherContent,
                "father_file",
                fatherFile.FileName);

            var motherContent =
                new StreamContent(motherFile.OpenReadStream());

            motherContent.Headers.ContentType =
                new MediaTypeHeaderValue(
                    string.IsNullOrWhiteSpace(motherFile.ContentType)
                        ? "application/octet-stream"
                        : motherFile.ContentType);

            content.Add(
                motherContent,
                "mother_file",
                motherFile.FileName);

            response = await _httpClient.PostAsync(
                "https://sama18-cftr-cystic-fibrosis-api.hf.space/analyze-parents",
                content);
        }

        // ================= ERROR =================
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(
                $"AI API Error: {(int)response.StatusCode} - {error}");
        }

        // ================= RAW JSON =================
        return await response.Content.ReadAsStringAsync();
    }
}
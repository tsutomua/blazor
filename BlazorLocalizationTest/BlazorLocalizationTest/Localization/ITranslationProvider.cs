namespace BlazorLocalizationTest.Localization
{
    public interface ITranslationProvider
    {
        string GetTranslation(string resourceId, string cultureCode);
    }
}
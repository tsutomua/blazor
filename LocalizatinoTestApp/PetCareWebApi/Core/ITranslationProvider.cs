namespace Core
{
    public interface ITranslationProvider
    {
        string GetTranslation(string resourceId, string cultureCode);
    }
}
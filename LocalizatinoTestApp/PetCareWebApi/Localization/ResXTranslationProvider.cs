using Microsoft.Extensions.Localization;

namespace Core
{
    public class ResXTranslationProvider : ITranslationProvider
    {
        private readonly IStringLocalizer stringLocalizer;

        public ResXTranslationProvider(IStringLocalizer stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }

        public string GetTranslation(string resourceId, string cultureCode)
        {
            return stringLocalizer.GetString(resourceId);           
        }
    }
}

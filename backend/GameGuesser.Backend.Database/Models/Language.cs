namespace GameGuesser.Backend.Database.Models;

public enum Language
{
    English = 0,
    //French = 1,
    Spanish = 2,
    //Dutch = 3
}

public static class LanguageUtils
{
    public static string? LanguageToEnglishString(Language lang)
    {
        return lang switch
        {
            Language.English => "english",
            //Language.French => "french",
            Language.Spanish => "spanish",
            //Language.Dutch => "dutch",
            _ => null
        };
    }

    public static Language? StringCountryCodeToLanguage(string str)
    {
        return str switch
        {
            "en" => Language.English,
            //"fr" => Language.French,
            "es" => Language.Spanish,
            //"nl" => Language.Dutch,
            _ => null
        };
    }

    public static string? LanguageToStringCountryCode(Language lang)
    {
        return lang switch
        {
            Language.English => "en",
            //Language.French => "fr",
            Language.Spanish => "es",
            //Language.Dutch => "nl",
            _ => null
        };
    }
}
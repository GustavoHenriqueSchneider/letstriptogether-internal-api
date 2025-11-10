namespace Infrastructure.Configurations;

public class EmailTemplateSettings
{
    public CompanyData Company { get; init; } = null!;
    public UrlData Url { get; init; } = null!;
}

public class CompanyData
{
    public string Name { get; init; } = null!;
    public string Contact { get; init; } = null!;
}
    
public class UrlData
{
    public string Website { get; init; } = null!;
    
    private readonly string _privacyPolicy = null!;
    public string PrivacyPolicy
    {
        get => Website + _privacyPolicy; 
        init =>  _privacyPolicy = value;
    }
    
    private readonly string _termsOfUse = null!;
    public string TermsOfUse
    {
        get => Website + _termsOfUse; 
        init =>  _termsOfUse = value;
    }
}
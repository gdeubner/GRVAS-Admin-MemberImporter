namespace GRVAS.Data.MemberImporter.Sheets;

internal class CredentialProvider : ICredentialProvider
{
    public SheetsService Service { get; set; }
    const string APPLICATION_NAME = "MemberImporter";
    private readonly string[] Scopes;
    public CredentialProvider()
    {
        Scopes = new string[] { SheetsService.Scope.Spreadsheets };

        InitializeService();
    }
    private void InitializeService()
    {
        var credential = GetCredentialsFromFile();
        Service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = APPLICATION_NAME
        });
    }
    private GoogleCredential GetCredentialsFromFile()
    {
        GoogleCredential credential;
        using (var stream = new FileStream($"{Directory.GetCurrentDirectory()}/client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }
        return credential;
    }
}

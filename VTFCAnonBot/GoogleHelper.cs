
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.Text;

namespace VTFCAnonBot;

public class GoogleHelper
{
    public readonly string token;
    public readonly string sheetFileName;
    public string ApplicationName = "";
    private UserCredential credential;
    private DriveService driveService;
    private SheetsService sheetService;
    private string sheetFileId;

    public GoogleHelper(string token, string sheetFileName) 
    {
        this.token = token;
        this.sheetFileName = sheetFileName;
    }

    public string[] Scopes { get; private set; } = new string[] { DriveService.Scope.Drive,
        SheetsService.Scope.Spreadsheets};
    public string SheetName { get; private set; }

    public async Task Start()
    {
        string credentialPath = Path.Combine(Environment.CurrentDirectory, ".credentials", ApplicationName);
        using (var strm = new MemoryStream(Encoding.UTF8.GetBytes(token)))
        {
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            clientSecrets: GoogleClientSecrets.FromStream(strm).Secrets,
            scopes: Scopes,
            user: "user",
            taskCancellationToken: CancellationToken.None,
            new FileDataStore(credentialPath, true));
        }

        this.driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        this.sheetService = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        var request = this.driveService.Files.List();
        var response = request.Execute(); 
        
        foreach ( var file in response.Files)
        {
            if(file.Name == sheetFileName)
            {
                sheetFileId = file.Id;
                break;
            }
        }

        if(!string.IsNullOrEmpty(sheetFileId))
        {
            var sheetRequest = sheetService.Spreadsheets.Get(sheetFileId);
            var sheetResponse = sheetRequest.Execute();

            SheetName = sheetResponse.Sheets[0].Properties.Title;
        }
    }


}

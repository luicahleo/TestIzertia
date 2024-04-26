using ClienteWebMSM.Models;
using ClienteWebMSM.Services;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ClienteWebMSM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        this._httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<RespondeMsm> EnviarSinProxy()
    {
        var client = _httpClientFactory.CreateClient("SWSMS");
        //var response = await client.GetAsync("api/SMS/EnviarSMS?telefono=0000-111&mensaje=Prueba API&alias=NUMPRUEBA&tipo=Alphabet&usuario=SMSGTESTIMATE&clave=SEGUIMIENTOGTESTIMATE");

        HttpRequestMessage message = new HttpRequestMessage();
        message.Headers.Add("Accept", "application/soap+xml; charset=utf-8");


        var proxyConfig = _configuration.GetSection("HttpClientWithProxy:Proxy");
        var smsWsConfig = _configuration.GetSection("HttpClientWithProxy:SMSWS");

        var proxyUrl = proxyConfig["Url"];
        var serverUrl = smsWsConfig["ServerURL"];

        message.Method = HttpMethod.Post;

        message.RequestUri = new Uri(serverUrl);

        var enviaSms = new EnviarSMS
        {
            matricula = "0000-111",
            movil = "NUMPRUEBA",
            texto = "Prueba API",
            usuario = "SMSGTESTIMATE",
            aplicacion = "SMSGTESTIMATE",
            categoria = "SEGUIMIENTOGTESTIMATE"
        };

        message.Content = new StringContent($"matricula={enviaSms.matricula}&movil={enviaSms.movil}&texto={enviaSms.texto}&usuario={enviaSms.usuario}&aplicacion={enviaSms.aplicacion}&categoria={enviaSms.categoria}", Encoding.UTF8,
                        "application/x-www-form-urlencoded");


        HttpResponseMessage apiResponse = null;

        apiResponse = await client.SendAsync(message);
        var apiContent = await apiResponse.Content.ReadAsStringAsync();


        RespondeMsm response = JsonConvert.DeserializeObject<RespondeMsm>(apiContent);
        return response;



        //SWSMSSoapClient smsClient = new SWSMSSoapClient(_httpClient, _configureInforma);
        //smsClient.EnviarSMSAsync("0000-111", "NUMPRUEBA", "Prueba API", "Alphabet", "SMSGTESTIMATE", "SEGUIMIENTOGTESTIMATE");
        //await _context.SaveChangesAsync();



    }

    public async Task Enviar()
    {

        var enviaSms = new EnviarSMS
        {
            matricula = "0000-111",
            movil = "NUMPRUEBA",
            texto = "Prueba API",
            usuario = "SMSGTESTIMATE",
            aplicacion = "SMSGTESTIMATE",
            categoria = "SEGUIMIENTOGTESTIMATE"
        };

        var proxyConfig = _configuration.GetSection("HttpClientWithProxy:Proxy");
        var proxyUrl = proxyConfig["Url"];
        var proxyPort = int.Parse(proxyConfig["Port"]);
        var proxyUsername = proxyConfig["Username"];
        var proxyPassword = proxyConfig["Password"];

        var proxy = new WebProxy()
        {
            Address = new Uri($"http://{proxyUrl}:{proxyPort}"),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,

            Credentials = new NetworkCredential(userName: proxyUsername, password: proxyPassword)
        };

        var httpClientHandler = new HttpClientHandler()
        {
            Proxy = proxy,
        };

        HttpClient client = new HttpClient(handler: httpClientHandler, disposeHandler: true);

        string soapMessage = $@"
        <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
            <soap:Body>
                <EnviarSMS xmlns=""http://tempuri.org/"">
                    <matricula>{enviaSms.matricula}</matricula>
                    <movil>{enviaSms.movil}</movil>
                    <texto>{enviaSms.texto}</texto>
                    <usuario>{enviaSms.usuario}</usuario>
                    <aplicacion>{enviaSms.aplicacion}</aplicacion>
                    <categoria>{enviaSms.categoria}</categoria>
                </EnviarSMS>
            </soap:Body>
        </soap:Envelope>";

        var content = new StringContent(soapMessage, Encoding.UTF8, "text/xml");

        HttpResponseMessage response = await client.PostAsync("url", content);

        if (response.IsSuccessStatusCode)
        {
        }
        else
        {
        }
    }


}

public class EnviarSMS
{
    public string matricula { get; set; }
    public string movil { get; set; }
    public string texto { get; set; }
    public string usuario { get; set; }
    public string aplicacion { get; set; }
    public string categoria { get; set; }
}
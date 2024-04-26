
namespace ClienteWebMSM.Services
{
    public class SendMsm : ISendMsm
    {
        public IHttpClientFactory _httpClient { get; set; }
        public SendMsm(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<RespondeMsm> EnviarSMSAsync<RespondeMsmT>(RequestMsn requestMsn)
        {
            var client = _httpClient.CreateClient("MSMAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");

            var response = new RespondeMsm();

            //if (requestMsn.Parametros == null)
            //{
            //    message.RequestUri = new Uri(requestMsn.Url);
            //}



            return response;

        }
    }
}

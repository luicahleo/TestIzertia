namespace ClienteWebMSM.Services;

public interface ISendMsm
{

    Task<RespondeMsm> EnviarSMSAsync<RespondeMsmT>(RequestMsn requestMsn);



}

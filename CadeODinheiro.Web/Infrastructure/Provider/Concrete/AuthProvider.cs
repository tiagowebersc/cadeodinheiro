using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using CadeODinheiro.Core.DTO;
using CadeODinheiro.Web.Infrastructure.Provider.Abstract;
using CadeODinheiro.Web.Infrastructure.Utils;
using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Infrastructure.Util;

namespace CadeODinheiro.Web.Infrastructure.Provider.Concrete
{
    public class AuthProvider : IAuthProvider
    {
        [Inject]
        public AuthModel authModel { get; set; }
        [Inject]
        public IUserBusiness userBusiness { get; set; }

        public bool Login(AuthModel authModel, out string msgError)
        {
            msgError = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(authModel.Login))
                {
                    msgError = "Usuário não informado!";
                    return false;
                }

                authModel.Login = authModel.Login.ToUpper();
                User user = userBusiness.Get.FirstOrDefault(u => u.login == authModel.Login);
                if (user == null)
                {
                    msgError = "Usuário não cadastrado!";
                    return false;
                }
                if (!user.senha.Equals(Encrypter.EncryptPass(authModel.Password)))
                {
                    msgError = "Senha incorreta!";
                    return false;
                }

                authModel.sID = user.sID;
                authModel.Nome = user.nome;
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                if (ex.InnerException != null)
                {
                    msgError += " - " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null) msgError += " - " + ex.InnerException.InnerException.Message;
                }
                return false;
            }
            
            
            // Gerar o ticket e armazena como cookie
            GenerateAndStoreTicket(authModel);

            return true;
        }

        private void GenerateAndStoreTicket(AuthModel authModel, int expiration = 180)
        {
            var ticketEnctrypt = generateTicketEncrypt(authModel, expiration);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEnctrypt);
            authCookie.Expires = DateTime.Now.AddMinutes(expiration);
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        public void Logout()
        {
            if (HttpContext.Current.Response.Cookies[".CadeODinheiroAuth"] != null)
            {
                var c = new HttpCookie(".CadeODinheiroAuth");
                c.Expires = DateTime.Now.AddDays(-1);
                //HttpContext.Current.Response.Cookies.Clear();
                HttpContext.Current.Response.Cookies.Add(c);
            }
            FormsAuthentication.SignOut();
        }

        public bool Authenticate
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        public AuthModel UserAntenticated
        {
            get
            {
                if (Authenticate)
                {
                    var identity = (FormsIdentity)HttpContext.Current.User.Identity;
                    var ticket = identity.Ticket;

                    authModel = Serializer.UnserializeAuthModel(ticket.UserData);

                    return authModel;
                }
                return null;
            }
        }

        public string generateTicketEncrypt(AuthModel authModel, int expirationInMinutes)
        {
            var authModeSerialized = Serializer.SerializeAuthModel(authModel);
            var ticket = new FormsAuthenticationTicket(1, authModel.Login, DateTime.Now, DateTime.Now.AddMinutes(expirationInMinutes), false, authModeSerialized, FormsAuthentication.FormsCookiePath);
            var ticketEncrypt = FormsAuthentication.Encrypt(ticket);
            return ticketEncrypt;
        }


        public bool IsValidLogicPass(string logicPass)
        {
            /*
             * Senha Lógica:
             * Hora (2 dígitos) * 2
             * Número do dia da semana (Dom-Sáb -- 0-7)
             * Primeira Letra do mês
             * Dia (2 dígitos)
             */
            var Hora = Convert.ToInt32(DateTime.Now.ToString("HH")) * 2;
            var DiaSem = (int)DateTime.Now.DayOfWeek;
            var Mes = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month).Substring(0,1).ToUpper();
            var Dia = DateTime.Now.ToString("dd");

            var senhaAtual = Hora + DiaSem.ToString() + Mes + Dia;
            if (senhaAtual != logicPass)
                return false;
            return true;
        }
    }
}
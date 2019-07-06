using CadeODinheiro.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Web.Infrastructure.Provider.Abstract
{
    public interface IAuthProvider
    {
        bool Login(AuthModel authModel, out string msgError);

        void Logout();

        bool Authenticate { get; }

        AuthModel UserAntenticated { get; }

        string generateTicketEncrypt(AuthModel authModel, int expirationInMinutes);

        bool IsValidLogicPass(string logicPass);
    }

    

}

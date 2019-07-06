using CadeODinheiro.Core.Business.Abstract;
using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Business.Concrete
{
    public class UserBusiness : BaseBusiness<User>, IUserBusiness
    {
        public override void Insert(User userParam)
        {
            if (Get.Any(u => u.login == userParam.login)) throw new InvalidOperationException("Já existe um usuário com este login");
            userParam.senha = Encrypter.EncryptPass(userParam.senha);
            userParam.login = userParam.login.ToUpper();
            base.Insert(userParam);
        }

        public override void Update(User userParam)
        {
            if (userParam.senha == null)
                throw new InvalidOperationException("Senha não pode ser vazia");

            if (userParam.senha.Length != 32)
                userParam.senha = Encrypter.EncryptPass(userParam.senha);

            base.Update(userParam);
        }

    }
}

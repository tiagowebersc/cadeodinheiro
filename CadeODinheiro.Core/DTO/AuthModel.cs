using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CadeODinheiro.Core.DTO
{
    [Serializable]
    public class AuthModel
    {
        [Display(Name = "ID")]
        public string sID { get; set; }

        [Required(ErrorMessage = "Login é obrigatório")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [Display(Name = "Senha")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Nome { get; set; }
    }
}

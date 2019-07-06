using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DTO
{
    public class MyDataModel
    {
        [Display(Name = "Login")]
        public string login { get; set; }

        [Display(Name = "Nome")]
        [StringLength(80)]
        [Required(ErrorMessage = "Nome deve ser informado!")]
        public string nome { get; set; }

        [Display(Name = "Senha")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string senha { get; set; }

        [Display(Name = "Confirmar Senha")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string confirmarSenha { get; set; }
    }
}

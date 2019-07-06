using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbUser")]
    public class User : BaseEntity
    {
        [Display(Name="Login")]
        [StringLength(10)]
        [Required(ErrorMessage="Login deve ser informado!")]
        public string login { get; set; }

        [Display(Name = "Senha")]
        [StringLength(255)]
        [MinLength(5)]
        [Required(ErrorMessage = "Senha deve ser informado!")]
        public string senha { get; set; }

        [Display(Name = "Nome")]
        [StringLength(80)]
        [Required(ErrorMessage = "Nome deve ser informado!")]
        public string nome { get; set; }
    }
}

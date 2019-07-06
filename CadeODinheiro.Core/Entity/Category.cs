using CadeODinheiro.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbCategory")]
    public class Category : BaseEntity
    {
        [Display(Name = "Descrição")]
        [StringLength(50)]
        [Required(ErrorMessage = "Descrição deve ser informado!")]
        public string descricao { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Tipo deve ser informado!")]
        public CategoryType CategoryType { get; set; }

        [ForeignKey("sUserID")]
        public User User { get; set; }
        [Required]
        public string sUserID { get; set; }

    }
}

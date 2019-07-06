using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbParameter")]
    public class Parameter : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string sParametro { get; set; }

        [Required]
        [StringLength(200)]
        public string sValor { get; set; }

        [StringLength(250)]
        public string sDescricao { get; set; }
    }
}

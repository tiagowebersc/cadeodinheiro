using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    [Table("tbLog")]
    public class Log : BaseEntity
    {
        [StringLength(25)]
        public string sOperacao { get; set; }

        public string sAgent { get; set; }

        public string sAction { get; set; }

        public string sController { get; set; }

        public string sSession { get; set; }

        [StringLength(20)]
        public string sIP { get; set; }
    }
}

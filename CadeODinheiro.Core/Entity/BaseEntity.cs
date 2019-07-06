using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.Entity
{
    public abstract class BaseEntity
    {
        [Key]
        [Required]
        public String sID { get; set; }

        [Required]
        public DateTime dRecord { get; set; }

        public BaseEntity(){
            this.sID = Guid.NewGuid().ToString();
        }
    }
}

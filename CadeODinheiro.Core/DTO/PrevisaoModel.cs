using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadeODinheiro.Core.DTO
{
    public class PrevisaoModel
    {
        public PrevisaoModel()
        {
            totalAPagar1 = 0;
            totalAPagar2 = 0;
            totalAPagar3 = 0;
            totalAPagar4 = 0;
            totalAPagar5 = 0;
            totalAPagar6 = 0;
            totalAPagar7 = 0;
            totalAPagar8 = 0;
            totalPago1 = 0;
            totalPago2 = 0;
            totalPago3 = 0;
            totalPago4 = 0;
            totalPago5 = 0;
            totalPago6 = 0;
            totalPago7 = 0;
            totalPago8 = 0;
            total1 = 0;
            total2 = 0;
            total3 = 0;
            total4 = 0;
            total5 = 0;
            total6 = 0;
            total7 = 0;
            total8 = 0;
        }

        public string sMes1 { get; set; }
        public List<PrevisaoMesModel> lstMes1 { get; set; }
        public double totalAPagar1 { get; set; }
        public double totalPago1 { get; set; }
        public double total1 { get; set; }

        public string sMes2 { get; set; }
        public List<PrevisaoMesModel> lstMes2 { get; set; }
        public double totalAPagar2 { get; set; }
        public double totalPago2 { get; set; }
        public double total2 { get; set; }
        
        public string sMes3 { get; set; }
        public List<PrevisaoMesModel> lstMes3 { get; set; }
        public double totalAPagar3 { get; set; }
        public double totalPago3 { get; set; }
        public double total3 { get; set; }
        
        public string sMes4 { get; set; }
        public List<PrevisaoMesModel> lstMes4 { get; set; }
        public double totalAPagar4 { get; set; }
        public double totalPago4 { get; set; }
        public double total4 { get; set; }
        
        public string sMes5 { get; set; }
        public List<PrevisaoMesModel> lstMes5 { get; set; }
        public double totalAPagar5 { get; set; }
        public double totalPago5 { get; set; }
        public double total5 { get; set; }

        public string sMes6 { get; set; }
        public List<PrevisaoMesModel> lstMes6 { get; set; }
        public double totalAPagar6 { get; set; }
        public double totalPago6 { get; set; }
        public double total6 { get; set; }

        public string sMes7 { get; set; }
        public List<PrevisaoMesModel> lstMes7 { get; set; }
        public double totalAPagar7 { get; set; }
        public double totalPago7 { get; set; }
        public double total7 { get; set; }

        public string sMes8 { get; set; }
        public List<PrevisaoMesModel> lstMes8 { get; set; }
        public double totalAPagar8 { get; set; }
        public double totalPago8 { get; set; }
        public double total8 { get; set; }
        
        public string sDataGrafico { get; set; }
    }

    public class PrevisaoMesModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime dData { get; set; }

        public string sDescricao { get; set; }

        public string sAccountID { get; set; }

        public string sDescConta { get; set; }

        public string sCategoryID { get; set; }

        public string sDescCategoria { get; set; }

        public double dValor { get; set; }

        public string sAgrupadorID { get; set; }

        public Boolean bPago { get; set; }
    }
}

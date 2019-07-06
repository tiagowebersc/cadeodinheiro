using CadeODinheiro.Core.Entity;
using CadeODinheiro.Core.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CadeODinheiro.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceCadeODinheiro
    {
        [OperationContract]
        string retornoTeste();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    //[DataContract]
    //public class CostCenterWS
    //{
    //    [DataMember]
    //    public string sID { get; set; }
    //    [DataMember]
    //    public string InventoryID { get; set; }
    //    [DataMember]
    //    public String sErpCode { get; set; }
    //    [DataMember]
    //    public String sName { get; set; }
    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FormSizeService
{
    /// <summary>
    /// This is a DTO that serialises a form types
    /// </summary>
    [DataContract]
    public class TypeModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        public TypeModel(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

    }

    public class FormType
    {
        /// <summary>
        /// This is the form types as an emun better than a table in a db 
        /// </summary>
        public enum RootType
        {
            Pot,
            RootBall,
            BareRoot,
            Bulb,
            Topiary
        }

        /// <summary>
        /// Build a list of Form Types ( TypeModels) from the emun RootType
        /// </summary>
        /// <returns></returns>
        public static List<TypeModel> buildList()
        {
            List<TypeModel> Outlist = new List<TypeModel>();
            foreach (int value in Enum.GetValues(typeof(RootType)))
            {
                TypeModel tm = new TypeModel(value, ((RootType)value).ToString());
                Outlist.Add(tm);
            }
            return Outlist;
        }

    }

    
}

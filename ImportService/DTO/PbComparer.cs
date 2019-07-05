using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImportService.DTO
{
    // Custom comparer for the Product class
    class PbComparer : IEqualityComparer<Models.Pannebakker>
    {
        // Products are equal if their names and product numbers are equal.
        public bool Equals(Models.Pannebakker x, Models.Pannebakker y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.FormSize == y.FormSize && x.Name == y.Name && x.FormSizeCode == y.FormSizeCode && x.Sku == y.Sku;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Models.Pannebakker product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashName = product.Name == null ? 0 : product.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashFSC = product.FormSizeCode == null ? 0 : product.FormSizeCode.GetHashCode();

            int hashFS = product.FormSize == null ? 0 : product.FormSize.GetHashCode();

            int hashSKU = product.Sku == null ? 0 : product.FormSizeCode.GetHashCode();

            //Calculate the hash code for the product.
            return hashName ^ hashFS ^ hashFSC ^ hashSKU;
        }

    }
}
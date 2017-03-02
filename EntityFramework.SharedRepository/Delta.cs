using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.Repository6
{
    public class Delta<T> where T : class
    {
        List<string> Fields { get; set; }
        public T Internal { get; set; }

        public List<string> UpdatedFields()
        {
            return Fields;
        }

        public Delta()
        {
            Internal = (T)Activator.CreateInstance(typeof(T));
            Fields = new List<string>();
        }

        public void SetValue(string fieldName, object data)
        {
            var prop1 = typeof(T).GetProperty(fieldName);
            if (prop1 == null)
            {
                throw new ArgumentException($"{fieldName} was not a property found on the object.");
            }
            try
            {
                prop1.SetValue(Internal, data);
                Fields.Add(fieldName);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
        }
    }
}

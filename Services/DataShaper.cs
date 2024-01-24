using Entities.Models;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
/*
 1.Adım=Tüm propertyleri aldık.
 2.Adım=İhtiyaç duyulan (required propertyleri) aldık.
 3.Adım=Value(key-value)
 4.Adım=Liste
 */
namespace Services
{
    public class DataShaper<T> : IDataShaper<T>
        where T : class
    {
        public PropertyInfo[] Properties { get; set; }
        public DataShaper()
        {
            Properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredFields=GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredFields);
        }
        public ShapedEntity ShapeData(T entity, string fieldsString)
            /*ExpandoObject= Herhangi bir nesne olabilir*/
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        }
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            /* /books? fields=id, title
             QueryString=id, title ihtiyaç var
             Book=id,title,price(x)*/
            var requiredFields=new List<PropertyInfo>();
            if(!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields=fieldsString.Split(',',StringSplitOptions.RemoveEmptyEntries);
                foreach (var field in fields)
                {
                    var property=Properties
                        .FirstOrDefault(pi=>pi.Name.Equals(field.Trim(),StringComparison.InvariantCultureIgnoreCase));
                    if (property is null)
                        continue;
                    requiredFields.Add(property);
                }
            }
            else
            {
                requiredFields=Properties.ToList();
            }
            return requiredFields;
        }
        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();
            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }
            var objectProperty = entity.GetType().GetProperty("ID");
            shapedObject.ID = (int)objectProperty.GetValue(entity);
            return shapedObject;
        }
        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities,
            IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();/*ExpandoObject çalışma zamanında dinamik olarak üretilen herhangi bir nesneye karşılık olarak gelebiliyor.*/
            foreach (var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
    }
}